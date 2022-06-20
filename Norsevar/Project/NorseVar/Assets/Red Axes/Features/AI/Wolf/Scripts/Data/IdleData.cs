using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.AI
{

    [Serializable]
    public class IdleData : BaseData
    {

        #region Serialized Fields

        [SerializeField] [Tooltip("Min time an enemy can wait at a position.")] [Range(1, 10)] [BoxGroup("Behaviour")]
        private float minWait;

        [SerializeField] [Tooltip("Max time an enemy can wait at a position.")] [Range(2, 20)] [BoxGroup("Behaviour")]
        private float maxWait;

        [Space(5)] [SerializeField] [Tooltip("How far he will move from his starting position")] [Range(1, 20)]
        private float radiusWander;

        #endregion

        #region Properties

        public float MinWait => minWait;

        public float MaxWait => maxWait;

        public float RadiusWander => radiusWander;

        #endregion

    }

}
