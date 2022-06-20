using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Norsevar.AI
{

    [RequireComponent(typeof(AIDataManager))] [RequireComponent(typeof(Rigidbody))] [RequireComponent(typeof(AttackBehaviour))]
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class StateBehaviour<T> : MonoBehaviour, IKickbackAble, IHunter
    {

        #region Private Fields

        private TextMeshPro _debugText;

        private Vector3 _groundNormal;
        private float _turnAmount;
        private float _forwardAmount;

        private NavMeshAgent _agent;
        private Rigidbody _rigidbody;
        private AttackBehaviour _attackBehaviour;

        private bool _isHit;

        #endregion

        #region Protected Fields

        protected AIStates aiStates;
        protected AnimationBehaviour animationBehaviour;

        #endregion

        #region Serialized Fields

        [SerializeField] private float stationaryTurnSpeed = 180;
        [SerializeField] private float movingTurnSpeed = 360;
        [SerializeField] private float groundCheckDistance = 0.1f;
        [SerializeField] protected Transform target;

        #endregion

        #region Properties

        public StateMachine<T> StateMachine { get; protected set; }

        #endregion

        #region Unity Methods

        protected virtual void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _rigidbody = GetComponent<Rigidbody>();
            AIDataManager aiDataManager = GetComponent<AIDataManager>();
            _attackBehaviour = GetComponent<AttackBehaviour>();

            _debugText = GetComponentInChildren<TextMeshPro>();
            animationBehaviour = GetComponentInChildren<AnimationBehaviour>();

            aiStates = aiDataManager.AIStates;

            _agent.updateRotation = false;
        }

        private void Update()
        {
            if (_debugText != null) _debugText.SetText(StateMachine.State.ToString());

            StateMachine.Update();
            if (_agent.enabled)
                Move(_agent.remainingDistance > _agent.stoppingDistance ? _agent.desiredVelocity : Vector3.zero);

            UpdateAnimator();
        }

        #endregion

        #region Private Methods

        private IEnumerator AddForce(Vector3 pForce)
        {
            yield return new WaitForFixedUpdate();
            _rigidbody.AddForce(pForce, ForceMode.Force);

            yield return new WaitUntil(
                () => animationBehaviour.IsAnimationByName("Damaged") || animationBehaviour.IsAnimationByName("Die"));

            yield return new WaitUntil(() => !animationBehaviour.GetAnimationRunning());

            _isHit = false;
        }

        private void ApplyExtraTurnRotation()
        {
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, _forwardAmount);
            transform.Rotate(0, _turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        private void CheckGroundStatus()
        {
            if (Physics.Raycast(GetCurrentPosition() + Vector3.up * 0.1f, Vector3.down, out RaycastHit hitInfo, groundCheckDistance))
                _groundNormal = hitInfo.normal;
        }

        private Vector3 GetTargetPosition()
        {
            return target.position;
        }

        private bool IsAgentEnabled()
        {
            return _agent.enabled;
        }

        private void Move(Vector3 pDirection)
        {
            //Check if direction is normalized
            if (pDirection.magnitude > 1f) pDirection.Normalize();

            //Transform from world to local
            pDirection = transform.InverseTransformDirection(pDirection);

            //Get Ground normal
            CheckGroundStatus();
            pDirection = Vector3.ProjectOnPlane(pDirection, _groundNormal);
            _turnAmount = Mathf.Atan2(pDirection.x, pDirection.z);
            _forwardAmount = pDirection.z;

            ApplyExtraTurnRotation();
        }

        #endregion

        #region Protected Methods

        protected void Attack()
        {
            _attackBehaviour.Attack();
        }

        protected float GetDistanceToTarget()
        {
            return Vector3.Distance(GetCurrentPosition(), target.position);
        }

        protected abstract IState<T> GetState(T pEnum);

        protected bool TargetIsInAggressionRange()
        {
            return aiStates.AggressionRange >= GetDistanceToTarget();
        }

        protected bool TargetIsNotInAggressionRange()
        {
            return !TargetIsInAggressionRange();
        }

        protected virtual void UpdateAnimator()
        {
            animationBehaviour.PlayMove(_forwardAmount, _turnAmount, GetSpeedMultiplier());
        }

        #endregion

        #region Public Methods

        public void AddForceAndWait(Vector3 pForce)
        {
            StartCoroutine(AddForce(pForce));
        }

        public void DisableAgent()
        {
            _agent.enabled = false;
        }

        public void DisableKinematic()
        {
            _rigidbody.isKinematic = false;
        }

        public void EnableAgent()
        {
            _agent.enabled = true;
        }

        public void EnableKinematic()
        {
            _rigidbody.isKinematic = true;
        }

        public float GetAgentSpeed()
        {
            return _agent.speed;
        }

        public Vector3 GetCurrentDestination()
        {
            return _agent.destination;
        }

        public Vector3 GetCurrentPosition()
        {
            return transform.position;
        }

        public Vector3 GetDirection()
        {
            return GetTargetPosition().GetDirection(GetCurrentPosition());
        }

        public abstract float GetSpeedMultiplier();

        public Transform GetTarget()
        {
            return target;
        }

        public bool IsAgentAtDestination(float pDistanceLeft = 0)
        {
            float stoppingDistance;
            return _agent.enabled &&
                   _agent.remainingDistance <=
                   ((stoppingDistance = _agent.stoppingDistance) > pDistanceLeft ? stoppingDistance : pDistanceLeft);
        }

        public bool IsHit()
        {
            return _isHit;
        }

        public virtual void Kickback(Vector3 pDirToTarget, float? damageInfo)
        {
            _isHit = true;
        }

        public void ResetNavMesh()
        {
            if (IsAgentEnabled()) _agent.ResetPath();
        }

        public void RotateTowardsTarget()
        {
            Vector3 direction = GetCurrentPosition().GetDirection(target.position);
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime);
        }

        public void SetAgentSpeed(float pSpeedMultiplier)
        {
            if (IsAgentEnabled()) _agent.speed = pSpeedMultiplier;
        }

        public void SetMove(int pValue)
        {
            _forwardAmount = pValue;
        }

        public void SetMoveDestination(Vector3 pTargetPosition)
        {
            if (IsAgentEnabled()) _agent.SetDestination(pTargetPosition);
        }

        public void SetState(T pEnum)
        {
            StateMachine.SetState(GetState(pEnum));
        }

        public void SetTarget(Transform pTarget)
        {
            target = pTarget;
        }

        #endregion

    }

}
