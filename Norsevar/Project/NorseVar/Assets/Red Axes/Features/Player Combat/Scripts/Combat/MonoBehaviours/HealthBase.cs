using System;
using System.Collections.Generic;
using Norsevar.AI;
using Norsevar.Stat_System;
using Norsevar.Status_Effect_System;
using UnityEngine;

namespace Norsevar.Combat
{
    public abstract class HealthBase : MonoBehaviour, IDamageable
    {

        public event Action OnInit;

        #region Private Fields

        private Stat _resistance;

        #endregion

        #region Protected Fields

        protected Resource Health;

        #endregion

        #region Serialized Fields

        [SerializeField] protected StatusEffectController statusEffectController;

        #endregion

        #region Properties

        protected StatusEffectController EffectController => statusEffectController;
        public Resource HealthResource => Health;

        #endregion

        #region Protected Methods

        protected float GetDamageValue(DamageInfo damageInfo)
        {
            return damageInfo.DamageType switch
            {
                EDamageType.Physical => damageInfo.DamageValue - damageInfo.DamageValue * _resistance.Value,
                EDamageType.Poison   => damageInfo.DamageValue,
                _                    => 0
            };
        }

        protected bool IsDamageInfo(DamageInfo damageInfo)
        {
            if (damageInfo != null)
                return false;

            Debug.LogWarning("There was no damage info send, pls make sure you send some or the entity wont get damaged.", this);
            return true;
        }

        protected bool IsEntityDeath()
        {
            return HealthResource.CurrentValue <= 0;
        }

        #endregion

        #region Public Methods

        public abstract float ReceiveDamage(DamageInfo damageInfo);
        public virtual void ReceiveHeal(float amount) { }

        public void ReceiveStatusEffect(BaseEffectData effect, GameObject source)
        {
            statusEffectController.ApplyStatusEffect(effect, source);
        }

        public virtual void SetStats(IReadOnlyDictionary<EStatType, Stat> dictionary)
        {
            _resistance = dictionary[EStatType.Resistance];
            Health = dictionary[EStatType.Health] as Resource;
            OnInit?.Invoke();
        }

        #endregion

    }
}
