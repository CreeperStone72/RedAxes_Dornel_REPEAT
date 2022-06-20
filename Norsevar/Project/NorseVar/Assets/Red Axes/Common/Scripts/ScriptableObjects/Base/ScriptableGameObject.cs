using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar
{

    public class ScriptableGameObject : SerializedScriptableObject
    {

        #region Serialized Fields

        [SerializeField] [LabelWidth(50)] [BoxGroup("General")] [ContextMenuItem("Reset Name", "ResetName")]
        private new string name;

        [TextArea(4, 14)] [SerializeField] [BoxGroup("General")] [ContextMenuItem("Reset Description", "ResetDescription")]
        private string description;

        #endregion

        #region Properties

        public string Name
        {
            get => name;
            set => name = value;
        }

        public string Description => description;

        #endregion

        #region Public Methods

        public void ResetDescription()
        {
            description = "";
        }

        public void ResetName()
        {
            name = "";
        }

        #endregion

    }

}
