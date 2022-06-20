using Norsevar.Status_Effect_System;
using UnityEngine;

namespace Norsevar.Combat
{
    public class BasicAttack : Attack
    {

        #region Constructors

        public BasicAttack(AttackData data, GameObject instigator, WeaponAttackModificationCollection attackModifications) : base(
            data,
            instigator,
            attackModifications)
        {

        }

        #endregion

        #region Properties

        public bool IsFinalComboAttack { get; set; }

        #endregion

        #region Public Methods

        public override EWeaponAttackType GetAttackType()
        {
            return IsFinalComboAttack ? EWeaponAttackType.BasicAttackFinal : EWeaponAttackType.BasicAttack;
        }

        #endregion

    }
}
