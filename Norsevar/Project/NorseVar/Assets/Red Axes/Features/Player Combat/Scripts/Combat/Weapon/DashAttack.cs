using Norsevar.Status_Effect_System;
using UnityEngine;

namespace Norsevar.Combat
{
    public class DashAttack : Attack
    {
        #region Constants and Statics

        private static readonly int DashAttackAnimProp = Animator.StringToHash("DashStrike");

        #endregion

        #region Constructors

        public DashAttack(AttackData data, GameObject instigator, WeaponAttackModificationCollection attackModifications) : base(data, instigator, attackModifications)
        {
        }

        #endregion

        #region Public Methods

        public override EWeaponAttackType GetAttackType() => EWeaponAttackType.DashAttack;

        public void Perform(Animator animator)
        {
            animator.SetBool(DashAttackAnimProp, true);
        }

        public override void AttackEnd(Animator animator)
        {
            animator.SetBool(DashAttackAnimProp, false);
        }

        #endregion
    }
}