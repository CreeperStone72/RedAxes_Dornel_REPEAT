using System.Collections;
using FSM;
using UnityEngine;

namespace Norsevar.Combat
{
    public class PlayerBaseState : StateBase<PlayerStateType>
    {
        #region Private Fields

        private readonly bool _hasTimeoutTime;
        private readonly WaitForSeconds _timoutTime;

        private Coroutine _timoutCoroutine;

        #endregion

        #region Protected Fields

        protected readonly PlayerController PlayerController;

        #endregion

        #region Constructors

        protected PlayerBaseState(PlayerController playerController, float timeoutTime, bool needsExitTime) : base(
            needsExitTime)
        {
            PlayerController = playerController;
            _hasTimeoutTime = timeoutTime > 0;
            _timoutTime = new WaitForSeconds(timeoutTime);
        }

        #endregion

        #region State Machine Methods

        public override void OnEnter()
        {
            StartTimeoutTimer();
        }

        public override void OnExit()
        {
            StopTimeoutTimer();
        }

        #endregion

        #region Public Methods

        public virtual void HandleAnimationEvent(EAnimationEventType eventType)
        {
        }

        public virtual void OnAnimatorMove()
        {
        }

        public virtual void OnDisable()
        {
        }

        public virtual void OnEnable()
        {
        }

        #endregion

        #region Private Methods

        private IEnumerator StateTimeoutTimer()
        {
            yield return _timoutTime;

            _timoutCoroutine = null;
            fsm.RequestStateChange(PlayerStateType.Idle);
        }

        #endregion

        #region Protected Methods

        protected void StartTimeoutTimer()
        {
            if (_hasTimeoutTime)
                _timoutCoroutine = PlayerController.StartCoroutine(StateTimeoutTimer());
        }
        
        protected void StopTimeoutTimer()
        {
            if (_timoutCoroutine != null)
                PlayerController.StopCoroutine(_timoutCoroutine);
        }

        #endregion
    }
}