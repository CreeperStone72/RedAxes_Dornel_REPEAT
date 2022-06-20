using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    public class DSTextSingleChoiceNode : DSTextNode
    {

        #region Public Methods

        public override void Draw()
        {
            base.Draw();

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
            DialogueType = DSDialogueType.Single;

            DSChoiceSaveData choiceSaveData = new()
            {
                Text = "Next Dialogue"
            };

            Choices.Add(choiceSaveData);
        }

        #endregion

    }

}
