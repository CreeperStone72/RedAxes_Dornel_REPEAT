namespace Norsevar.Combat
{
    public class PlayerCombatState : PlayerBaseState
    {

        #region Protected Fields

        protected readonly PlayerCombatBehaviour PlayerCombat;

        #endregion

        #region Constructors

        protected PlayerCombatState(PlayerController playerController, float timeoutTime, bool needsExitTime = false) : base(playerController, timeoutTime, needsExitTime)
        {
            PlayerCombat = playerController.Combat;
        }

        #endregion

        #region Private Methods

        private void SetWeaponTrailEmission(bool active)
        {
            if (PlayerController.TrailParticleSystem == null) return;
            var emission = PlayerController.TrailParticleSystem.emission;
            emission.enabled = active;
        }

        #endregion

        #region Public Methods

        public override void HandleAnimationEvent(EAnimationEventType eventType)
        {
            PlayerCombat.EquippedWeapon?.HandleAnimationEvent(eventType);

            switch (eventType)
            {
                case EAnimationEventType.AttackMoveForceStart:
                {
                    PlayerController.ForceReceiver.AddForce(
                        PlayerController.transform.forward,
                        PlayerCombat.CurrentAttack.Data.AttackForwardMoveForce);
                    break;
                }
                case EAnimationEventType.AttackTrailStart:
                {
                    PlayerController.FetchNewTrailParticleSystem();
                    SetWeaponTrailEmission(true);
                    break;
                }
                case EAnimationEventType.AttackTrailEnd:
                {
                    SetWeaponTrailEmission(false);
                    PlayerController.TrailParticleSystem.transform.SetParent(null, true);
                    break;
                }
            }
        }

        public override void OnExit()
        {
            SetWeaponTrailEmission(false);
            PlayerController.DiscardTrailParticleSystem();
            base.OnExit();
        }

        #endregion

    }
}
