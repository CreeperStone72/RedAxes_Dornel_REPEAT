using Norsevar.Stat_System;
using UnityEngine;

namespace Norsevar.Combat
{
    public class DefenseAttackState : PlayerCombatState
    {

        #region Constants and Statics

        private static readonly int Velocity = Animator.StringToHash("Velocity");
        private static readonly int DefensiveAttack = Animator.StringToHash("DefensiveAttack");

        #endregion

        #region Private Fields

        private readonly Stat _movementSpeedStat;

        #endregion

        #region Constructors

        public DefenseAttackState(PlayerController playerController, float timeoutTime, bool needsExitTime = false) : base(playerController, timeoutTime, needsExitTime)
        {
            _movementSpeedStat = PlayerController[EStatType.MovementSpeedMultiplier];
        }

        #endregion

        #region State Machine Methods

        public override void OnEnter()
        {
            base.OnEnter();
            PlayerController.Animator.SetBool(DefensiveAttack, true);
            PlayerController.PlayerHealth.IsBlocking = true;
            PlayerController.PlayerHealth.OnBlock += OnDamageBlocked;

            //When transitioning to this state while moving, add a slight forward force to make the transition look smoother
            if (PlayerController.Animator.GetFloat(Velocity) > 0.1)
            {
                PlayerController.ForceReceiver.AddForce(
                    PlayerController.transform.forward,
                    Mathf.Lerp(10f, 20f, _movementSpeedStat.Value - 1));
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayerController.Animator.SetBool(DefensiveAttack, false);
            PlayerController.PlayerHealth.IsBlocking = false;
            PlayerController.PlayerHealth.OnBlock -= OnDamageBlocked;
        }

        public override void OnLogic()
        {
            PlayerController.Movement.UpdateDirection(true);
        }

        #endregion

        #region Private Methods

        private void Dash()
        {
            if (fsm.ActiveState == this)
                fsm.RequestStateChange(PlayerStateType.Dash);
        }

        private void EndDefensive()
        {
            if (fsm.ActiveState == this)
                fsm.RequestStateChange(PlayerStateType.Idle);
        }

        private void OnDamageBlocked(DamageInfo damageInfo)
        {

        }

        #endregion

        #region Public Methods

        public override void OnDisable()
        {
            PlayerInputs.Instance.RemoveDefensiveCancel("DefensiveAttackState");
            PlayerInputs.Instance.RemoveDash("DefenseAttackState_Dash");
        }

        public override void OnEnable()
        {
            PlayerInputs.Instance.AddDefensiveCancel(EndDefensive, "DefensiveAttackState");
            PlayerInputs.Instance.AddDash(Dash, "DefenseAttackState_Dash");
        }

        #endregion

    }
}
