using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Norsevar.AI
{

    [TaskCategory("Norsevar/Wolf")]
    public class MoveAway : NavMeshAgentBt
    {

        #region Private Fields

        private Vector3 _currentDestination;

        #endregion

        #region Private Methods

        private Vector3 GetNewPosition()
        {
            return transform.position + -transform.forward * distanceFromTarget;
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
            _currentDestination = GetNewPosition();
            SetDestination(_currentDestination);
        }

        public override TaskStatus OnUpdate()
        {
            if (IsAgentAtDestination())
            {
                OnReset();
                return TaskStatus.Success;
            }

            Move();
            return TaskStatus.Running;
        }

        #endregion

        public float distanceFromTarget;

        public BaseData huntData;
    }

}
