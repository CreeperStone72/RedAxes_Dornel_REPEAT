using Norsevar.Stat_System;
using UnityEngine;

namespace Norsevar.Combat
{
    public class BasicAttackComboState : PlayerCombatState
    {

        #region Constants and Statics

        private static readonly int Velocity = Animator.StringToHash("Velocity");

        #endregion

        #region Private Fields

        private readonly Stat _movementSpeedStat;
        private bool _lockDirection;

        #endregion

        #region Constructors

        public BasicAttackComboState(PlayerController playerController, float timeoutTime, bool needsExitTime = false) : base(playerController, timeoutTime, needsExitTime)
        {
            PlayerInputs.Instance.AddAttack(Attack, "BasicAttackComboState_Attack");
            _movementSpeedStat = PlayerController[EStatType.MovementSpeedMultiplier];
        }

        #region State Machine Methods

        public override void OnEnter()
        {
            base.OnEnter();
            Attack();
            PlayerCombat.EquippedWeapon.OnAttackEnd += OnAttackEnd;

            //When transitioning to this state while moving, add a slight forward force to make the transition look smoother
            if (PlayerController.Animator.GetFloat(Velocity) > 0.1)
            {
                PlayerController.ForceReceiver.AddForce(
                    PlayerController.transform.forward,
                    Mathf.Lerp(20f, 30f, _movementSpeedStat.Value - 1));
            }

            _lockDirection = false;
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayerInputs.Instance.RemoveAttack("Attack");
            PlayerCombat.EquippedWeapon.OnAttackEnd -= OnAttackEnd;
        }

        public override void OnLogic()
        {
            if (!_lockDirection)
                PlayerController.Movement.UpdateDirection(true);
        }

        #endregion

        #endregion

        #region Private Methods

        private void Attack()
        {
            if (fsm.ActiveState == this)
                PlayerCombat.EquippedWeapon.Attack();
        }

        private void OnAttackEnd()
        {
            fsm.RequestStateChange(PlayerStateType.Idle);
        }

        #endregion

        #region Public Methods

        public override void HandleAnimationEvent(EAnimationEventType eventType)
        {
            base.HandleAnimationEvent(eventType);

            _lockDirection = eventType switch
            {
                EAnimationEventType.AttackMoveForceStart => true,
                EAnimationEventType.ComboDelayEnd        => false,
                _                                        => _lockDirection
            };
        }

        #endregion

    }
}
