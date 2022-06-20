using System.Collections.Generic;
using Norsevar.Stat_System;
using UnityEngine;

namespace Norsevar.Status_Effect_System
{
    [CreateAssetMenu(fileName = "Stat Effect", menuName = "Norsevar/Status Effect System/Effects/Stat Effect")]
    public class StatModifierEffectData : BaseEffectData
    {

        #region Protected Methods

        protected override void DisableEffect()
        {
            effectable.EffectRemoveStatModifiers(this);
        }

        protected override void EnableEffect()
        {
            foreach ((EStatType statType, StatModifier statModifier) in StatModifiers)
            {
                statModifier.Source = this;
                effectable.EffectApplyStatModifier(statType, statModifier);
            }
        }

        #endregion

        public Dictionary<EStatType, StatModifier> StatModifiers;
    }
}
