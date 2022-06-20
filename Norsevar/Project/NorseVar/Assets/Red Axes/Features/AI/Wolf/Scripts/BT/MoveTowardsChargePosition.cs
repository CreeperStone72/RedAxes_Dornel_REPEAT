using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Norsevar.AI
{

    [TaskCategory("Norsevar/Wolf")]
    public class MoveTowardsChargePosition : NavMeshAgentBt
    {

        #region Private Fields

        private Vector3 _currentDestination;

        #endregion

        #region Private Methods

        private Vector3 GetTargetPosition()
        {
            return target.Value.position;
        }

        #endregion

        #region Protected Methods

        protected override float GetSpeedModifier()
        {
            return speed.Value;
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
            if (IsAgentAtDestination() || Vector3.Distance(_currentDestination, transform.position) < distanceFromTarget.Value)
                return TaskStatus.Success;

            Move();

            return TaskStatus.Running;
        }

        #endregion

        public SharedFloat distanceFromTarget;
        public SharedFloat speed;
        public SharedTransform target;
    }

}
