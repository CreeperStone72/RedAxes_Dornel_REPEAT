using System.Collections;
using UnityEngine;

namespace Norsevar.Combat
{
    public class DashState : IdleMoveState
    {

        #region Constants and Statics

        private static readonly int DashProperty = Animator.StringToHash("Dash");
        private static readonly int DashSpeed = Animator.StringToHash("DashSpeed");

        #endregion

        #region Private Fields

        private PlayerMovementData _movementData;
        private bool _isDashing;
        private bool _doDashStrike;
        private bool _canTransition;

        #endregion

        #region Constructors

        public DashState(PlayerController playerController, PlayerMovementData movementData, float timeoutTime, bool needsExitTime = false) 
            : base(playerController, timeoutTime, needsExitTime)
        {
            PlayerInputs playerInputs = PlayerInputs.Instance;
            playerInputs.AddSpecialPerformed(() => AttemptStateChange(PlayerStateType.SpecialAttack), "DashState_SpecialAttack");
            playerInputs.AddAttack(() => AttemptStateChange(PlayerStateType.BasicAttack), "DashState_BasicAttack");
            playerInputs.AddAttack(ActivateDashStrike, "DashState_DashStrike");
        }

        #endregion

        #region State Machine Methods

        public override void OnEnter()
        {
            CanApplyDirection = false;
            _isDashing = false;
            _doDashStrike = false;
            _canTransition = false;

            PlayerMovement.UpdateDirection();
            PlayerController.Animator.SetBool(DashProperty, true);
            
            base.OnEnter();
        }

        public override void OnLogic()
        {
            if (CanApplyDirection)
                PlayerMovement.UpdateDirection(true);
        }

        #endregion

        #region Private Methods

        private void ActivateDashStrike()
        {
            if (fsm.ActiveState == this)
                _doDashStrike = true;
        }

        private void AttemptStateChange(PlayerStateType type)
        {
            if (fsm.ActiveState == this && _canTransition && !_doDashStrike)
                fsm.RequestStateChange(type);
        }

        private IEnumerator DashCoroutine(float time, float speed)
        {
            Animator.SetBool(DashProperty, true);
            float startTime = Time.time;

            while (Time.time < startTime + time)
            {
                CharacterController.Move(PlayerController.transform.forward * speed * Time.deltaTime);
                yield return null;
            }

            Animator.SetBool(DashProperty, false);
            Animator.SetFloat(DashSpeed, 1f);
            _isDashing = false;

            if (fsm.ActiveState == this)
                fsm.RequestStateChange(PlayerStateType.Idle);
        }

        private void DashEnd()
        {
            Animator.SetFloat(DashSpeed, .8f);
            //CanApplyDirection = true;
            _canTransition = true;

            if (_doDashStrike)
                fsm.RequestStateChange(PlayerStateType.DashAttack);
        }

        private void DashStart()
        {
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Player_Movement_Dash, CharacterController.transform.position);
            PlayerController.StartCoroutine(DashCoroutine(PlayerMovement.MovementData.DashTime, PlayerMovement.MovementData.DashSpeed));
            _isDashing = true;
        }

        #endregion

        #region Public Methods

        public override void HandleAnimationEvent(EAnimationEventType eventType)
        {
            switch (eventType)
            {
                case EAnimationEventType.DashStart:
                    DashStart();
                    break;
                case EAnimationEventType.DashEnd:
                    DashEnd();
                    break;
            }
        }

        public override void OnAnimatorMove()
        {
            if (!_isDashing)
                base.OnAnimatorMove();
        }

        #endregion

    }
}
