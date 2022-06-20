using System;
using UnityEngine;

namespace Norsevar.Combat
{
    public class CombatEntityControllerPlayer : CombatEntityController
    {
        #region Private Fields

        private Health _playerHealth;

        #endregion

        #region Public Properties

        public Health PlayerHealth => _playerHealth;

        #endregion

        #region Protected Methods

        protected override void Init()
        {
            base.Init();
            _playerHealth = healthBehaviour as Health;
            if (_playerHealth == null)
                return;

            Health.OnDeath += OnDie;
            Health.OnHit += OnHit;
        }

        private void OnDestroy()
        {
            Health.OnDeath -= OnDie;
            Health.OnHit -= OnHit;
        }

        #endregion

    }
}
