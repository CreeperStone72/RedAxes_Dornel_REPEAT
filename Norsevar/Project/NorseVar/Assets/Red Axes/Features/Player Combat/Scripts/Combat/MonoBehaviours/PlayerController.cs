using Norsevar.Snow;
using Norsevar.Upgrade_System;
using UnityEngine;

namespace Norsevar.Combat
{
    [RequireComponent(typeof(PlayerMovementBehaviour), typeof(PlayerCombatBehaviour), typeof(ForceReceiver))]
    public class PlayerController : CombatEntityControllerPlayer, IConsumer, IUpgradeReceiver
    {

        #region Constants and Statics

        private static readonly int IsHit = Animator.StringToHash("IsHit");

        #endregion

        #region Private Fields

        private UpgradeController _upgradeController;
        private FSM.StateMachine<PlayerStateType> _stateMachine;

        #endregion

        #region Serialized Fields

        [Header("Movement")]
        [SerializeField] private PlayerMovementData movementData;
        [SerializeField] private Camera followCamera;

        [Header("Combat")]
        [SerializeField] private WeaponData equippedWeaponData;
        [SerializeField] private GameObject equippedWeaponObject;

        [Header("Feedback")]
        [SerializeField] private PlayerFeedback playerFeedback;

        //todo replace this with Feel
        [SerializeField] private GameObject weaponTrailsPrefab;
        [SerializeField] private GameObject weaponTrailsParent;
        
        [Header("States")] 
        [SerializeField] private PlayerStateType startingState = PlayerStateType.Idle;

        #endregion

        #region Properties

        public PlayerCombatBehaviour Combat { get; private set; }
        public PlayerMovementBehaviour Movement { get; private set; }
        public Animator Animator { get; private set; }
        public ForceReceiver ForceReceiver { get; private set; }
        public ParticleSystem TrailParticleSystem { get; private set; }

        public bool AttackEnabled { get; set; } = true;

        #endregion

        #region Unity Methods

        protected void Awake()
        {
            Animator = GetComponent<Animator>();
            ForceReceiver = GetComponent<ForceReceiver>();
            
            Init();
            
            InitializeMovement();
            InitializeCombat();
            InitializeStateMachine();

            EnableStates();
            NorseGame.Instance.Register(this);
        }

        private void Update()
        {
            //Update the State Machine
            _stateMachine.OnLogic();
        }

        private void OnEnable()
        {
            if (statController is not null)
                EnableStates();
        }

        private void OnDisable()
        {
            NorseGame.Instance.Unregister<PlayerController>();
        }

        private void OnAnimatorMove()
        {
            (_stateMachine.ActiveState as PlayerBaseState)?.OnAnimatorMove();
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.TryGetComponent(out IConsumable consumable))
                consumable.Consume(this);
        }

        #endregion

        #region Private Methods

        private void EnableStates()
        {
            foreach (var state in _stateMachine.States)
                (state as PlayerBaseState)?.OnEnable();
        }

        private void InitializeCombat()
        {
            _upgradeController = GetComponent<UpgradeController>();
            _upgradeController.Initialize(statController);

            Combat = GetComponent<PlayerCombatBehaviour>();
            Combat.OnWeaponChange += _upgradeController.OnWeaponChange;
            Combat.Initialize(Animator, equippedWeaponData, equippedWeaponObject, statController);
        }

        private void InitializeMovement()
        {
            Movement = GetComponent<PlayerMovementBehaviour>();
            Movement.Initialize(movementData, followCamera, Animator);
        }

