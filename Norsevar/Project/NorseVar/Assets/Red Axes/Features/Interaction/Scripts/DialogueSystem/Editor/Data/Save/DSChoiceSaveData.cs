using System;
using UnityEngine;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    [Serializable]
    public class DSChoiceSaveData
    {

        #region Serialized Fields

        [SerializeField] private string text;

        [SerializeField] private string nodeID;

        #endregion

        #region Properties

        public string Text
        {
            get => text;
            set => text = value;
        }

        public string NodeID
        {
            get => nodeID;
            set => nodeID = value;
        }

        #endregion

    }

}
