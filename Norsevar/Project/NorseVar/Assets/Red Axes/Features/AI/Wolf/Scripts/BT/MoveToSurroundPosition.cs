using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Norsevar.AI
{

    [TaskCategory("Norsevar/Wolf")]
    public class MoveToSurroundPosition : NavMeshAgentBt
    {

        #region Private Fields

        private Vector3 _currentDestination;

        #endregion

        #region Private Methods

        private Vector3 GetPositionAroundTarget()
        {
            return packBehaviour.Value.GetPositionAroundTarget(id.Value, surroundData.DistanceFromTarget);
        }

        private Vector3 GetTargetPosition()
        {
            return target.Value.position;
        }

        private void RotateTowardsTarget()
        {
            Vector3 direction = transform.position.GetDirection(GetTargetPosition());
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);
        }

        #endregion

        #region Protected Methods

        protected override float GetSpeedModifier()
        {
            return surroundData.SpeedMultiplier;
        }

        #endregion

        #region Public Methods

        public override void OnStart()
        {
            base.OnStart();
            _currentDestination = GetPositionAroundTarget();
            SetDestination(_currentDestination);
        }

        public override TaskStatus OnUpdate()
        {
            Debug.Log(_currentDestination);
            if (IsAgentAtDestination())
                return TaskStatus.Success;

            RotateTowardsTarget();
            Move();

            _currentDestination = GetPositionAroundTarget();
            SetDestination(_currentDestination);
            return TaskStatus.Running;
        }

        #endregion

        public SharedInt id;
        public SharedPackBehaviour packBehaviour;

        public ApproachData surroundData;
        public SharedTransform target;
    }

}
