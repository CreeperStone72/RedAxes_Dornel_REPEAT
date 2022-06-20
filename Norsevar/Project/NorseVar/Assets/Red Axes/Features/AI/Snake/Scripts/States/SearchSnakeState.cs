using UnityEngine;

namespace Norsevar.AI
{

    public class SearchSnakeState : IState<ESnakeStates>
    {

        #region Private Fields

        private readonly SnakeStateBehaviour _stateBehaviour;
        private Vector3 _currentDestination;

        #endregion

        #region Constructors

        public SearchSnakeState(SnakeStateBehaviour pStateBehaviour)
        {
            _stateBehaviour = pStateBehaviour;
        }

        #endregion

        #region Public Methods

        public ESnakeStates GetState()
        {
            return ESnakeStates.Search;
        }

        public void OnEnter()
        {
            _stateBehaviour.EnableAgent();
            Vector3 currentPosition = _stateBehaviour.transform.position;
            _currentDestination = currentPosition.AddRandomPositionVector(
                _stateBehaviour.GetSearchDistance(),
                _stateBehaviour.GetSearchDistance());
            _stateBehaviour.SetMoveDestination(_currentDestination);
        }

        public void OnExit()
        {
            _stateBehaviour.ResetNavMesh();
            _stateBehaviour.DisableAgent();
        }

        public void Update()
        {
            if (!_stateBehaviour.IsAgentAtDestination())
                return;

            Vector3 currentPosition = _stateBehaviour.transform.position;
            _currentDestination = currentPosition.AddRandomPositionVector(
                _stateBehaviour.GetSearchDistance(),
                _stateBehaviour.GetSearchDistance());
            _stateBehaviour.SetMoveDestination(_currentDestination);
        }

        #endregion

    }

}
