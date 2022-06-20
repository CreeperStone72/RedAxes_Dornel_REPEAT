using System;
using UnityEngine;

namespace Norsevar.AI
{

    [Serializable]
    public class ApproachData : BaseData
    {

        #region Serialized Fields

        [SerializeField] [Tooltip("Distance to target.")] [Range(1, 20)]
        private float distanceFromTarget;

        #endregion

        #region Properties

        public float DistanceFromTarget => distanceFromTarget;

        #endregion

    }

}
