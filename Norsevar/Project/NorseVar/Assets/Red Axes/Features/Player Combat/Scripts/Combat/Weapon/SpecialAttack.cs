using Norsevar.Status_Effect_System;
using UnityEngine;

namespace Norsevar.Combat
{
    public class SpecialAttack : Attack
    {

        #region Constants and Statics

        private static readonly int SpecialAttackAnimProp = Animator.StringToHash("SpecialAttack");

        #endregion

        #region Constructors

        public SpecialAttack(AttackData data, GameObject instigator, WeaponAttackModificationCollection attackModifications) : base(
            data,
            instigator,
            attackModifications)
        {
        }

        #endregion

        #region Public Methods

        public override void AttackEnd(Animator animator)
        {
            animator.SetBool(SpecialAttackAnimProp, false);
        }

        public override EWeaponAttackType GetAttackType()
        {
            return EWeaponAttackType.SpecialAttack;
        }

        public void Perform(Animator animator)
        {
            animator.SetBool(SpecialAttackAnimProp, true);
        }

        #endregion

    }
}
