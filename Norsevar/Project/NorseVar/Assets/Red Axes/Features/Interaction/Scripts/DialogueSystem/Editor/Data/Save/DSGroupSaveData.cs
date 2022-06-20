using System;
using UnityEngine;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    [Serializable]
    public class DSGroupSaveData
    {

        #region Serialized Fields

        [SerializeField] private string id;

        [SerializeField] private string name;

        [SerializeField] private Vector2 position;

        #endregion

        #region Properties

        public string Id
        {
            get => id;
            set => id = value;
        }

        public string Name
        {
            get => name;
            set => name = value;
        }

        public Vector2 Position
        {
            get => position;
            set => position = value;
        }

        #endregion

    }

}
