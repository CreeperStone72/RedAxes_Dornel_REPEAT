using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.AI
{

    [Serializable]
    public class AttackData : ApproachData
    {

        #region Serialized Fields

        [SerializeField] [Tooltip("Min time an enemy waits before he attacks.")] [Range(1, 10)] [BoxGroup("Behaviour")]
        private float minWait;

        [SerializeField] [Tooltip("Max time an enemy waits before he attacks.")] [Range(10, 30)] [BoxGroup("Behaviour")]
        private float maxWait;

        #endregion

        #region Properties

        public float MinWait => minWait;

        public float MaxWait => maxWait;

        #endregion

    }

}
