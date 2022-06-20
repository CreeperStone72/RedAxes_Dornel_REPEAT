using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace Norsevar.AI
{
    public class NavMeshAnimationBehaviour : MonoBehaviour
    {

        #region Private Fields

        private Vector3 _groundNormal;
        private float _turnAmount;
        private float _forwardAmount;

        private int _hashIDForward;
        private int _hashIDTurn;
        private Animator _animator;
        private NavMeshAgent _navMesh;

        #endregion

        #region Serialized Fields

        [SerializeField]
        public string forwardParameterName;

        [SerializeField]
        private string turnParameterName;

        [ValidateInput("HasAnimator")] [SerializeField]
        private GameObject animGameObject;

        [ValidateInput("HasNavMesh")] [SerializeField]
        private GameObject navMeshGameObject;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _animator = animGameObject.GetComponent<Animator>();
            _navMesh = navMeshGameObject.GetComponent<NavMeshAgent>();
            _hashIDForward = Animator.StringToHash(forwardParameterName);
            _hashIDTurn = Animator.StringToHash(turnParameterName);
        }

        protected virtual void Update()
        {
            if (_animator is null)
            {
                Debug.LogWarning($"{GetType()}: Animator is null");
                return;
            }

            if (_navMesh is null)
            {
                Debug.LogWarning($"{GetType()}: NavMesh is null");
                return;
            }

            Move(_navMesh.remainingDistance > _navMesh.stoppingDistance ? _navMesh.desiredVelocity : Vector3.zero);

            float speedMultiplier = 1;
            _animator.SetFloat(_hashIDForward, _forwardAmount * speedMultiplier, 0.1f, Time.deltaTime);
            _animator.SetFloat(_hashIDTurn, _turnAmount * speedMultiplier, 0.1f, Time.deltaTime);
        }

        #endregion

        #region Private Methods

        private void ApplyExtraTurnRotation()
        {
            float stationaryTurnSpeed = 180;
            float movingTurnSpeed = 360;
            float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, _forwardAmount);
            transform.Rotate(0, _turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        private void CheckGroundStatus()
        {
            float groundCheckDistance = 0.1f;
            if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hitInfo, groundCheckDistance))
                _groundNormal = hitInfo.normal;
        }

        [UsedImplicitly]
        private bool HasAnimator()
        {
            return animGameObject != null && animGameObject.GetComponent<Animator>() != null;
        }

        [UsedImplicitly]
        private bool HasNavMesh()
        {
            return navMeshGameObject != null && navMeshGameObject.GetComponent<NavMeshAgent>() != null;
        }

        private void Move(Vector3 pDirection)
        {
            if (pDirection.magnitude > 1f) pDirection.Normalize();

            pDirection = transform.InverseTransformDirection(pDirection);

            CheckGroundStatus();
            pDirection = Vector3.ProjectOnPlane(pDirection, _groundNormal);
            _turnAmount = Mathf.Atan2(pDirection.x, pDirection.z);
            _forwardAmount = pDirection.z;

            ApplyExtraTurnRotation();
        }

        #endregion

    }
}
