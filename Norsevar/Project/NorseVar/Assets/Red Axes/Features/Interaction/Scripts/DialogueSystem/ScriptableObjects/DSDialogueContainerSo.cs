using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Norsevar.Interaction.DialogueSystem
{

    public class DSDialogueContainerSo : SerializedScriptableObject
    {

        #region Serialized Fields

        [SerializeField] [HorizontalGroup("Split", MaxWidth = 200)] [BoxGroup("Split/General")] [LabelWidth(75)]
        private string fileName;

        [SerializeField] [EnumToggleButtons] [OnValueChanged("ChangeSelected")] [BoxGroup("Split/Settings")] [LabelWidth(75)] [HideLabel]
        private EDialogueType type;

        [SerializeField] [OnValueChanged("ChangeSelected")] [BoxGroup("Split/Settings")] [LabelWidth(75)]
        private bool startNode;

        [SerializeField] [HideInInspector]
        private SerializedDictionary<DSDialogueGroupSo, List<DSDialogueSo>> dialogueGroups;

        [SerializeField] [HideInInspector]
        private List<DSDialogueSo> ungroupedDialogues;

        [SerializeField] [ValueDropdown("GetPossibleGroups")] [ShowIf("IsGrouped")] [BoxGroup("Selected Dialogue")] [LabelText("Group")]
        [LabelWidth(75)]
        private DSDialogueGroupSo selectedGroup;

        [SerializeField] [ValueDropdown("GetPossibleDialogue")] [BoxGroup("Selected Dialogue")] [HideLabel]
        private DSDialogueSo selectedDialogue;

        #endregion

        #region Properties

        public string FileName => fileName;

        public SerializedDictionary<DSDialogueGroupSo, List<DSDialogueSo>> DialogueGroups => dialogueGroups;

        public List<DSDialogueSo> UngroupedDialogues => ungroupedDialogues;

        public DSDialogueSo SelectedDialogue => selectedDialogue;

        #endregion

        #region Private Methods

        private void ChangeSelected()
        {
            selectedDialogue = null;
        }

        private List<DSDialogueSo> GetPossibleDialogue()
        {
            List<DSDialogueSo> dialogues = new();

            if (IsGrouped())
            {
                if (selectedGroup is null) return null;
                if (startNode)
                {
                    dialogues.AddRange(DialogueGroups[selectedGroup].Where(dialogue => dialogue.IsStartingDialogue));
                    return dialogues;
                }
                dialogues.AddRange(DialogueGroups[selectedGroup]);
                return dialogues;
            }

            if (startNode)
            {
                dialogues.AddRange(UngroupedDialogues.Where(dialogue => dialogue.IsStartingDialogue));
                return dialogues;
            }
            dialogues.AddRange(UngroupedDialogues);
            return dialogues;
        }

        private List<DSDialogueGroupSo> GetPossibleGroups()
        {
            return (from pair in DialogueGroups select pair.Key).ToList();
        }

        private bool IsGrouped()
        {
            return type == EDialogueType.Grouped;
        }

        #endregion

        #region Public Methods

        public void Initialize(string pFileName)
        {
            fileName = pFileName;

            ungroupedDialogues = new List<DSDialogueSo>();

            dialogueGroups = new SerializedDictionary<DSDialogueGroupSo, List<DSDialogueSo>>();
        }

        #endregion

    }

    public enum EDialogueType
    {
        Grouped,
        Ungrouped
    }

}
