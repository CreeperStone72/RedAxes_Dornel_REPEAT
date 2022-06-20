using Norsevar.Interaction.DialogueSystem;
using UnityEngine;

namespace Norsevar.Interaction
{

    public class DialogueBehaviour : MonoBehaviour
    {

        #region Serialized Fields

        [SerializeField] private DSDialogueContainerSo container;

        [SerializeField] private DialogueEvent dialogueEvent;
        [SerializeField] private GameEvent pauseEvent;

        #endregion

        #region Public Methods

        public void SendDialogueEvent()
        {
            dialogueEvent.Raise(new MerchantInfo(container));
            pauseEvent.Raise();
        }

        #endregion

    }

}
