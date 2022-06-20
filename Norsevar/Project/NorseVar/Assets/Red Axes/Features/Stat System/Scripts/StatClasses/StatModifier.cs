using System;
using UnityEngine;

namespace Norsevar.Stat_System
{
    [Serializable]
    public class StatModifier
    {

        #region Serialized Fields

        [SerializeField] private float value;
        [SerializeField] private EModifierType type;

        #endregion

        #region Constructors

        public StatModifier()
        {
        }

        public StatModifier(float value, EModifierType type, object source = null)
        {
            this.value = value;
            this.type = type;
            Source = source;
        }

        #endregion

        #region Properties

        public object Source { get; set; }
        public float Value => value;
        public EModifierType Type => type;

        #endregion

    }
}
