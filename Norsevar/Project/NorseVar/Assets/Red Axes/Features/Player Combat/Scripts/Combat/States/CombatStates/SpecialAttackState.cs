using Norsevar.Stat_System;
using UnityEngine;

namespace Norsevar.Combat
{
    public class SpecialAttackState : PlayerCombatState
    {

        #region Constants and Statics

        private static readonly int Velocity = Animator.StringToHash("Velocity");

        #endregion

        #region Private Fields

        private readonly Stat _movementSpeedStat;

        #endregion

        #region Constructors

        public SpecialAttackState(PlayerController playerController, float timeoutTime, bool needsExitTime = false) : base(playerController, timeoutTime, needsExitTime)
        {
            _movementSpeedStat = PlayerController[EStatType.MovementSpeedMultiplier];
        }

        #endregion
        
        #region State Machine Methods

        public override void OnEnter()
        {
            base.OnEnter();
            PlayerController.Movement.UpdateDirection(true);
            PlayerCombat.EquippedWeapon.PerformSpecialAttack();
            PlayerCombat.EquippedWeapon.OnAttackEnd += OnAttackEnd;

            //When transitioning to this state while moving, add a slight forward force to make the transition look smoother
            if (PlayerController.Animator.GetFloat(Velocity) > 0.1)
            {
                PlayerController.ForceReceiver.AddForce(
                    PlayerController.transform.forward,
                    Mathf.Lerp(20f, 30f, _movementSpeedStat.Value - 1));
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayerCombat.EquippedWeapon.OnAttackEnd -= OnAttackEnd;
        }

        public override void OnLogic()
        {
            PlayerController.Movement.UpdateDirection(true);
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
