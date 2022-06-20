using System;
using Norsevar.Combat;
using Norsevar.Stat_System;
using Norsevar.Status_Effect_System;
using UnityEngine;

namespace Norsevar.Combat
{
    public class PlayerCombatBehaviour : MonoBehaviour
    {
        #region Delegates and Events

        public event Action<Weapon, Weapon> OnWeaponChange;

        #endregion

        #region Private Fields

        private Animator _characterAnimator;
        private Weapon _equippedWeapon;
        private StatController _statController;
        private GameObject _equippedWeaponObject;
        private MeshFilter _equippedWeaponMeshFilter;
        private WeaponAttackModificationCollection _attackModificationCollection;

        #endregion

        #region Properties

        public Attack CurrentAttack => _equippedWeapon?.CurrentAttack;
        public Weapon EquippedWeapon => _equippedWeapon;

        #endregion

        private void Awake()
        {
            _attackModificationCollection = new WeaponAttackModificationCollection();
        }

        private void Update()
        {
            _equippedWeapon?.Update();
        }

        public void EquipWeapon(WeaponData weaponData)
        {
            if (!weaponData || _equippedWeapon != null && _equippedWeapon.Data == weaponData)
                return;

            _equippedWeapon?.RemoveStatModifiers();
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Interaction_EquipWeapon, _characterAnimator.transform.position);
            
            Weapon oldWeapon = _equippedWeapon;
            _equippedWeaponMeshFilter.mesh = weaponData.WeaponMesh;
            _equippedWeapon = new Weapon(
                weaponData,
                gameObject,
                _equippedWeaponObject,
                _characterAnimator,
                _statController,
                _attackModificationCollection);
            OnWeaponChange?.Invoke(oldWeapon, _equippedWeapon);
        }

        #region Public Methods

        public void Initialize(
            Animator       animator,
            WeaponData     startWeaponData,
            GameObject     equippedWeaponObject,
            StatController statController)
        {
            _characterAnimator = animator;
            _statController = statController;
            _equippedWeaponObject = equippedWeaponObject;
            _equippedWeaponMeshFilter = equippedWeaponObject.GetComponent<MeshFilter>();
            EquipWeapon(startWeaponData);
        }

        #endregion

    }
}
