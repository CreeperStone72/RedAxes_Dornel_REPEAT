using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    public class DSSearchWindow : ScriptableObject, ISearchWindowProvider
    {

        #region Private Fields

        private DSGraphView _graphView;
        private Texture2D _indentationIcon;

        #endregion

        #region Public Methods

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new()
            {
                new SearchTreeGroupEntry(new GUIContent("Create Element")),
                new SearchTreeGroupEntry(new GUIContent("Node"), 1),
                new SearchTreeEntry(new GUIContent("Single Choice", _indentationIcon))
                {
                    level = 2,
                    userData = DSDialogueType.Single
                },
                new SearchTreeEntry(new GUIContent("Multiple Choice", _indentationIcon))
                {
                    level = 2,
                    userData = DSDialogueType.Multiple
                },
                new SearchTreeEntry(new GUIContent("Action Node", _indentationIcon))
                {
                    level = 2,
                    userData = DSDialogueType.Action
                },
                new SearchTreeGroupEntry(new GUIContent("Dialogue Group"), 1),
                new SearchTreeEntry(new GUIContent("Single Group", _indentationIcon))
                {
                    level = 2,
                    userData = new Group()
                }
            };

            return searchTreeEntries;
        }

        public void Initialize(DSGraphView dsGraphView)
        {
            _graphView = dsGraphView;
            _indentationIcon = new Texture2D(1, 1);
            _indentationIcon.SetPixel(0, 0, Color.clear);
            _indentationIcon.Apply();
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            Vector2 localMousePosition = _graphView.GetLocalMousePosition(context.screenMousePosition, true, context);
            switch (searchTreeEntry.userData)
            {
                case DSDialogueType.Multiple:
                {
                    DSTextMultipleChoiceNode textMultipleChoiceNode = (DSTextMultipleChoiceNode)_graphView.CreateNode(
                        "DialogueName",
                        DSDialogueType.Multiple,
                        localMousePosition);
                    _graphView.AddElement(textMultipleChoiceNode);
                    return true;
                }
                case DSDialogueType.Single:
                {
                    DSTextSingleChoiceNode textSingleChoiceNode = (DSTextSingleChoiceNode)_graphView.CreateNode(
                        "DialogueName",
                        DSDialogueType.Single,
                        localMousePosition);
                    _graphView.AddElement(textSingleChoiceNode);
                    return true;
                }
                case DSDialogueType.Action:
                {
                    DSActionNode actionNode = (DSActionNode)_graphView.CreateNode("ActionNode", DSDialogueType.Action, localMousePosition);
                    _graphView.AddElement(actionNode);
                    return true;
                }
                case Group:
                {
                    _graphView.CreateGroup("Dialogue Group", localMousePosition);
                    return true;
                }
                default: return false;
            }
        }

        #endregion

    }

}
