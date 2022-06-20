using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Norsevar.AI
{

    [TaskCategory("Norsevar/Wolf")]
    public class MoveTowardsTarget : NavMeshAgentBt
    {

        #region Private Fields

        private Vector3 _currentDestination;

        #endregion

        #region Private Methods

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
            return huntData.SpeedMultiplier;
        }

        #endregion

        #region Public Methods

        public override void OnStart()
        {
            base.OnStart();
            _currentDestination = GetTargetPosition();
            SetDestination(_currentDestination);
        }

        public override TaskStatus OnUpdate()
        {
            RotateTowardsTarget();
            if (IsAgentAtDestination())
            {
                OnReset();
                return TaskStatus.Success;
            }

            Move();

            _currentDestination = GetTargetPosition();
            SetDestination(_currentDestination);
            return TaskStatus.Running;
        }

        #endregion

        public BaseData huntData;
        public SharedTransform target;
    }

}
