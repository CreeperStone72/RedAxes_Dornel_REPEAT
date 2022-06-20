using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks.Movement;
using UnityEngine;
using UnityEngine.AI;

namespace Norsevar.AI.BT
{

    public class NavMeshMovement : Movement
    {

        #region Protected Fields

        protected NavMeshAgent navMeshAgent;

        #endregion

        #region Protected Methods

        protected override bool HasArrived()
        {
            float remainingDistance = navMeshAgent.pathPending ? float.PositiveInfinity : navMeshAgent.remainingDistance;

            return remainingDistance <= arriveDistance.Value;
        }

        protected override bool HasPath()
        {
            return navMeshAgent.hasPath && navMeshAgent.remainingDistance > arriveDistance.Value;
        }

        protected bool SamplePosition(Vector3 position)
        {
            return NavMesh.SamplePosition(position, out NavMeshHit _, navMeshAgent.height * 2, NavMesh.AllAreas);
        }

        protected override bool SetDestination(Vector3 destination)
        {
            navMeshAgent.isStopped = false;
            return navMeshAgent.SetDestination(destination);
        }

        protected override void Stop()
        {
            if (navMeshAgent.hasPath) navMeshAgent.isStopped = true;
        }

        protected override void UpdateRotation(bool update)
        {
            navMeshAgent.updateRotation = update;
            navMeshAgent.updateUpAxis = update;
        }

        protected override Vector3 Velocity()
        {
            return navMeshAgent.velocity;
        }

        #endregion

        #region Public Methods

        public override void OnAwake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public override void OnBehaviorComplete()
        {
            Stop();
        }

        public override void OnEnd()
        {
            Stop();
        }

        public override void OnReset()
        {
            speed = 10;
            angularSpeed = 120;
            arriveDistance = 1;
        }

        public override void OnStart()
        {
            navMeshAgent.speed = speed.Value;
            navMeshAgent.angularSpeed = angularSpeed.Value;
            navMeshAgent.isStopped = false;
            UpdateRotation(false);
        }

        #endregion

        [Tooltip("The speed of the agent")]
        public SharedFloat speed = 10;

        [Tooltip("The angular speed of the agent")]
        public SharedFloat angularSpeed = 120;

        [Tooltip(
            "The agent has arrived when the destination is less than the specified amount. This distance should be greater than or equal to the NavMeshAgent StoppingDistance.")]
        public SharedFloat arriveDistance = 0.2f;
    }

}
