using Norsevar.Combat;
using UnityEngine;

namespace Norsevar.Status_Effect_System
{
    [CreateAssetMenu(fileName = "Damage Effect", menuName = "Norsevar/Status Effect System/Effects/Damage Effect")]
    public class DamageEffectData : BaseEffectData
    {

        #region Private Fields

        private float _currentIntervalCooldown;

        #endregion

        #region Serialized Fields

        [SerializeField] private EDamageType damageType;
        [SerializeField] [Min(0.1f)] private float applyDamageIntervalInSeconds;
        [SerializeField] [Min(1f)] private float damageToApply;

        #endregion

        #region Protected Methods

        protected override void DisableEffect()
        {
        }

        protected override void EnableEffect()
        {
            _currentIntervalCooldown = applyDamageIntervalInSeconds;
        }

        #endregion

        #region Public Methods

        public override void TickEffect()
        {
            if (_currentIntervalCooldown <= 0)
            {
                _currentIntervalCooldown = applyDamageIntervalInSeconds;
                effectable.EffectDamage(
                    new DamageInfo
                    {
                        DamageType = damageType,
                        DamageValue = damageToApply,
                        CanBeBlocked = false
                    });
            }

            _currentIntervalCooldown -= Time.deltaTime;

            base.TickEffect();
        }

        #endregion

    }
}
