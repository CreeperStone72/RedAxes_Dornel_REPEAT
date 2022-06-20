using UnityEngine;

namespace Norsevar.AI
{

    public class HitSnakeState : IState<ESnakeStates>
    {

        #region Private Fields

        private readonly SnakeStateBehaviour _snakeStateBehaviour;
        private Vector3 _dir;
        private ESnakeStates _previousState;

        #endregion

        #region Constructors

        public HitSnakeState(SnakeStateBehaviour pSnakeStateBehaviour)
        {
            _snakeStateBehaviour = pSnakeStateBehaviour;
        }

        #endregion

        #region Public Methods

        public ESnakeStates GetState()
        {
            return ESnakeStates.Hit;
        }

        public void OnEnter()
        {
            _snakeStateBehaviour.DisableAgent();
            _snakeStateBehaviour.DisableKinematic();
            _snakeStateBehaviour.AddForceAndWait(_dir);
        }

        public void OnExit()
        {
            _snakeStateBehaviour.EnableKinematic();
        }

        public void SetDirection(Vector3 pDirToTarget)
        {
            _dir = pDirToTarget;
        }

        public void SetPreviousState(ESnakeStates pState)
        {
            _previousState = pState;
        }

        public void Update()
        {
            if (_snakeStateBehaviour.IsHit()) return;
            if (_previousState == ESnakeStates.Hit)
                _previousState = ESnakeStates.Move;
            _snakeStateBehaviour.SetState(_previousState);
        }

        #endregion

    }

}
