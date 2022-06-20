using Norsevar.Combat;
using Norsevar.Stat_System;

namespace Norsevar.Status_Effect_System
{
    public interface IEffectable
    {

        #region Public Methods

        void EffectApplyStatModifier(EStatType stat, StatModifier modifier);
        void EffectDamage(DamageInfo           damageInfo);
        void EffectRemoveStatModifiers(object  source);

        #endregion

    }
}
