using System;
using System.Collections.Generic;
using Norsevar.MusicAndSFX;
using Norsevar.Stat_System;
using Norsevar.Status_Effect_System;
using UnityEngine;

namespace Norsevar.Combat
{
    public class Health : HealthBase
    {

        #region Delegates and Events

        public event Action<DamageInfo> OnBlock;
        public static event Action OnDeath;
        public static event Action<float> OnHit;
        public static event Action<float> OnHeal;
        public static event Action<float, float> OnMaxHealthChanged; 

        #endregion

        #region Properties

        public bool IsBlocking { get; set; }

        #endregion

        #region Private Methods
        
        private void MaxHealthChanged(float oldValue, float newValue)
        {
            OnMaxHealthChanged?.Invoke(oldValue, newValue);
            ReceiveHeal(newValue - oldValue);
        }

        #endregion

        #region Public Methods

        public override void ReceiveHeal(float amount)
        {
            Health.ApplyModifier(new StatModifier(amount, EModifierType.Additive));
            OnHeal?.Invoke(Health.CurrentValue);
        }

        public override float ReceiveDamage(DamageInfo damageInfo)
        {
            if (IsDamageInfo(damageInfo) || IsEntityDeath())
                return 0;

            if (IsBlocking && damageInfo.CanBeBlocked)
            {
                //only block if the damage source is in front of the entity
                Vector3 damageSourcePos = damageInfo.SourceGameObject.transform.position;
                Vector3 playerPos = transform.position;
                damageSourcePos.y = playerPos.y;
                Vector3 dirToDamageSource = (damageSourcePos - playerPos).normalized;

                if (Vector3.Angle(transform.forward, dirToDamageSource) < 100f / 2f)
                {
                    print("Damage was blocked!");
                    OnBlock?.Invoke(damageInfo);
                    return 0;
                }
            }

            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Player_Hurt, gameObject);

            statusEffectController.ApplyStatusEffect(damageInfo.EffectsToApply, damageInfo.SourceGameObject);

            if (statusEffectController.IsEffectActive(EStatusEffectType.Invincible))
                return 0;

            float damage = GetDamageValue(damageInfo);

            Health.ApplyModifier(new StatModifier(-damage, EModifierType.Additive));

            FMODGlobalParameterChangeScript.SetCurrentHealth(Health.CurrentValue);

            if (IsEntityDeath())
            {
                Analytics.AddPlayerKilled();
                NorseGame.Instance.RaiseEvent(ENorseGameEvent.Player_Death, transform.position);
                OnDeath?.Invoke();
            }
            else
                OnHit?.Invoke(Health.CurrentValue);

            return damage;
        }

        public override void SetStats(IReadOnlyDictionary<EStatType, Stat> dictionary)
        {
            base.SetStats(dictionary);
            Health.OnValueChanged += MaxHealthChanged;
        }

        #endregion

    }
}
