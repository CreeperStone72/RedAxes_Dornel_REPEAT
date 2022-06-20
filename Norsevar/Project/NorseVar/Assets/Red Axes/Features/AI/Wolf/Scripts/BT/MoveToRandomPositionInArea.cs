using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Norsevar.AI
{

    [TaskCategory("Norsevar/Wolf")]
    public class MoveToRandomPositionInArea : NavMeshAgentBt
    {

        #region Private Fields

        private Vector3 _currentDestination;
        private Vector3 _startPosition;

        #endregion

        #region Private Methods

        private float Range(float value)
        {
            return Random.Range(value - radius.Value, value + radius.Value);
        }

        private Vector3 TargetPosition()
        {
            return new Vector3(Range(_startPosition.x), _startPosition.y, Range(_startPosition.z));
        }

        #endregion

        #region Protected Methods

        protected override float GetSpeedModifier()
        {
            return speed.Value;
        }

        #endregion

        #region Public Methods

        public override void OnAwake()
        {
            base.OnAwake();
            _startPosition = transform.position;
        }

        public override void OnStart()
        {
            base.OnStart();
            _currentDestination = TargetPosition();
            SetDestination(_currentDestination);
        }

        public override TaskStatus OnUpdate()
        {
            if (IsAgentAtDestination())
                return TaskStatus.Success;

            Move();
            return TaskStatus.Running;
        }

        #endregion

        public SharedFloat speed;
        public SharedFloat radius;
    }

}
