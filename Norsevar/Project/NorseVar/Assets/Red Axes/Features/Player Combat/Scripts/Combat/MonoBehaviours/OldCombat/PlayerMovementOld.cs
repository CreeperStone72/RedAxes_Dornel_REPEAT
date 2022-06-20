using System.Collections;
using Norsevar.Stat_System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Norsevar.Combat.OldCombat
{
    [RequireComponent(typeof(CharacterController), typeof(Animator))]
    public class PlayerMovementOld : MonoBehaviour
    {

        #region Constants and Statics

        private static readonly int Velocity = Animator.StringToHash("Velocity");
        private static readonly int DashProperty = Animator.StringToHash("Dash");
        private static readonly int DashSpeed = Animator.StringToHash("DashSpeed");
        private static readonly int MovementSpeedMultiplier = Animator.StringToHash("MovementSpeedMultiplier");

        #endregion

        #region Private Fields

        private Animator _characterAnimator;
        private CharacterController _characterController;

        private Vector3 _forward, _right;
        private PlayerInputActions _playerInputActions;
        private bool _canMove = true;
        private bool _isAttacking;
        private bool _canUpdateDirection = true;
        private bool _isDashing;

        private Vector2 _currentInputVector;
        private Vector2 _smoothInputVelocity;

        private float _currentMovementSpeed;
        private float _smoothMovementSpeed;
        private Stat _movementSpeedMultStat;

        private Vector3 _moveDir;

        #endregion

        #region Serialized Fields

        [Header("References")]
        [SerializeField] private Camera characterCamera;
        [SerializeField] private ForceReceiver forceReceiver;

        [Header("Data")]
        [SerializeField] private PlayerMovementData movementData;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _characterAnimator = GetComponent<Animator>();

            _forward = characterCamera.transform.forward;
            _forward.y = 0;
            _forward = _forward.normalized;
            _right = Quaternion.Euler(new Vector3(0, 90, 0)) * _forward;

            _playerInputActions = new PlayerInputActions();
        }

        private void Update()
        {
            _moveDir = GetMoveDirection();
            _characterAnimator.SetFloat(Velocity, _currentInputVector.magnitude);
            _characterAnimator.SetFloat(MovementSpeedMultiplier, _movementSpeedMultStat.Value - 1);
            // _characterAnimator.SetFloat(MovementSpeedMultiplier, _movementSpeedMultStat.Value);

            // if (_moveDir == Vector3.zero)
            // {
            //     _currentMovementSpeed = 0;
            //     return;
            // }

            if (_canUpdateDirection)
                UpdateDirection(_moveDir);

            // if(_movementSpeedMultStat.Value > 0)
            //     Move(_moveDir);
            // else
            //     MovementStopFalloff();
        }

        private void FixedUpdate()
        {
            if (!_characterController.isGrounded)
                _characterController.Move(Physics.gravity * Time.fixedDeltaTime);
        }

        private void OnEnable()
        {
            // todo change this out to use PlayerInputs manager
            _playerInputActions.Player.Enable();
            _playerInputActions.Player.Dash.performed += Dash;
        }

        private void OnDisable()
        {
            // todo change this out to use PlayerInputs manager
            _playerInputActions.Player.Disable();
            _playerInputActions.Player.Dash.performed -= Dash;
        }

        private void OnAnimatorMove()
        {
            if (!_isDashing)
                _characterController.Move(_characterAnimator.deltaPosition);
        }

        #endregion

        #region Private Methods

        private void Dash(InputAction.CallbackContext context)
        {
            if (!_canMove || _isDashing || _isAttacking)
                return;

            _characterAnimator.SetBool(DashProperty, true);
        }

        private IEnumerator DashCoroutine(float time, float speed)
        {
            _characterAnimator.SetBool(DashProperty, true);
            float startTime = Time.time;

            while (Time.time < startTime + time)
            {
                _characterController.SimpleMove(transform.forward * speed);
                yield return null;
            }

            _characterAnimator.SetBool(DashProperty, false);
            _characterAnimator.SetFloat(DashSpeed, 1f);
            _isDashing = false;
        }

        private Vector3 GetMoveDirection()
        {
            //Get Movement Input and smooth it (important for keyboard gameplay!)
            Vector2 inputVector = _playerInputActions.Player.Movement.ReadValue<Vector2>();
            _currentInputVector = Vector2.SmoothDamp(
                _currentInputVector,
                inputVector,
                ref _smoothInputVelocity,
                movementData.SmoothInputSpeed);

            //Project movement input onto world plane
            Vector3 inputToV3 = new(_currentInputVector.x, 0, _currentInputVector.y);

            //Transform input based on camera axis (forward should move the player "up")
            Vector3 rightMovement = _right * inputToV3.x;
            Vector3 forwardMovement = _forward * inputToV3.z;

            return rightMovement + forwardMovement;
        }

        // private void Move(Vector3 direction)
        // {
        //     float movementSpeedMult = _movementSpeedMultStat.Value;
        //     float speed = movementData.MovementSpeed * movementSpeedMult;
        //     _characterController.SimpleMove(direction * speed);
        //     characterAnimator.SetFloat(Velocity, _currentInputVector.magnitude * movementSpeedMult);
        //     _currentMovementSpeed = speed;
        // }

        //prevent movement from stopping abruptly when attacking
        // private void MovementStopFalloff()
        // {
        //     if (!(_currentMovementSpeed > 0f)) return;
        //
        //     _currentMovementSpeed = Mathf.SmoothDamp(_currentMovementSpeed, 0f, ref _smoothMovementSpeed, .3f);
        //     _characterController.SimpleMove(_moveDir * _currentMovementSpeed);
        // }

        private void UpdateDirection(Vector3 direction)
        {
            transform.LookAt(transform.position + direction);
        }

        #endregion

        #region Public Methods

        public void HandleAnimationEvent(EAnimationEventType animationEventType, PlayerCombatOld combat)
        {
            switch (animationEventType)
            {
                case EAnimationEventType.AttackStart:
                    _isAttacking = true;
                    _canMove = _movementSpeedMultStat.Value > 0;
                    _characterAnimator.SetFloat(Velocity, 0);
                    break;
                case EAnimationEventType.AttackEnd:
                    _canMove = true;
                    _isAttacking = false;
                    break;
                case EAnimationEventType.ComboDelayEnd:
                    _isAttacking = combat.CurrentAttack != null;
                    _canMove = _movementSpeedMultStat.Value > 0;
                    break;
                case EAnimationEventType.DashStart:
                    NorseGame.Instance.RaiseEvent(ENorseGameEvent.Player_Movement_Dash, _characterController.transform.position);
                    StartCoroutine(DashCoroutine(movementData.DashTime, movementData.DashSpeed));
                    _isDashing = true;
                    _canUpdateDirection = false;
                    break;
                case EAnimationEventType.DashEnd:
                    _characterAnimator.SetFloat(DashSpeed, .8f);
                    _canUpdateDirection = true;
                    break;
                case EAnimationEventType.AttackMoveForceStart:
                    if (combat.CurrentAttack != null)
                        forceReceiver.AddForce(transform.forward, combat.CurrentAttack.Data.AttackForwardMoveForce);
                    break;

            }
        }

        public void Initialize(StatController statController)
        {
            _movementSpeedMultStat = statController.Stats[EStatType.MovementSpeedMultiplier];

            //This crashes Unity for some reason
            //_characterAnimator.SetFloat(MovementSpeedMultiplier, 1 - _movementSpeedMultStat.Value);
            // _movementSpeedMultStat.OnValueChanged += () =>
            // {
            //     _characterAnimator.SetFloat(MovementSpeedMultiplier, 1 - _movementSpeedMultStat.Value);
            // };
        }

        public void SetActiveMovement(bool active)
        {
            if (active)
                _playerInputActions.Player.Enable();
            else
                _playerInputActions.Player.Disable();
        }

        #endregion

    }
}
