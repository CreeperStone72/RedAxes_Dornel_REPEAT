using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.Interaction.DialogueSystem
{

    public class DSDialogueSo : SerializedScriptableObject
    {

        #region Serialized Fields

        [SerializeField] private string dialogueName;

        [SerializeField] [TableList] private List<DSDialogueChoiceData> choices;

        [SerializeField] private string text;

        [SerializeField] private DSDialogueType dialogueType;

        [SerializeField] private bool isStartingDialogue;
        [SerializeField] private EAction action;

        #endregion

        #region Properties

        public string DialogueName => dialogueName;

        public List<DSDialogueChoiceData> Choices => choices;

        public string Text => text;

        public EAction Action => action;

        public DSDialogueType DialogueType => dialogueType;

        public bool IsStartingDialogue => isStartingDialogue;

        #endregion

        #region Public Methods

        public DSDialogueSo GetNextDialogue(int pIndex = 0)
        {
            return Choices[pIndex].NextDialogue;
        }

        public void Initialize(
            string                     pDialogueName,
            string                     pText,
            List<DSDialogueChoiceData> pChoices,
            DSDialogueType             pDialogueType,
            bool                       pIsStartingDialogue)
        {
            dialogueName = pDialogueName;
            text = pText;
            choices = pChoices;
            dialogueType = pDialogueType;
            isStartingDialogue = pIsStartingDialogue;
            action = EAction.None;
        }

        public void Initialize(
            string                     pDialogueName,
            EAction                    pAction,
            List<DSDialogueChoiceData> pChoices,
            DSDialogueType             pDialogueType,
            bool                       pIsStartingDialogue)
        {
            dialogueName = pDialogueName;
            action = pAction;
            choices = pChoices;
            dialogueType = pDialogueType;
            isStartingDialogue = pIsStartingDialogue;
            text = string.Empty;
        }

        #endregion

    }

}
