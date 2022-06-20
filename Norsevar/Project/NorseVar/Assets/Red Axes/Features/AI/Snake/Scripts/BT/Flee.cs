using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Norsevar.AI.BT
{

    [TaskCategory("Norsevar/Movement")]
    public class Flee : NavMeshMovement
    {

        #region Private Fields

        private bool _hasMoved;

        #endregion

        #region Private Methods

        private Vector3 Target()
        {
            return transform.position + (transform.position - target.Value.position).normalized * lookAheadDistance.Value;
        }

        #endregion

        #region Protected Methods

        protected override bool SetDestination(Vector3 destination)
        {
            return SamplePosition(destination) && base.SetDestination(destination);
        }

        #endregion

        #region Public Methods

        public override void OnReset()
        {
            base.OnReset();

            fledDistance = 20;
            lookAheadDistance = 5;
            target = null;
        }

        public override void OnStart()
        {
            base.OnStart();

            _hasMoved = false;

            SetDestination(Target());
        }

        public override TaskStatus OnUpdate()
        {
            if (Vector3.Magnitude(transform.position - target.Value.position) > fledDistance.Value)
                return TaskStatus.Success;

            if (HasArrived())
            {
                if (!_hasMoved) return TaskStatus.Failure;
                if (!SetDestination(Target())) return TaskStatus.Failure;
                _hasMoved = false;
            }
            else
            {
                float velocityMagnitude = Velocity().sqrMagnitude;
                _hasMoved = velocityMagnitude > 0f;
            }

            return TaskStatus.Running;
        }

        #endregion

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The agent has fled when the magnitude is greater than this value")]
        public SharedFloat fledDistance = 20;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The distance to look ahead when fleeing")]
        public SharedFloat lookAheadDistance = 5;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the agent is fleeing from")]
        public SharedTransform target;
    }

}
