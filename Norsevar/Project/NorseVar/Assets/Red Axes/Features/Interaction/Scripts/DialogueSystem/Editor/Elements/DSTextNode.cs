using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    public class DSTextNode : DSNode
    {

        #region Properties

        public string Text { get; set; }

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

            Foldout textFoldout = DSElementUtility.CreateFoldout("Dialogue Text");

            TextField textField = DSElementUtility.CreateTextArea(
                Text,
                null,
                callback =>
                {
                    Text = callback.newValue;
                });
            textField.AddClasses("ds-node_textfield", "ds-node_quote-textfield");

            textFoldout.Add(textField);

            customDataContainer.Add(textFoldout);

            extensionContainer.Add(customDataContainer);

        }

        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);
            Text = "Dialogue Text";
        }

        #endregion

    }

}
