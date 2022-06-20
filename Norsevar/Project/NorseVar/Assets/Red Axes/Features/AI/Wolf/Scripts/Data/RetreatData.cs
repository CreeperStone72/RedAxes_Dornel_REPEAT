using System;
using UnityEngine;

namespace Norsevar.AI
{

    [Serializable]
    public class RetreatData : BaseData
    {

        #region Serialized Fields

        [SerializeField] [Range(1, 30)] private float distance;

        #endregion

        #region Properties

        public float Distance => distance;

        #endregion

    }

}
