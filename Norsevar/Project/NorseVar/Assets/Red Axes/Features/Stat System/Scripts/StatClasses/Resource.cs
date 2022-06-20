using System;
using UnityEngine;

namespace Norsevar.Stat_System
{
    public sealed class Resource : Stat
    {

        #region Delegates and Events

        public event Action<StatModifier> AppliedModifier;
        public event Action<float> OnCurrentValueChanged;

        #endregion

        #region Constructors

        public Resource(StatDefinition baseStatDefinition) : base(baseStatDefinition)
        {
            CurrentValue = Value;
            PreviousValue = Value;
        }

        #endregion

        #region Properties

        public float CurrentValue { get; private set; }
        public float PreviousValue { get; private set; }

        #endregion

        #region Public Methods

        public void ApplyModifier(StatModifier modifier)
        {
            PreviousValue = CurrentValue;
            float newValue = CurrentValue;

            switch (modifier.Type)
            {
                case EModifierType.Override:
                    newValue = modifier.Value;
                    break;
                case EModifierType.Additive:
                    newValue += modifier.Value;
                    break;
                case EModifierType.Multiplicative:
                    newValue *= modifier.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            newValue = Mathf.Clamp(newValue, 0, value);

            if (Math.Abs(CurrentValue - newValue) < .0001f)
                return;
            CurrentValue = newValue;
            OnCurrentValueChanged?.Invoke(CurrentValue);
            AppliedModifier?.Invoke(modifier);
        }

        #endregion

    }
}
