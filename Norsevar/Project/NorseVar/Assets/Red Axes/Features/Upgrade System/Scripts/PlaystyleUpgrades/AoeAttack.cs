using Norsevar.Combat;
using UnityEngine;

namespace Norsevar.Upgrade_System
{
    public class AoeAttack : PlaystyleModifierBase
    {

        #region Private Fields

        private Attack _attack;

        #endregion

        #region Serialized Fields

        [SerializeField] private AttackData attackData;
        [SerializeField] private GameObject vfxEffect;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _attack = new Attack(attackData, gameObject, null);
        }

        #endregion

        #region Public Methods

        public override void HandleAnimationEvent(Attack attack, EAnimationEventType animationEventType)
        {
            if (animationEventType != EAnimationEventType.AttackHit || attack?.GetAttackType() != EWeaponAttackType.SpecialAttack)
                return;

            _attack.AttackHit(
                ownerTransform,
                ownerTransform.position,
                statController[EStatType.DamageMultiplier],
                statController[EStatType.CriticalStrikeChance]);
            vfxEffect.SetActive(true);
        }

        #endregion

    }
}
