using System;
using UnityEngine;

namespace Norsevar.Interaction.DialogueSystem
{

    [Serializable]
    public class DSDialogueChoiceData
    {

        #region Serialized Fields

        [SerializeField] private string text;

        [SerializeField] private DSDialogueSo nextDialogue;

        #endregion

        #region Properties

        public string Text
        {
            get => text;
            set => text = value;
        }

        public DSDialogueSo NextDialogue
        {
            get => nextDialogue;
            set => nextDialogue = value;
        }

        #endregion

    }

}
