using System;
using System.Collections.Generic;
using UnityEngine;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    [Serializable]
    public abstract class DSNodeSaveData
    {

        #region Serialized Fields

        [SerializeField] private string name;
        [SerializeField] private List<DSChoiceSaveData> choices;
        [SerializeField] private string groupID;
        [SerializeField] private DSDialogueType dialogueType;
        [SerializeField] private Vector2 position;
        [SerializeField] private string id;

        #endregion

        #region Properties

        public string Name
        {
            get => name;
            set => name = value;
        }

        public List<DSChoiceSaveData> Choices
        {
            get => choices;
            set => choices = value;
        }

        public string GroupID
        {
            get => groupID;
            set => groupID = value;
        }

        public DSDialogueType DialogueType
        {
            get => dialogueType;
            set => dialogueType = value;
        }

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        public string Id
        {
            get => id;
            set => id = value;
        }

        #endregion

    }

}
