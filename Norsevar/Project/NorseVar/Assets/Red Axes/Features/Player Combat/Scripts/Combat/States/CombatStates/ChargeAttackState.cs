namespace Norsevar.Combat
{
    public class ChargeAttackState : PlayerCombatState
    {

        #region Constructors

        public ChargeAttackState(PlayerController playerController, float timeoutTime, bool needsExitTime = false) : base(playerController, timeoutTime, needsExitTime)
        {
        }

        #endregion

        #region State Machine Methods

        public override void OnEnter()
        {
            PlayerCombat.EquippedWeapon.OnAttackEnd += OnAttackEnd;
            
            if(!PlayerCombat.EquippedWeapon.ChargeAttackChargeStart())
                OnAttackEnd();

            if (PlayerInputs.IsUsingGamepad)
                PlayerController.Movement.DirectionMarker.SetActive(true);
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayerCombat.EquippedWeapon.OnAttackEnd -= OnAttackEnd;
        }

        public override void OnLogic()
        {
            base.OnLogic();
            PlayerController.Movement.UpdateDirection(true);
            PlayerController.Movement.DirectionMarker.OnUpdateBasedOnForward();
        }

        #endregion

        #region Private Methods

        private void ChargeEnd()
        {
            if (fsm.ActiveState != this)
                return;

            PlayerCombat.EquippedWeapon.ChargeAttackChargeEnd();

            if (PlayerInputs.IsUsingGamepad)
                PlayerController.Movement.DirectionMarker.SetActive(false);
        }

        private void OnAttackEnd()
        {
            fsm.RequestStateChange(PlayerStateType.Idle);
        }

        #endregion

        #region Public Methods

        public override void OnDisable()
        {
            if (PlayerInputs.Instance)
                PlayerInputs.Instance.RemoveChargeCancel("ChargeAttackState_ChargeEnd");
        }

        public override void OnEnable()
        {
            PlayerInputs.Instance.AddChargeCancel(ChargeEnd, "ChargeAttackState_ChargeEnd");
        }

        #endregion

    }
}
