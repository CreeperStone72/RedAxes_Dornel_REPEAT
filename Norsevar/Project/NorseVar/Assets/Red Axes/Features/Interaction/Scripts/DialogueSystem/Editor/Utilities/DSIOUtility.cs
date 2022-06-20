using System;
using System.Collections.Generic;
using System.Linq;
using Norsevar.Interaction.DialogueSystem.Utilities;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityObject = UnityEngine.Object;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    public static class DSIOUtility
    {

        #region Constants and Statics

        private const string ROOT_FOLDER_PATH = "Assets/Red Axes/Features/Interaction";
        public static readonly string EditorFolderPath = $"{ROOT_FOLDER_PATH}/Scripts/DialogueSystem/Editor";

        private static string _graphFileName;
        private static string _containerFolderPath;
        private static DSGraphView _graphView;
        private static List<DSGroup> _groups;
        private static List<DSNode> _nodes;

        private static Dictionary<string, DSDialogueGroupSo> _createdDialogueGroups;
        private static Dictionary<string, DSDialogueSo> _createdDialogues;
        private static Dictionary<string, DSGroup> _loadedGroups;
        private static Dictionary<string, DSNode> _loadedNodes;

        #endregion

        #region Private Methods

        private static List<DSChoiceSaveData> CloneNodeChoices(IEnumerable<DSChoiceSaveData> nodeChoices)
        {
            return (from choice in nodeChoices select new DSChoiceSaveData { Text = choice.Text, NodeID = choice.NodeID }).ToList();
        }

        private static List<DSDialogueChoiceData> ConvertNodeChoicesToDialogueChoices(IEnumerable<DSChoiceSaveData> nodeChoices)
        {
            return (from nodeChoice in nodeChoices select new DSDialogueChoiceData { Text = nodeChoice.Text }).ToList();
        }

        private static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            T asset = LoadAsset<T>(path, assetName);

            if (asset is not null) return asset;

            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, fullPath);
            return asset;
        }

        private static void CreateFolder(string path, string folderName)
        {
            if (AssetDatabase.IsValidFolder($"{path}/{folderName}")) return;
            AssetDatabase.CreateFolder(path, folderName);
        }

        private static void CreateStaticFolders()
        {

            CreateFolder(EditorFolderPath, "Graphs");
            CreateFolder(ROOT_FOLDER_PATH, "DialogueSystem");
            CreateFolder($"{ROOT_FOLDER_PATH}/DialogueSystem", "Dialogues");
            CreateFolder($"{ROOT_FOLDER_PATH}/DialogueSystem/Dialogues", _graphFileName);

            CreateFolder(_containerFolderPath, "Global");
            CreateFolder(_containerFolderPath, "Groups");

            CreateFolder($"{_containerFolderPath}/Global", "Dialogues");
        }

        private static void GetElementsFromGraphView()
        {
            _graphView.graphElements.ForEach(
                element =>
                {
                    switch (element)
                    {
                        case DSNode node:
                            _nodes.Add(node);
                            return;
                        case DSGroup dsGroup:
                            _groups.Add(dsGroup);
                            return;
                    }

                });
        }

        private static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        private static void LoadGroups(List<DSGroupSaveData> groups)
        {
            foreach (DSGroupSaveData groupData in groups)
            {
                DSGroup group = _graphView.CreateGroup(groupData.Name, groupData.Position);
                group.Id = groupData.Id;
                _loadedGroups.Add(group.Id, group);
            }
        }

        private static void LoadNodeConnections()
        {
            foreach ((string _, DSNode loadedNode) in _loadedNodes)
            {
                foreach (Port port in loadedNode.outputContainer.Children().Cast<Port>())
                {
                    DSChoiceSaveData choiceData = (DSChoiceSaveData)port.userData;

                    if (string.IsNullOrEmpty(choiceData.NodeID)) continue;

                    DSNode nextTextNode = _loadedNodes[choiceData.NodeID];

                    Port nextNodeInputPort = (Port)nextTextNode.inputContainer.Children().First();

                    Edge edge = port.ConnectTo(nextNodeInputPort);

                    _graphView.AddElement(edge);

                    loadedNode.RefreshPorts();
                }
            }
        }

        private static void LoadNodes(List<DSNodeSaveData> nodes)
        {
            foreach (DSNodeSaveData nodeData in nodes)
            {
                List<DSChoiceSaveData> choices = CloneNodeChoices(nodeData.Choices);

                DSNode node = _graphView.CreateNode(nodeData.Name, nodeData.DialogueType, nodeData.Position, false);

                node.Id = nodeData.Id;
                node.Choices = choices;

                if (node.DialogueType == DSDialogueType.Action)
                    ((DSActionNode)node).Action = ((DSActionNodeSaveData)nodeData).Action;
                else
                    ((DSTextNode)node).Text = ((DSTextNodeSaveData)nodeData).Text;

                node.Draw();

                _graphView.AddElement(node);

                _loadedNodes.Add(node.Id, node);

                if (string.IsNullOrEmpty(nodeData.GroupID)) continue;

                DSGroup group = _loadedGroups[nodeData.GroupID];
                node.Group = group;

                group.AddElement(node);
            }
        }

        private static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        private static void RemoveFolder(string fullPath)
        {
            FileUtil.DeleteFileOrDirectory($"{fullPath}.meta");
            FileUtil.DeleteFileOrDirectory($"{fullPath}/");
        }

        private static void SaveAsset(UnityObject asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void SaveGroups(DSGraphSaveDataSo graphData, DSDialogueContainerSo dialogueContainer)
        {
            List<string> groupNames = new();
            foreach (DSGroup dsGroup in _groups)
            {
                SaveGroupToGraph(dsGroup, graphData);
                SaveGroupToSo(dsGroup, dialogueContainer);

                groupNames.Add(dsGroup.title);
            }

            UpdateOldGroups(groupNames, graphData);
        }

        private static void SaveGroupToGraph(DSGroup dsGroup, DSGraphSaveDataSo graphData)
        {
            DSGroupSaveData groupData = new()
            {
                Id = dsGroup.Id,
                Name = dsGroup.title,
                Position = dsGroup.GetPosition().position
            };

            graphData.Groups.Add(groupData);
        }

        private static void SaveGroupToSo(DSGroup dsGroup, DSDialogueContainerSo dialogueContainer)
        {
            string groupName = dsGroup.title;

            CreateFolder($"{_containerFolderPath}/Groups", groupName);
            CreateFolder($"{_containerFolderPath}/Groups/{groupName}", "Dialogues");

            DSDialogueGroupSo dialogueGroup = CreateAsset<DSDialogueGroupSo>($"{_containerFolderPath}/Groups/{groupName}", groupName);

            dialogueGroup.Initialize(groupName);

            _createdDialogueGroups.Add(dsGroup.Id, dialogueGroup);

            dialogueContainer.DialogueGroups.Add(dialogueGroup, new List<DSDialogueSo>());

            SaveAsset(dialogueGroup);
        }

        private static void SaveNodes(DSGraphSaveDataSo graphData, DSDialogueContainerSo dialogueContainer)
        {

            SerializedDictionary<string, List<string>> groupedNodeNames = new();
            List<string> ungroupedNodeNames = new();

            foreach (DSNode node in _nodes)
            {
                SaveNodeToGraph(node, graphData);
                SaveNodeToSo(node, dialogueContainer);

                if (node.Group is not null)
                {
                    groupedNodeNames.AddItem(node.Group.title, node.DialogueName);
                    continue;
                }

                ungroupedNodeNames.Add(node.DialogueName);
            }

            UpdateDialogueChoicesConnected();
            UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
            UpdateOldGroupedNodes(groupedNodeNames, graphData);
        }

        private static void SaveNodeToGraph(DSNode node, DSGraphSaveDataSo graphData)
        {
            List<DSChoiceSaveData> choices = CloneNodeChoices(node.Choices);

            DSNodeSaveData saveData;
            switch (node.DialogueType)
            {

                case DSDialogueType.Single:
                case DSDialogueType.Multiple:
                    saveData = new DSTextNodeSaveData
                    {
                        Id = node.Id,
                        Name = node.DialogueName,
                        Choices = choices,
                        Text = ((DSTextNode)node).Text,
                        GroupID = node.Group?.Id,
                        DialogueType = node.DialogueType,
                        Position = node.GetPosition().position
                    };
                    break;
                case DSDialogueType.Action:
                    saveData = new DSActionNodeSaveData
                    {
                        Id = node.Id,
                        Name = node.DialogueName,
                        Choices = choices,
                        Action = ((DSActionNode)node).Action,
                        GroupID = node.Group?.Id,
                        DialogueType = node.DialogueType,
                        Position = node.GetPosition().position
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            graphData.Nodes.Add(saveData);
        }

        private static void SaveNodeToSo(DSNode node, DSDialogueContainerSo dialogueContainer)
        {
            DSDialogueSo dialogue;

            if (node.Group is not null)
            {
                dialogue = CreateAsset<DSDialogueSo>($"{_containerFolderPath}/Groups/{node.Group.title}/Dialogues", node.DialogueName);

                dialogueContainer.DialogueGroups.AddItem(_createdDialogueGroups[node.Group.Id], dialogue);
            }
            else
            {
                dialogue = CreateAsset<DSDialogueSo>($"{_containerFolderPath}/Global/Dialogues", node.DialogueName);
                dialogueContainer.UngroupedDialogues.Add(dialogue);
            }

            switch (node.DialogueType)
            {

                case DSDialogueType.Single:
                case DSDialogueType.Multiple:
                    dialogue.Initialize(
                        node.DialogueName,
                        ((DSTextNode)node).Text,
                        ConvertNodeChoicesToDialogueChoices(node.Choices),
                        node.DialogueType,
                        node.IsStartingNode());
                    break;
                case DSDialogueType.Action:
                    dialogue.Initialize(
                        node.DialogueName,
                        ((DSActionNode)node).Action,
                        ConvertNodeChoicesToDialogueChoices(node.Choices),
                        node.DialogueType,
                        node.IsStartingNode());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _createdDialogues.Add(node.Id, dialogue);

            SaveAsset(dialogue);
        }

        private static void UpdateDialogueChoicesConnected()
        {
            foreach (DSNode node in _nodes)
            {
                DSDialogueSo dialogue = _createdDialogues[node.Id];

                for (int i = 0; i < node.Choices.Count; i++)
                {
                    DSChoiceSaveData nodeChoice = node.Choices[i];

                    if (string.IsNullOrEmpty(nodeChoice.NodeID)) continue;

                    dialogue.Choices[i].NextDialogue = _createdDialogues[nodeChoice.NodeID];

                    SaveAsset(dialogue);
                }
            }
        }

        private static void UpdateOldGroupedNodes(SerializedDictionary<string, List<string>> currentNodeNames, DSGraphSaveDataSo graphData)
        {
            if (graphData.OldGroupedNodeNames is not null && graphData.OldGroupedNodeNames.Count is not 0)
            {
                foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeNames)
                {
                    List<string> nodesToRemove = new();

                    if (currentNodeNames.ContainsKey(oldGroupedNode.Key))
                        nodesToRemove = oldGroupedNode.Value.Except(currentNodeNames[oldGroupedNode.Key]).ToList();

                    foreach (string nodeToRemove in nodesToRemove)
                        RemoveAsset($"{_containerFolderPath}/Groups/{oldGroupedNode.Key}/Dialogues", nodeToRemove);
                }
            }

            graphData.OldGroupedNodeNames = new SerializedDictionary<string, List<string>>();

            foreach ((string key, List<string> value) in currentNodeNames)
                graphData.OldGroupedNodeNames.Add(key, value);
        }

        private static void UpdateOldGroups(IReadOnlyCollection<string> currentGroupNames, DSGraphSaveDataSo graphData)
        {
            if (graphData.OldGroupNames is not null && graphData.OldGroupNames.Count is not 0)
            {
                List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();

                foreach (string groupToRemove in groupsToRemove)
                    RemoveFolder($"{_containerFolderPath}/Groups/{groupToRemove}");
            }

            graphData.OldGroupNames = new List<string>(currentGroupNames);
        }

        private static void UpdateOldUngroupedNodes(IReadOnlyCollection<string> currentNodeNames, DSGraphSaveDataSo graphData)
        {
            if (graphData.OldNodeNames is not null && graphData.OldNodeNames.Count is not 0)
            {
                List<string> nodesToRemove = graphData.OldNodeNames.Except(currentNodeNames).ToList();

                foreach (string nodeToRemove in nodesToRemove)
                    RemoveAsset($"{_containerFolderPath}/Global/Dialogues", nodeToRemove);
            }

            graphData.OldNodeNames = new List<string>(currentNodeNames);
        }

        #endregion

        #region Public Methods

        public static void Initialize(DSGraphView dsGraphView, string graphName)
        {
            _graphView = dsGraphView;
            _graphFileName = graphName;

            _groups = new List<DSGroup>();
            _nodes = new List<DSNode>();
            _createdDialogueGroups = new Dictionary<string, DSDialogueGroupSo>();
            _createdDialogues = new Dictionary<string, DSDialogueSo>();
            _loadedGroups = new Dictionary<string, DSGroup>();
            _loadedNodes = new Dictionary<string, DSNode>();

            _containerFolderPath = $"{ROOT_FOLDER_PATH}/DialogueSystem/Dialogues/{_graphFileName}";
        }

        public static void Load()
        {
            DSGraphSaveDataSo graphData = LoadAsset<DSGraphSaveDataSo>($"{EditorFolderPath}/Graphs", _graphFileName);

            if (graphData is null)
            {
                EditorUtility.DisplayDialog("File not found", "Could not load the file.", "OK");
                return;
            }

            DSEditorWindow.UpdateFileName(graphData.FileName);

            LoadGroups(graphData.Groups);
            LoadNodes(graphData.Nodes);
            LoadNodeConnections();
        }

        public static void Save()
        {
            CreateStaticFolders();
            GetElementsFromGraphView();

            DSGraphSaveDataSo graphData = CreateAsset<DSGraphSaveDataSo>($"{EditorFolderPath}/Graphs", $"{_graphFileName}Graph");

            graphData.Initialize(_graphFileName);

            DSDialogueContainerSo dialogueContainer = CreateAsset<DSDialogueContainerSo>(_containerFolderPath, _graphFileName);
            dialogueContainer.Initialize(_graphFileName);

            SaveGroups(graphData, dialogueContainer);
            SaveNodes(graphData, dialogueContainer);

            SaveAsset(graphData);

            SaveAsset(dialogueContainer);
        }

        #endregion

    }

}
