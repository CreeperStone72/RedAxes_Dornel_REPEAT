using Norsevar.Combat;
using UnityEngine;

namespace Norsevar.Upgrade_System
{
    public class AnimationEventStatusEffectModifier : PlaystyleModifierBase
    {

        #region Serialized Fields

        [SerializeField] private AnimationTypeStatusEffectDictionary effectsToApply;

        #endregion

        #region Public Methods

        public override void HandleAnimationEvent(Attack weaponAttack, EAnimationEventType animationEventType)
        {
            if (effectsToApply.ContainsKey(animationEventType))
                statusEffectController.ApplyStatusEffect(effectsToApply[animationEventType]);
        }

        #endregion

    }
}
