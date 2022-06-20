using System;
using UnityEngine;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    [Serializable]
    public class DSTextNodeSaveData : DSNodeSaveData
    {

        #region Serialized Fields

        [SerializeField] private string text;

        #endregion

        #region Properties

        public string Text
        {
            get => text;
            set => text = value;
        }

        #endregion

    }

}
