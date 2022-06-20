using System.Collections.Generic;
using Norsevar.Combat;
using Norsevar.Currencies;
using Norsevar.Stat_System;
using Unity.Mathematics;
using UnityEngine;

namespace Norsevar.AI
{

    [RequireComponent(typeof(AIDataManager))]
    public class HealthBehaviour : HealthBase
    {

        #region Private Fields

        private Resource _armor;

        private AnimationBehaviour _animationBehaviour;
        private EnemyEvent _onEnemyDeath;
        private bool _isWolf;

        #endregion

        #region Unity Methods

        protected virtual void Start()
        {
            _animationBehaviour = GetComponentInChildren<AnimationBehaviour>();

            AIDataManager aiDataManager = GetComponent<AIDataManager>();
            _onEnemyDeath = aiDataManager.OnDeath;
            _isWolf = aiDataManager.IsWolf();
        }

        #endregion

        #region Private Methods

        private void DamageOverflow(float damageValue)
        {
            if (!(_armor.CurrentValue - damageValue < 0)) return;
            HealthResource.ApplyModifier(new StatModifier(-math.abs(_armor.PreviousValue - damageValue), EModifierType.Additive));
        }

        #endregion

        #region Protected Methods

        protected virtual bool IsDead()
        {
            if (!IsEntityDeath()) return false;


            _onEnemyDeath.Raise(new EnemyData { currencyType = CurrencyType.Permanent, amount = 50, position = transform.position });
            _animationBehaviour.PlayDeath(
                () =>
                {
                    Analytics.AddEnemyKilled();
                    Destroy(gameObject);
                });

            NorseGame.Instance.RaiseEvent(
                _isWolf ? ENorseGameEvent.Enemies_Wolf_Death : ENorseGameEvent.Enemies_Snake_Death,
                transform.position);
            return true;
        }

        #endregion

        #region Public Methods

        public override float ReceiveDamage(DamageInfo damageInfo)
        {
            _animationBehaviour.PlayDamaged();

            NorseGame.Instance.RaiseEvent(
                _isWolf ? ENorseGameEvent.Enemies_Wolf_Hurt : ENorseGameEvent.Enemies_Snake_Hurt,
                transform.position);

            EffectController.ApplyStatusEffect(damageInfo.EffectsToApply, damageInfo.SourceGameObject);

            float damage = GetDamageValue(damageInfo);

            if (_armor.CurrentValue > 0)
            {
                _armor.ApplyModifier(new StatModifier(-damage, EModifierType.Additive));
                DamageOverflow(damage);
            }

            else
            {
                HealthResource.ApplyModifier(new StatModifier(-damage, EModifierType.Additive));
                IsDead();
            }

            return damage;
        }

        public override void SetStats(IReadOnlyDictionary<EStatType, Stat> dictionary)
        {
            base.SetStats(dictionary);
            _armor = dictionary[EStatType.Armor] as Resource;
        }

        #endregion

    }

}
