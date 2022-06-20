using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    public class DSTextMultipleChoiceNode : DSTextNode
    {

        #region Private Methods

        private Port CreateChoicePort(object userData)
        {
            Port choicePort = this.CreatePort();

            choicePort.userData = userData;

            DSChoiceSaveData choiceData = (DSChoiceSaveData)userData;

            Button deleteChoiceButton = DSElementUtility.CreateButton(
                "X",
                () =>
                {
                    if (Choices.Count == 1) return;

                    if (choicePort.connected) graphView.DeleteElements(choicePort.connections);

                    Choices.Remove(choiceData);

                    graphView.RemoveElement(choicePort);

                });
            deleteChoiceButton.AddToClassList("ds-node_button");

            TextField choiceTextField = DSElementUtility.CreateTextField(
                choiceData.Text,
                null,
                callback =>
                {
                    choiceData.Text = callback.newValue;
                });
            choiceTextField.AddClasses("ds-node_textfield", "ds-node_choice-textfield", "ds-node_textfield_hidden");

            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);

            return choicePort;
        }

        #endregion

        #region Public Methods

        public override void Draw()
        {
            base.Draw();

            Button addChoiceButton = DSElementUtility.CreateButton(
                "Add Choice",
                () =>
                {

                    DSChoiceSaveData choiceSaveData = new()
                    {
                        Text = "Next Choice"
                    };

                    Choices.Add(choiceSaveData);

                    Port choicePort = CreateChoicePort(choiceSaveData);

                    outputContainer.Add(choicePort);
                });

            addChoiceButton.AddToClassList("ds-node_button");

            mainContainer.Insert(1, addChoiceButton);

            foreach (Port choicePort in from choice in Choices select CreateChoicePort(choice))
                outputContainer.Add(choicePort);
            RefreshExpandedState();
        }

        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.Multiple;

            DSChoiceSaveData choiceSaveData = new()
            {
                Text = "Next Choice"
            };

            Choices.Add(choiceSaveData);
        }

        #endregion

    }

}
