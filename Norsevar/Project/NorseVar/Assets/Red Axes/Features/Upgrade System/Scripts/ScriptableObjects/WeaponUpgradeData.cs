using System;
using System.Collections.Generic;
using Norsevar.Status_Effect_System;
using UnityEngine;

namespace Norsevar.Upgrade_System
{
    [CreateAssetMenu(fileName = "WeaponUpgrade", menuName = "Norsevar/Upgrades/Weapon Upgrade Data")]
    public class WeaponUpgradeData : UpgradeData
    {

        #region Serialized Fields

        [Header("Weapon Modifications")]
        [Tooltip("These Status Effects will be applied to enemies hit by the specified attack after the Upgrade was consumed.")]
        [SerializeField]
        private List<AttackStatusEffect> weaponAttackStatusEffects;

        #endregion

        #region Properties

        public List<AttackStatusEffect> StatusEffects => weaponAttackStatusEffects;

        #endregion

        #region Unity Methods

        protected override void OnValidate()
        {
            base.OnValidate();
            WeaponAttackDamageMultiplierModifiers ??= new Dictionary<EWeaponAttackType, float>();
        }

        #endregion

        [Serializable]
        public struct AttackStatusEffect
        {

            #region Serialized Fields

            public EWeaponAttackType attackType;
            public BaseEffectData effectData;

            #endregion

        }

        [Space] [Tooltip("Values defined here will be added to the damage multiplier of the attack after the upgrade was consumed.")]
        public Dictionary<EWeaponAttackType, float> WeaponAttackDamageMultiplierModifiers;
    }
}
