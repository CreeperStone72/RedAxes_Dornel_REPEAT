using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Serialization;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    public class DSGraphView : GraphView
    {

        #region Private Fields

        private readonly DSEditorWindow _editorWindow;

        [OdinSerialize]
        private readonly SerializedDictionary<string, DSNodeErrorData> _ungroupedNodes;

        [OdinSerialize]
        private readonly SerializedDictionary<Group, SerializedDictionary<string, DSNodeErrorData>> _groupedNodes;

        [OdinSerialize]
        private readonly SerializedDictionary<string, DSGroupErrorData> _groups;

        private MiniMap _miniMap;

        private DSSearchWindow _searchWindow;

        private int _nameErrorAmount;

        #endregion

        #region Constructors

        public DSGraphView(DSEditorWindow editorWindow)
        {
            _editorWindow = editorWindow;
            _ungroupedNodes = new SerializedDictionary<string, DSNodeErrorData>();
            _groups = new SerializedDictionary<string, DSGroupErrorData>();
            _groupedNodes = new SerializedDictionary<Group, SerializedDictionary<string, DSNodeErrorData>>();

            AddManipulators();
            AddGridBackground();
            AddSearchWindow();
            AddMiniMap();

            OnElementsDeleted();
            OnGroupElementsAdded();
            OnGroupElementsRemoved();
            OnGroupRenamed();
            OnGraphViewChanged();

            AddStyles();
            AddMiniMapStyles();
        }

        #endregion

        #region Properties

        public int NameErrorAmount
        {
            get => _nameErrorAmount;
            set
            {
                _nameErrorAmount = value;

                switch (_nameErrorAmount)
                {
                    case 0:
                        _editorWindow.EnableSaving();
                        break;
                    case 1:
                        _editorWindow.DisableSaving();
                        break;
                }

            }
        }

        #endregion

        #region Private Methods

        private void AddGridBackground()
        {
            GridBackground gridBackground = new();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }

        private void AddGroup(DSGroup group)
        {
            string groupName = group.title.ToLower();

            if (!_groups.ContainsKey(groupName))
            {
                DSGroupErrorData groupErrorData = new();

                groupErrorData.Groups.Add(group);

                _groups.Add(groupName, groupErrorData);
                return;
            }

            List<DSGroup> groups = _groups[groupName].Groups;

            groups.Add(group);

            Color errorColor = _groups[groupName].ErrorData.Color;

            group.SetErrorStyle(errorColor);

            if (groups.Count != 2) return;
            NameErrorAmount--;
            groups[0].SetErrorStyle(errorColor);
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateNodeContextualMenu("Add Single Choice Node", DSDialogueType.Single));
            this.AddManipulator(CreateNodeContextualMenu("Add Multiple Choice Node", DSDialogueType.Multiple));

            this.AddManipulator(CreateGroupContextualMenu());

        }

        private void AddMiniMap()
        {
            _miniMap = new MiniMap
            {
                anchored = true,
                maxHeight = 150
            };
            _miniMap.SetPosition(new Rect(15, 50, 200, 150));

            Add(_miniMap);

            _miniMap.visible = false;
        }

        private void AddMiniMapStyles()
        {
            StyleColor backgroundColor = new(new Color32(29, 29, 29, 255));
            StyleColor borderColor = new(new Color32(51, 51, 51, 255));

            _miniMap.style.backgroundColor = backgroundColor;
            _miniMap.style.borderBottomColor = borderColor;
            _miniMap.style.borderTopColor = borderColor;
            _miniMap.style.borderRightColor = borderColor;
            _miniMap.style.borderLeftColor = borderColor;
        }

        private void AddSearchWindow()
        {
            if (_searchWindow is null)
            {
                _searchWindow = ScriptableObject.CreateInstance<DSSearchWindow>();
                _searchWindow.Initialize(this);
            }

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }

        private void AddStyles()
        {
            this.AddStyleSheets("Dialogue System/DSGraphViewStyles.uss", "Dialogue System/DSNodeStyles.uss");
        }

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new(
                menuEvent => menuEvent.menu.AppendAction(
                    "Add Group",
                    actionEvent => CreateGroup("DialogueGroup", GetLocalMousePosition(actionEvent.eventInfo.mousePosition))));

            return contextualMenuManipulator;
        }

        private IManipulator CreateNodeContextualMenu(string actionTitle, DSDialogueType dialogueType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new(
                menuEvent => menuEvent.menu.AppendAction(
                    actionTitle,
                    actionEvent => AddElement(
                        CreateNode("DialogueName", dialogueType, GetLocalMousePosition(actionEvent.eventInfo.mousePosition)))));

            return contextualMenuManipulator;
        }

        private void OnElementsDeleted()
        {
            deleteSelection = (_, _) =>
            {
                Type groupType = typeof(DSGroup);
                Type edgeType = typeof(Edge);

                List<DSNode> nodesToDelete = new();
                List<Edge> edgesToDelete = new();
                List<DSGroup> groupsToDelete = new();

                foreach (GraphElement element in selection.Cast<GraphElement>())
                {
                    if (element is DSNode node)
                    {
                        nodesToDelete.Add(node);
                        continue;
                    }

                    if (element.GetType() == edgeType)
                    {
                        Edge edge = (Edge)element;

                        edgesToDelete.Add(edge);
                    }

                    if (element.GetType() != groupType) continue;

                    DSGroup group = (DSGroup)element;

                    groupsToDelete.Add(group);
                }

                foreach (DSGroup dsGroup in groupsToDelete)
                {
                    List<DSNode> groupNodes = new();

                    foreach (GraphElement element in dsGroup.containedElements)
                    {
                        if (element is not DSNode node) continue;

                        groupNodes.Add(node);
                    }

                    dsGroup.RemoveElements(groupNodes);

                    RemoveGroup(dsGroup);

                    RemoveElement(dsGroup);
                }

                DeleteElements(edgesToDelete);

                foreach (DSNode node in nodesToDelete)
                {
                    if (node.Group is not null) node.Group.RemoveElement(node);

                    RemoveUngroupedNode(node);

                    node.DisconnectAllPorts();

                    RemoveElement(node);
                }
            };
        }

        private void OnGraphViewChanged()
        {
            graphViewChanged = change =>
            {
                if (change.edgesToCreate is not null)
                {
                    foreach (Edge edge in change.edgesToCreate)
                    {
                        DSNode nextTextNode = (DSNode)edge.input.node;

                        DSChoiceSaveData choiceData = (DSChoiceSaveData)edge.output.userData;

                        choiceData.NodeID = nextTextNode.Id;
                    }
                }

                if (change.elementsToRemove is null) return change;
                {
                    Type edgeType = typeof(Edge);

                    foreach (DSChoiceSaveData choiceData in from element in change.elementsToRemove
                                                            where element.GetType() != edgeType
                                                            select (Edge)element
                                                            into edge
                                                            select (DSChoiceSaveData)edge.output.userData)
                        choiceData.NodeID = string.Empty;
                }

                return change;
            };
        }

        private void OnGroupElementsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (element is not DSNode node) continue;

                    DSGroup dsGroup = (DSGroup)group;

                    RemoveUngroupedNode(node);
                    AddGroupedNode(node, dsGroup);
                }
            };
        }

        private void OnGroupElementsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (element is not DSNode node) continue;

                    RemoveGroupedNode(group, node);
                    AddUngroupedNode(node);
                }
            };
        }

        private void OnGroupRenamed()
        {
            groupTitleChanged = (group, s) =>
            {
                DSGroup dsGroup = (DSGroup)group;

                dsGroup.title = s.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(dsGroup.title))
                {
                    if (!string.IsNullOrEmpty(dsGroup.oldTitle)) ++NameErrorAmount;
                }
                else
                {
                    if (string.IsNullOrEmpty(dsGroup.oldTitle)) --NameErrorAmount;
                }

                RemoveGroup(dsGroup);

                dsGroup.oldTitle = dsGroup.title;

                AddGroup(dsGroup);
            };
        }

        private void RemoveGroup(DSGroup dsGroup)
        {
            string groupName = dsGroup.oldTitle.ToLower();

            List<DSGroup> groups = _groups[groupName].Groups;

            groups.Remove(dsGroup);
            dsGroup.ResetStyle();

            switch (groups.Count)
            {
                case 1:
                    NameErrorAmount--;
                    groups[0].ResetStyle();
                    return;
                case 0:
                    _groups.Remove(groupName);
                    break;
            }
        }

        #endregion

        #region Public Methods

        public void AddGroupedNode(DSNode textNode, DSGroup group)
        {
            string nodeName = textNode.DialogueName.ToLower();
            textNode.Group = group;

            if (!_groupedNodes.ContainsKey(group))
                _groupedNodes.Add(group, new SerializedDictionary<string, DSNodeErrorData>());

            if (!_groupedNodes[group].ContainsKey(nodeName))
            {
                DSNodeErrorData errorData = new();

                errorData.Nodes.Add(textNode);

                _groupedNodes[group].Add(nodeName, errorData);

                return;
            }

            List<DSNode> groupedNodes = _groupedNodes[group][nodeName].Nodes;

            groupedNodes.Add(textNode);

            Color errorColor = _groupedNodes[group][nodeName].ErrorData.Color;

            textNode.SetErrorStyle(errorColor);

            if (groupedNodes.Count != 2) return;
            NameErrorAmount++;
            groupedNodes[0].SetErrorStyle(errorColor);

        }

        public void AddUngroupedNode(DSNode textNode)
        {
            string nodeName = textNode.DialogueName.ToLower();

            if (!_ungroupedNodes.ContainsKey(nodeName))
            {
                DSNodeErrorData nodeErrorData = new();

                nodeErrorData.Nodes.Add(textNode);

                _ungroupedNodes.Add(nodeName, nodeErrorData);
                return;
            }

            List<DSNode> ungroupedNodes = _ungroupedNodes[nodeName].Nodes;

            ungroupedNodes.Add(textNode);

            Color errorColor = _ungroupedNodes[nodeName].ErrorData.Color;

            textNode.SetErrorStyle(errorColor);

            if (ungroupedNodes.Count != 2) return;
            NameErrorAmount++;
            ungroupedNodes[0].SetErrorStyle(errorColor);
        }

        public void ClearGraph()
        {
            graphElements.ForEach(RemoveElement);

            _groups.Clear();
            _ungroupedNodes.Clear();
            _groupedNodes.Clear();

            NameErrorAmount = 0;
        }

        public DSGroup CreateGroup(string title, Vector2 position)
        {
            DSGroup group = new(title, position);

            AddGroup(group);

            AddElement(group);

            foreach (GraphElement selectable in selection.Cast<GraphElement>())
            {
                if (selectable is not DSNode node) continue;

                group.AddElement(node);
            }

            return group;
        }

        public DSNode CreateNode(string nodeName, DSDialogueType dialogueType, Vector2 position, bool shouldDraw = true)
        {
            DSNode node = dialogueType switch
            {
                DSDialogueType.Single   => new DSTextSingleChoiceNode(),
                DSDialogueType.Multiple => new DSTextMultipleChoiceNode(),
                DSDialogueType.Action   => new DSActionNode(),
                _                       => throw new ArgumentOutOfRangeException(nameof(dialogueType), dialogueType, null)
            };

            node.Initialize(nodeName, this, position);

            if (shouldDraw) node.Draw();

            AddUngroupedNode(node);

            return node;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new();

            ports.ForEach(
                port =>
                {
                    if (startPort == port) return;

                    if (startPort.node == port.node) return;

                    if (startPort.direction == port.direction) return;

                    compatiblePorts.Add(port);

                });

            return compatiblePorts;
        }

        public Vector2 GetLocalMousePosition(Vector2 position, bool isSearchWindow = false, SearchWindowContext context = default)
        {

            Vector2 worldMousePosition = position;

            if (isSearchWindow)
            {
                worldMousePosition = _editorWindow.rootVisualElement.ChangeCoordinatesTo(
                    _editorWindow.rootVisualElement.parent,
                    context.screenMousePosition - _editorWindow.position.position);
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }

        public void RemoveGroupedNode(Group group, DSNode textNode)
        {
            string nodeName = textNode.DialogueName.ToLower();
            textNode.Group = null;

            List<DSNode> groupedNodes = _groupedNodes[group][nodeName].Nodes;

            groupedNodes.Remove(textNode);

            textNode.ResetColor();

            switch (groupedNodes.Count)
            {
                case 1:
                    NameErrorAmount--;
                    groupedNodes[0].ResetColor();
                    return;
                case 0:
                {
                    _groupedNodes[group].Remove(nodeName);

                    if (_groupedNodes[group].Count == 0) _groupedNodes.Remove(group);
                    break;
                }
            }

        }

        public void RemoveUngroupedNode(DSNode textNode)
        {
            string nodeName = textNode.DialogueName.ToLower();

            List<DSNode> ungroupedNodes = _ungroupedNodes[nodeName].Nodes;

            ungroupedNodes.Remove(textNode);
            textNode.ResetColor();

            switch (ungroupedNodes.Count)
            {
                case 1:
                    NameErrorAmount--;
                    ungroupedNodes[0].ResetColor();
                    return;
                case 0:
                    _ungroupedNodes.Remove(nodeName);
                    break;
            }

        }

        public void ToggleMiniMap()
        {
            _miniMap.visible = !_miniMap.visible;
        }

        #endregion

    }

}
