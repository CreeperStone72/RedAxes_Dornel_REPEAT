using Cinemachine;
using UnityEngine;

namespace Norsevar.Combat
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovementBehaviour : MonoBehaviour
    {

        #region Constants and Statics

        private static readonly int Velocity = Animator.StringToHash("Velocity");

        #endregion

        #region Private Fields

        private Animator _animator;
        private Camera _camera;
        private CinemachineBrain _cinemachineBrain;

        private Vector3 _forward, _right;
        private Vector2 _currentInputVector;
        private Vector2 _smoothInputVelocity;
        private float _currentMovementSpeed;
        private float _smoothMovementSpeed;
        private bool _canMove = true;

        #endregion

        #region Properties

        public CharacterController Controller { get; private set; }
        public DirectionMarker DirectionMarker { get; private set; }
        public PlayerMovementData MovementData { get; private set; }
        public Vector2 CurrentInputVector => _currentInputVector;
        public Vector3 CurrentMoveDirection { get; private set; }

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (_canMove)
            {
                CurrentMoveDirection = GetMoveDirection();
                _animator.SetFloat(Velocity, CurrentInputVector.magnitude);
            }

            if (!PlayerInputs.IsUsingGamepad)
                DirectionMarker.OnUpdateBasedOnMouse();
        }

        private void FixedUpdate()
        {
            //Apply Gravity
            if (!Controller.isGrounded)
                Controller.Move(Physics.gravity * Time.fixedDeltaTime);
        }

        private void OnEnable()
        {
            PlayerInputs.OnUsingGamepadChanged += OnUsingGamepadChanged;

            if (_cinemachineBrain)
                _cinemachineBrain.m_CameraActivatedEvent.AddListener(OnCinemachineTransition);
        }

        private void OnDisable()
        {
            PlayerInputs.OnUsingGamepadChanged -= OnUsingGamepadChanged;

            if (_cinemachineBrain)
                _cinemachineBrain.m_CameraActivatedEvent.RemoveListener(OnCinemachineTransition);
        }

        #endregion

        #region Private Methods
        
        private void OnCinemachineTransition(ICinemachineCamera from, ICinemachineCamera to)
        {
            if (_cinemachineBrain.ActiveBlend is null)
                return;
            
            SetCanMove(false);
            
            //Wait for the transition to finish, then update the movement axis
            this.ExecuteInSeconds(
                () =>
                {
                    UpdateCameraAxis();
                    SetCanMove(true);
                },
                _cinemachineBrain.m_DefaultBlend.BlendTime);
        }

        private void OnUsingGamepadChanged(bool isUsingGamepad)
        {
            DirectionMarker.SetActive(!isUsingGamepad);
        }

        private void UpdateCameraAxis()
        {
            _forward = _camera.transform.forward;
            _forward.y = 0;
            _forward = _forward.normalized;
            _right = Quaternion.Euler(new Vector3(0, 90, 0)) * _forward;
        }

        #endregion

        #region Public Methods
        
        public void UpdateDirection(bool useMousePosition = false)
        {
            if (!_canMove)
                return;

            if (useMousePosition && !PlayerInputs.IsUsingGamepad)
                ApplyDirection(DirectionMarker.MouseDirection);
            else
                ApplyDirection(CurrentMoveDirection);
        }

        private void ApplyDirection(Vector3 direction)
        {
            if (direction != Vector3.zero)
                transform.LookAt(transform.position + direction.normalized);
        }

        private Vector3 GetMoveDirection()
        {
            //Get Movement Input and smooth it (important for keyboard gameplay!)
            Vector2 inputVector = PlayerInputs.Instance.GetMovement().ReadValue<Vector2>();
            _currentInputVector = Vector2.SmoothDamp(
                _currentInputVector,
                inputVector,
                ref _smoothInputVelocity,
                MovementData.SmoothInputSpeed);

            //Project movement input onto world plane
            Vector3 inputToV3 = new(_currentInputVector.x, 0, _currentInputVector.y);

            //Transform input based on camera axis (forward should move the player "up")
            Vector3 rightMovement = _right * inputToV3.x;
            Vector3 forwardMovement = _forward * inputToV3.z;

            return rightMovement + forwardMovement;
        }

        public void Initialize(PlayerMovementData movementData, Camera characterCamera, Animator animator)
        {
            MovementData = movementData;
            _camera = characterCamera;
            _cinemachineBrain = characterCamera.GetComponent<CinemachineBrain>();
            _animator = animator;
            Controller = GetComponent<CharacterController>();
            DirectionMarker = new DirectionMarker(transform, characterCamera, movementData);

            _cinemachineBrain.m_CameraActivatedEvent.AddListener(OnCinemachineTransition);
            UpdateCameraAxis();
            DirectionMarker.SetActive(!PlayerInputs.IsUsingGamepad);
        }

        public void SetCanMove(bool canMove)
        {
            if (!canMove)
            {
                _animator.SetFloat(Velocity, 0);
                _currentInputVector = Vector2.zero;
            }

            _canMove = canMove;
        }

        #endregion

    }
}
