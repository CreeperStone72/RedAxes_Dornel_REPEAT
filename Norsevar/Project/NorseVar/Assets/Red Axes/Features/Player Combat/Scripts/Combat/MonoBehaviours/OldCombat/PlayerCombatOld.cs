using System;
using Norsevar.Stat_System;
using Norsevar.Status_Effect_System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Norsevar.Combat.OldCombat
{

    [RequireComponent(typeof(Animator))]
    public class PlayerCombatOld : MonoBehaviour
    {

        #region Delegates and Events

        public event Action<Weapon, Weapon> OnWeaponChange;

        #endregion

        #region Private Fields

        private Animator _characterAnimator;
        private PlayerInputActions _playerInputActions;
        private Weapon _equippedWeapon;

        #endregion

        #region Serialized Fields

        [Header("References")]
        [SerializeField] private GameObject equippedWeaponObject;

        [Header("Combat")]
        [SerializeField] private WeaponData equippedWeaponData;

        #endregion

        #region Properties

        public Attack CurrentAttack => _equippedWeapon?.CurrentAttack;

        public GameObject EquippedWeaponObject => equippedWeaponObject;

        public WeaponAttackModificationCollection AttackModificationCollection { get; private set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            AttackModificationCollection = new WeaponAttackModificationCollection();
            _playerInputActions = new PlayerInputActions();
            _characterAnimator = GetComponent<Animator>();
        }

        private void Update()
        {
            _equippedWeapon?.Update();
        }

        private void OnEnable()
        {
            PlayerInputs.Instance.AddAttack(Attack, "Attack");

            // todo use Player Inputs Instance

            _playerInputActions.Player.Enable();
            _playerInputActions.Player.ChargeAttack.started += ChargeStart;
            _playerInputActions.Player.ChargeAttack.canceled += ChargeEnd;
            _playerInputActions.Player.SpecialAttack.performed += SpecialAttack;
        }

        private void OnDisable()
        {
            PlayerInputs.Instance?.RemoveAttack("Attack");

            // todo use Player Inputs Instance
            _playerInputActions.Player.Disable();
            _playerInputActions.Player.ChargeAttack.started -= ChargeStart;
            _playerInputActions.Player.ChargeAttack.canceled -= ChargeEnd;
            _playerInputActions.Player.SpecialAttack.performed -= SpecialAttack;
        }

        #endregion

        #region Private Methods

        private void Attack()
        {
            _equippedWeapon?.Attack();
        }

        private void ChargeEnd(InputAction.CallbackContext context)
        {
            _equippedWeapon?.ChargeAttackChargeEnd();
        }

        private void ChargeStart(InputAction.CallbackContext context)
        {
            _equippedWeapon?.ChargeAttackChargeStart();
        }

        private void EquipWeapon(WeaponData weaponData, StatController statController)
        {
            if (!equippedWeaponData || _equippedWeapon != null && _equippedWeapon.Data == weaponData)
                return;

            _equippedWeapon?.RemoveStatModifiers();
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Interaction_EquipWeapon, _characterAnimator.transform.position);
            Weapon oldWeapon = _equippedWeapon;
            _equippedWeapon = new Weapon(
                weaponData,
                gameObject,
                equippedWeaponObject,
                _characterAnimator,
                statController,
                AttackModificationCollection);
            OnWeaponChange?.Invoke(oldWeapon, _equippedWeapon);
        }

        private void SpecialAttack(InputAction.CallbackContext context)
        {
            _equippedWeapon?.PerformSpecialAttack();
        }

        #endregion

        #region Public Methods

        public void HandleAnimationEvent(EAnimationEventType animationEventType)
        {
            _equippedWeapon?.HandleAnimationEvent(animationEventType);
        }

        public void Initialize(StatController statController)
        {
            EquipWeapon(equippedWeaponData, statController);
        }

        public void SetActivePlayerCombat(bool active)
        {
            if (active)
                _playerInputActions.Player.Enable();
            else
                _playerInputActions.Player.Disable();

        }

        #endregion

    }

}
