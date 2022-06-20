using UnityEngine;

namespace Norsevar.Combat
{
    public class UnconsciousState : PlayerBaseState
    {
        private static readonly int StartingUnconscious = Animator.StringToHash("StartingUnconscious");
        
        private bool _detectInput;
        private bool _isGettingUp;

        #region Constructor

        public UnconsciousState(PlayerController playerController, float timeoutTime, bool needsExitTime = false) 
            : base(playerController, timeoutTime, needsExitTime)
        {
        }

        #endregion

        public override void OnEnter()
        {
            base.OnEnter();
            PlayerController.Animator.SetBool(StartingUnconscious, true);
            PlayerController.ExecuteInSeconds(() => { _detectInput = true; }, 1f);
            PlayerController.Movement.DirectionMarker.SetActive(false);
        }

        public override void OnLogic()
        {
            base.OnLogic();

            if (_detectInput && !_isGettingUp)
            {
                Vector2 inputVector = PlayerInputs.Instance.GetMovement().ReadValue<Vector2>();

                if (inputVector != Vector2.zero)
                {
                    PlayerController.Animator.SetBool(StartingUnconscious, false);
                    _isGettingUp = true;
                }
            }
        }

        public override void HandleAnimationEvent(EAnimationEventType eventType)
        {
            base.HandleAnimationEvent(eventType);

            if (eventType == EAnimationEventType.AttackStart)
            {
                PlayerController.ExecuteInSeconds(() => { PlayerController.Movement.DirectionMarker.SetActive(true); }, 1f);
                fsm.RequestStateChange(PlayerStateType.Idle);
            }
        }
    }
}