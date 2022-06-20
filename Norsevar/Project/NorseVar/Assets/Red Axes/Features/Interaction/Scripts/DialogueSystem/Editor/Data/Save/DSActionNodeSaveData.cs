using System;
using UnityEngine;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    [Serializable]
    public class DSActionNodeSaveData : DSNodeSaveData
    {

        #region Serialized Fields

        [SerializeField] private EAction action;

        #endregion

        #region Properties

        public EAction Action
        {
            get => action;
            set => action = value;
        }

        #endregion

    }

}
