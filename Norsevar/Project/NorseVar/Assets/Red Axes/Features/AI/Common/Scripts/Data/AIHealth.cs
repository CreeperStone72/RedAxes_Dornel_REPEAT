using System;
using UnityEngine;

namespace Norsevar.AI
{

    [Serializable]
    public class AIHealth
    {

        #region Serialized Fields

        [SerializeField] [Range(0, 100)] private float baseHealth;

        [SerializeField] [Range(0, 100)] private int baseArmor;

        #endregion

        #region Properties

        public float BaseHealth => baseHealth;

        public int BaseArmor => baseArmor;

        #endregion

    }

}
