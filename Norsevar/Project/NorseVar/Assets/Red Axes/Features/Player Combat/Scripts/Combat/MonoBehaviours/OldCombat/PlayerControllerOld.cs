using Norsevar.Snow;
using Norsevar.Upgrade_System;
using UnityEngine;

namespace Norsevar.Combat.OldCombat
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerControllerOld : CombatEntityController, IConsumer, IUpgradeReceiver
    {

        #region Constants and Statics

        private static readonly int IsHit = Animator.StringToHash("IsHit");
        private static readonly int Death = Animator.StringToHash("Death");

        #endregion

        #region Private Fields

        private Animator _animator;
        private UpgradeController _upgradeController;
        private PlayerMovementOld _playerMovement;

        #endregion

        #region Serialized Fields

        [Header("References")]
        [SerializeField] private PlayerFeedback playerFeedback;

        [Header("Data")]
        [SerializeField] private PlayerDataCollection playerData;

        #endregion

        #region Properties

        public PlayerCombatOld PlayerCombat { get; private set; }

        #endregion

        #region Unity Methods

        protected void Awake()
        {
            Init();
            _animator = GetComponent<Animator>();
            _upgradeController = GetComponent<UpgradeController>();
            PlayerCombat = GetComponent<PlayerCombatOld>();
            _playerMovement = GetComponent<PlayerMovementOld>();

            _upgradeController.Initialize(statController);

            PlayerCombat.Initialize(statController);
            PlayerCombat.OnWeaponChange += _upgradeController.OnWeaponChange;

            _playerMovement.Initialize(statController);

            NorseGame.Instance.Register(this);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.TryGetComponent(out IConsumable consumable))
                consumable.Consume(this);
        }

        #endregion

        #region Protected Methods

        protected override void OnDie()
        {
            base.OnDie();
            _animator.SetTrigger(Death);
            _playerMovement.SetActiveMovement(false);
            PlayerCombat.SetActivePlayerCombat(false);
        }

        protected override void OnHit(float f)
        {
            base.OnHit(f);
            _animator.SetTrigger(IsHit);
        }

        #endregion

        #region Public Methods

        public void ApplyUpgrade(Upgrade upgrade)
        {
            _upgradeController.ApplyUpgrade(upgrade);
        }

        public void ApplyHealthPickup(float healthValue)
        {
            throw new System.NotImplementedException();
        }

        public void HandleAnimationEvent(AnimationEvent pInfo)
        {
            EAnimationEventType animationEventType = (EAnimationEventType)pInfo.intParameter;

            PlayerCombat.HandleAnimationEvent(animationEventType);
            _playerMovement.HandleAnimationEvent(animationEventType, PlayerCombat);
            _upgradeController.HandleAnimationEvent(animationEventType);
        }

        public void HandleFootTouchSnowEvent(AnimationEvent pInfo)
        {
            playerFeedback.OnFootTouchSnow(pInfo);
        }

        public void HandlePlayerBreatheEvent(AnimationEvent pInfo)
        {
            playerFeedback.OnBreathe(pInfo);
        }

        #endregion

    }
}
