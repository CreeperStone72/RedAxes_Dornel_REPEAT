using UnityEngine;

namespace Norsevar.AI
{

    public class AttackSnakeState : IState<ESnakeStates>
    {

        #region Private Fields

        private readonly SnakeStateBehaviour _snakeStateBehaviour;

        private float _timeBetweenAttacks;

        #endregion

        #region Constructors

        public AttackSnakeState(SnakeStateBehaviour pSnakeStateBehaviour)
        {
            _snakeStateBehaviour = pSnakeStateBehaviour;
        }

        #endregion

        #region Private Methods

        private void Attack()
        {
            if (!_snakeStateBehaviour.TargetIsInAttackRange()) return;
            _snakeStateBehaviour.PlayAttackAnimation();
            _timeBetweenAttacks = _snakeStateBehaviour.GetTimeBetweenAttacks();
        }

        #endregion

        #region Public Methods

        public ESnakeStates GetState()
        {
            return ESnakeStates.Attack;
        }


        public void OnEnter()
        {
            _snakeStateBehaviour.DisableAgent();
        }

        public void OnExit()
        {
        }

        public void Update()
        {
            _snakeStateBehaviour.RotateTowardsTarget();

            if (_timeBetweenAttacks > 0)
            {
                _timeBetweenAttacks -= Time.deltaTime;
                return;
            }

            Attack();
        }

        #endregion

    }

}
