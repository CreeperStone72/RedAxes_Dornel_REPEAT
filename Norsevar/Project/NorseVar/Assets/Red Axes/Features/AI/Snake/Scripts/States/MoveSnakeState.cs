using UnityEngine;

namespace Norsevar.AI
{

    public class MoveSnakeState : IState<ESnakeStates>
    {

        #region Private Fields

        private readonly SnakeStateBehaviour _stateBehaviour;

        #endregion

        #region Constructors

        public MoveSnakeState(SnakeStateBehaviour pSnakeStateBehaviour)
        {
            _stateBehaviour = pSnakeStateBehaviour;
        }

        #endregion

        #region Public Methods

        public ESnakeStates GetState()
        {
            return ESnakeStates.Move;
        }

        public void OnEnter()
        {
            _stateBehaviour.EnableAgent();
            Vector3 direction = _stateBehaviour.GetDirection();
            Vector3 targetPosition = _stateBehaviour.GetCurrentPosition() + direction;
            _stateBehaviour.SetMoveDestination(targetPosition);
        }

        public void OnExit()
        {
            _stateBehaviour.ResetNavMesh();
            _stateBehaviour.DisableAgent();
            _stateBehaviour.SetMove(0);
        }

        public void Update()
        {
            Vector3 direction = _stateBehaviour.GetDirection();
            Vector3 moveToPosition = _stateBehaviour.GetCurrentPosition() + direction;
            _stateBehaviour.SetMoveDestination(moveToPosition);
        }

        #endregion

    }

}
