namespace Norsevar.Combat
{
    public class DashAttackState : PlayerCombatState
    {

        #region Constructors

        public DashAttackState(PlayerController playerController, float timeoutTime, bool needsExitTime = false) : base(playerController, timeoutTime, needsExitTime)
        {
        }

        #endregion
        
        #region State Machine Methods

        public override void OnEnter()
        {
            base.OnEnter();
            PlayerCombat.EquippedWeapon.PerformDashAttack();
            PlayerCombat.EquippedWeapon.OnAttackEnd += OnAttackEnd;
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayerCombat.EquippedWeapon.OnAttackEnd -= OnAttackEnd;
        }

        #endregion

        #region Private Methods

        private void OnAttackEnd()
        {
            fsm.RequestStateChange(PlayerStateType.Idle);
        }

        #endregion

    }
}
