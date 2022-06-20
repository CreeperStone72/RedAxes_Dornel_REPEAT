using Norsevar.Combat;
using Norsevar.Stat_System;

namespace Norsevar.UI
{
    public class PlayerBarListener : HealthBarListener
    {

        #region Unity Methods

        private void Start()
        {
            Resource health = NorseGame.Instance.Get<PlayerController>()[EStatType.Health] as Resource;
            MaxHealth = health.Value;
            CurrentHealth = health.CurrentValue;
        }

        protected override void OnEnable()
        {
            Health.OnDeath += () => { OnValueChanged(0); };
            Health.OnHit += OnValueChanged;
            Health.OnHeal += OnValueChanged;
            Health.OnMaxHealthChanged += OnMaxHealthChanged;
        }

        protected override void OnDisable()
        {
            Health.OnHit -= OnValueChanged;
            Health.OnHeal -= OnValueChanged;
            Health.OnMaxHealthChanged -= OnMaxHealthChanged;
        }

        #endregion

    }
}
