using System;
using BehaviorDesigner.Runtime;
using UnityEngine;
using UnityEngine.AI;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

namespace Norsevar.AI
{

    public abstract class NavMeshAgentBt : Action
    {

        #region Constants and Statics

        private const float GROUND_CHECK_DISTANCE = 0.1f;
        private const float STATIONARY_TURN_SPEED = 180;
        private const float MOVING_TURN_SPEED = 360;
        private const float INITIAL_SPEED = 1;

        #endregion

        #region Private Fields

        private float _forwardAmount;
        private Vector3 _groundNormal;
        private float _turnAmount;
        private NavMeshAgent _navMeshAgent;
        protected BoxCollider collider;

        #endregion

        #region Private Methods

        private void ApplyExtraTurnRotation()
        {
            float turnSpeed = Mathf.Lerp(STATIONARY_TURN_SPEED, MOVING_TURN_SPEED, _forwardAmount);
            transform.Rotate(0, _turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        private void CheckGroundStatus()
        {
            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hitInfo, GROUND_CHECK_DISTANCE))
                _groundNormal = hitInfo.normal;
        }

        private void Move(Vector3 direction)
        {
            //Check if direction is normalized
            if (direction.magnitude > 1f) direction.Normalize();

            //Transform from world to local
            direction = transform.InverseTransformDirection(direction);

            //Get Ground normal
            CheckGroundStatus();
            direction = Vector3.ProjectOnPlane(direction, _groundNormal);
            _turnAmount = Mathf.Atan2(direction.x, direction.z);
            _forwardAmount = direction.z;

            ApplyExtraTurnRotation();
        }

        private void SetSpeed()
        {
            _navMeshAgent.speed = INITIAL_SPEED * GetSpeedModifier();
        }

        private void UpdateAnimator()
        {
            animationBehaviour.Value.PlayMove(_forwardAmount, _turnAmount, GetSpeedModifier());
        }

        #endregion

        #region Protected Methods

        protected abstract float GetSpeedModifier();

        protected void DisableNavMesh()
        {
            _navMeshAgent.ResetPath();
            _navMeshAgent.enabled = false;
        }

        protected void EnableNavMesh()
        {
            _navMeshAgent.enabled = true;
        }

        protected bool IsAgentAtDestination()
        {
            return _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance;
        }

        protected void Move()
        {
            Move(_navMeshAgent.desiredVelocity);
            UpdateAnimator();
        }

        protected void Reset()
        {
            _navMeshAgent?.ResetPath();
            if (_navMeshAgent)
            {
                _navMeshAgent.velocity = Vector3.zero;
                _navMeshAgent.speed = INITIAL_SPEED;
            }

            _forwardAmount = 0;
            _turnAmount = 0;
            _groundNormal = Vector3.zero;
        }

        protected void Rotate(Vector3 direction)
        {
            Move(direction);
            UpdateAnimator();
        }

        protected void SetDestination(Vector3 destination)
        {
            _navMeshAgent.SetDestination(destination);
        }

        #endregion

        #region Public Methods

        public override void OnAwake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _navMeshAgent.updateRotation = false;
            collider = GetComponent<BoxCollider>();
        }

        public override void OnReset()
        {
            Reset();
        }

        public override void OnStart()
        {
            SetSpeed();
        }

        #endregion

        public SharedAnimationBehaviour animationBehaviour;
    }

    [Serializable]
    public class SharedAnimationBehaviour : SharedVariable<AnimationBehaviour>
    {

        #region Public Methods

        public static implicit operator SharedAnimationBehaviour(AnimationBehaviour value)
        {
            return new SharedAnimationBehaviour { Value = value };
        }

        #endregion

    }

    [Serializable]
    public class SharedPackBehaviour : SharedVariable<PackBehaviour>
    {

        #region Public Methods

        public static implicit operator SharedPackBehaviour(PackBehaviour value)
        {
            return new SharedPackBehaviour { Value = value };
        }

        #endregion

    }

}
