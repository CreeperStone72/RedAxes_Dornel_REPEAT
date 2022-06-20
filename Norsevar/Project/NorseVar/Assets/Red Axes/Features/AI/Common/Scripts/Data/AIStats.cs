using System;
using UnityEngine;

namespace Norsevar.AI
{

    [Serializable]
    public class AIStats
    {

        #region Serialized Fields

        [SerializeField] [Range(0, 100)] private int strength;

        [SerializeField] [Range(0, 100)] private int speed;

        [SerializeField] [Range(0, 100)] private int vitality;

        #endregion

        #region Properties

        public int Strength => strength;

        public int Speed => speed;

        public int Vitality => vitality;

        #endregion

    }

}
