using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.AI
{

    [Serializable] [InlineProperty] [HideLabel]
    public class BaseData
    {

        #region Serialized Fields

        [Tooltip("Speed is multiplied with this value for this state. On state change the speed is reset.")] [Range(0.01f, 10)]
        [SerializeField]
        private float speedMultiplier;

        #endregion

        #region Properties

        public float SpeedMultiplier => speedMultiplier;

        #endregion

    }

}
