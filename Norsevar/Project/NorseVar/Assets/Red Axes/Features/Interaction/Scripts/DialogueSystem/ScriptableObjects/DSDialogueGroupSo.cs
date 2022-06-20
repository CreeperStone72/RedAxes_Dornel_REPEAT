using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.Interaction.DialogueSystem
{

    public class DSDialogueGroupSo : SerializedScriptableObject
    {

        #region Serialized Fields

        [SerializeField] private string groupName;

        #endregion

        #region Properties

        public string GroupName => groupName;

        #endregion

        #region Public Methods

        public void Initialize(string pGroupName)
        {
            groupName = pGroupName;
        }

        #endregion

    }

}
