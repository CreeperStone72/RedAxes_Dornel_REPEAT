using System.Collections.Generic;
using Norsevar.Combat;
using Norsevar.Stat_System;
using Norsevar.Status_Effect_System;
using UnityEngine;

namespace Norsevar.Upgrade_System
{
    [RequireComponent(typeof(StatusEffectController))]
    public class UpgradeController : MonoBehaviour
    {

        #region Private Fields

        private StatController _statController;
        private StatusEffectController _statusEffectController;
        private List<Upgrade> _appliedUpgrades;
        private List<PlaystyleModifierBase> _activePlaystyleModifiers;
        private Weapon _equippedWeapon;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _statusEffectController = GetComponent<StatusEffectController>();
            _appliedUpgrades = new List<Upgrade>();
            _activePlaystyleModifiers = new List<PlaystyleModifierBase>();
        }

        #endregion

        #region Private Methods

        private void HandleWeaponHit(Attack attack, List<GameObject> hits)
        {
            foreach (PlaystyleModifierBase playstyleModifier in _activePlaystyleModifiers)
                playstyleModifier.OnHit(attack, hits);
        }

        #endregion

        #region Public Methods

        public void AddPlaystyleModifier(PlaystyleModifierBase modifier)
        {
            if (!modifier)
                return;

            PlaystyleModifierBase playstyleModifier =
                Instantiate(modifier.gameObject, gameObject.transform).GetComponent<PlaystyleModifierBase>();
            playstyleModifier.Initialize(transform, _statController, _statusEffectController);
            _activePlaystyleModifiers.Add(playstyleModifier);
        }

        public bool ApplyUpgrade(Upgrade upgrade)
        {
            if (upgrade == null)
                return false;

            upgrade.ApplyStatModifiers(_statController);
            upgrade.ApplyPlaystyleModifiers(this);

            if (upgrade.UpgradeData is WeaponUpgradeData weaponUpgradeData)
                _equippedWeapon?.ApplyUpgrade(weaponUpgradeData);

            _appliedUpgrades.Add(upgrade);
            return true;
        }

        public void HandleAnimationEvent(EAnimationEventType type)
        {
            foreach (PlaystyleModifierBase playstyleModifier in _activePlaystyleModifiers)
                playstyleModifier.HandleAnimationEvent(_equippedWeapon?.CurrentAttack, type);
        }

        public void Initialize(StatController statController)
        {
            _statController = statController;
        }

        public void OnWeaponChange(Weapon oldWeapon, Weapon newWeapon)
        {
            if (_equippedWeapon != null)
                _equippedWeapon.OnWeaponHit -= HandleWeaponHit;

            if (newWeapon != null)
                newWeapon.OnWeaponHit += HandleWeaponHit;

            _equippedWeapon = newWeapon;
        }

        public void RemovePlaystyleModifier(PlaystyleModifierBase modifier)
        {
            if (!modifier)
                return;


            _activePlaystyleModifiers.Remove(modifier);
            Destroy(modifier);
        }

        public void RemoveUpgrade(Upgrade upgrade)
        {
            upgrade.RemoveStatModifiers(_statController);
            upgrade.RemovePlayStyleModifiers(this);
            _appliedUpgrades.Remove(upgrade);
        }

        #endregion

    }
}
