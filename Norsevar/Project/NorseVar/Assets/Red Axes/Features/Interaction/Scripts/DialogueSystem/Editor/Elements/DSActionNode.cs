using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    public class DSActionNode : DSNode
    {

        #region Properties

        [field: SerializeField]
        public EAction Action { get; set; }

        #endregion

        #region Public Methods

        public override void Draw()
        {
            TextField dialogueNameTextField = DSElementUtility.CreateTextField(
                DialogueName,
                null,
                callback =>
                {
                    TextField target = (TextField)callback.target;

                    target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();

                    if (string.IsNullOrEmpty(target.value))
                    {
                        if (!string.IsNullOrEmpty(DialogueName)) ++graphView.NameErrorAmount;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(DialogueName)) --graphView.NameErrorAmount;
                    }

                    if (Group == null)
                    {
                        graphView.RemoveUngroupedNode(this);
                        DialogueName = callback.newValue;
                        graphView.AddUngroupedNode(this);
                        return;
                    }
                    DSGroup group = Group;
                    graphView.RemoveGroupedNode(Group, this);
                    DialogueName = callback.newValue;
                    graphView.AddGroupedNode(this, group);

                });

            dialogueNameTextField.AddClasses("ds-node_textfield", "ds-node_filename-textfield", "ds-node_textfield_hidden");

            titleContainer.Insert(0, dialogueNameTextField);

            Port inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputContainer.Add(inputPort);

            VisualElement customDataContainer = new();

            customDataContainer.AddToClassList("ds-node_custom-data-container");

            EnumField enumField = DSElementUtility.CreateEnumField(
                Action,
                callback =>
                {
                    Action = (EAction)callback.newValue;
                });

            customDataContainer.Add(enumField);

            extensionContainer.Add(customDataContainer);

            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(choice.Text);

                choicePort.userData = choice;

                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }

        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.Action;

            Action = EAction.Leave;
        }

        #endregion

    }

}
