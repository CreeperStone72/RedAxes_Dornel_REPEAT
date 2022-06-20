using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Norsevar.Stat_System
{
    public class Stat
    {

        #region Delegates and Events

        public event Action<float, float> OnValueChanged;

        #endregion

        #region Protected Fields

        protected readonly StatDefinition baseStatDefinition;
        protected float baseValue;
        protected float value;
        protected readonly List<StatModifier> modifiers;

        #endregion

        #region Constructors

        public Stat()
        {
            modifiers = new List<StatModifier>();
            Modifiers = modifiers.AsReadOnly();
        }

        public Stat(StatDefinition baseStatDefinition) : this()
        {
            this.baseStatDefinition = baseStatDefinition;
            BaseValue = baseStatDefinition.BaseValue;
        }

        #endregion

        #region Properties

        public float Value => value;

        public float BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
                CalculateFinalValue();
            }
        }

        public ReadOnlyCollection<StatModifier> Modifiers { get; }
        public StatDefinition BaseStatDefinition => baseStatDefinition;

        #endregion

        #region Protected Methods

        protected void CalculateFinalValue()
        {
            //Sort the modifer to establish an order of operations
            modifiers.Sort((x, y) => x.Type.CompareTo(y.Type));
            float newValue = BaseValue;
            float sumMultAdd = 0;

            //Apply all of the modifiers to the base value to get the final value
            for (int i = 0; i < modifiers.Count; i++)
            {
                StatModifier modifer = modifiers[i];
                switch (modifer.Type)
                {
                    case EModifierType.OverrideBase:
                        newValue = modifer.Value;
                        break;
                    case EModifierType.Additive:
                        newValue += modifer.Value;
                        break;
                    case EModifierType.AddMultiplicative:
                        sumMultAdd += modifer.Value;

                        if (i + 1 <= modifiers.Count || modifiers[i + 1].Type != EModifierType.AddMultiplicative)
                        {
                            newValue *= 1 + sumMultAdd;
                            sumMultAdd = 0;
                        }
                        break;
                    case EModifierType.Multiplicative:
                        newValue *= 1 + modifer.Value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            //Enforce the maximum value (if one is defined)
            if (BaseStatDefinition && BaseStatDefinition.Cap >= 0)
                newValue = Mathf.Min(newValue, BaseStatDefinition.Cap);

            //Round the final value
            float oldValue = value;
            value = (float)Math.Round(newValue, 3);

            //Callback if the value has changed
            if (Math.Abs(value - oldValue) > 0.0001f)
                OnValueChanged?.Invoke(oldValue, value);
        }

        #endregion

        #region Public Methods

        public void AddModifier(StatModifier modifier)
        {
            modifiers.Add(modifier);
            CalculateFinalValue();
        }

        public bool RemoveAllModifiersFromSource(object source)
        {
            if (source == null)
                return false;

            int numberRemoved = modifiers.RemoveAll(mod => mod.Source != null && mod.Source == source);
            if (numberRemoved > 0)
            {
                CalculateFinalValue();
                return true;
            }

            return false;
        }

        public bool RemoveModifier(StatModifier modifier)
        {
            bool removed = modifiers.Remove(modifier);
            if (removed) CalculateFinalValue();
            return removed;
        }

        #endregion

    }
}