        private void InitializeStateMachine()
        {
            _stateMachine = new FSM.StateMachine<PlayerStateType>(false);

            _stateMachine.AddState(PlayerStateType.Idle, new IdleMoveState(this, -1f));
            _stateMachine.AddState(PlayerStateType.Dash, new DashState(this, movementData, 2f));
            _stateMachine.AddState(PlayerStateType.BasicAttack, new BasicAttackComboState(this, 3f));
            _stateMachine.AddState(PlayerStateType.SpecialAttack, new SpecialAttackState(this, 3f));
            _stateMachine.AddState(PlayerStateType.ChargeAttack, new ChargeAttackState(this, -1f));
            _stateMachine.AddState(PlayerStateType.DashAttack, new DashAttackState(this, 3f));
            _stateMachine.AddState(PlayerStateType.DefenceAttack, new DefenseAttackState(this, -1f));
            _stateMachine.AddState(PlayerStateType.Death, new DeathState(this, -1f));
            _stateMachine.AddState(PlayerStateType.ReceiveUpgrade, new UpgradePickupState(this, 3f));
            _stateMachine.AddState(PlayerStateType.StartUnconscious, new UnconsciousState(this, -1f));

            //Idle Transitions
            PlayerInputActions.PlayerActions playerActions = PlayerInputs.Instance.GetPlayerActions();
            _stateMachine.AddTransition(
                PlayerStateType.Idle,
                PlayerStateType.Dash,
                _ => playerActions.Dash.WasPerformedThisFrame());
            _stateMachine.AddTransition(
                PlayerStateType.Idle,
                PlayerStateType.BasicAttack,
                _ => playerActions.Attack.WasPerformedThisFrame() && AttackEnabled);
            _stateMachine.AddTransition(
                PlayerStateType.Idle,
                PlayerStateType.SpecialAttack,
                _ => playerActions.SpecialAttack.WasPerformedThisFrame());
            _stateMachine.AddTransition(
                PlayerStateType.Idle,
                PlayerStateType.ChargeAttack,
                _ => playerActions.ChargeAttack.IsPressed());
            _stateMachine.AddTransition(
                PlayerStateType.Idle,
                PlayerStateType.DefenceAttack,
                _ => playerActions.DefensiveAttack.IsPressed());
            
            _stateMachine.SetStartState(startingState);
            _stateMachine.Init();
        }

        #endregion

        #region Protected Methods

        protected override void OnDie()
        {
            base.OnDie();
            _stateMachine.RequestStateChange(PlayerStateType.Death);
        }

        protected override void OnHit(float f)
        {
            base.OnHit(f);
            Animator.SetTrigger(IsHit);
        }

        #endregion

        #region Public Methods

        public void ApplyUpgrade(Upgrade upgrade)
        {
            if (_upgradeController.ApplyUpgrade(upgrade))
                _stateMachine.RequestStateChange(PlayerStateType.ReceiveUpgrade);
        }

        public void ApplyHealthPickup(float healthValue)
        {
            PlayerHealth.ReceiveHeal(healthValue);
        }

        public void FetchNewTrailParticleSystem()
        {
            if(TrailParticleSystem != null) DiscardTrailParticleSystem();

            TrailParticleSystem = NorseGame.Instance.Get<ObjectPooler>().PoolVFX(weaponTrailsPrefab, 2).GetComponent<ParticleSystem>();
            TrailParticleSystem.gameObject.transform.SetParent(weaponTrailsParent.transform, false);
            TrailParticleSystem.gameObject.SetActive(true);
        }

        public void DiscardTrailParticleSystem()
        {
            TrailParticleSystem = null;
        }

        public void HandleAnimationEvent(AnimationEvent info)
        {
            //Get the animation event type and pass it to the currently active state.
            EAnimationEventType animationEventType = (EAnimationEventType)info.intParameter;
            (_stateMachine.ActiveState as PlayerBaseState)?.HandleAnimationEvent(animationEventType);
            
            //Playstyle upgrades need to handle animation events to know when to spawn certain effects.
            _upgradeController.HandleAnimationEvent(animationEventType);
        }

        public void HandleFootTouchSnowEvent(AnimationEvent info)
        {
            playerFeedback.OnFootTouchSnow(info);
        }

        public void HandlePlayerBreatheEvent(AnimationEvent info)
        {
            playerFeedback.OnBreathe(info);
        }

        #endregion

    }
}
