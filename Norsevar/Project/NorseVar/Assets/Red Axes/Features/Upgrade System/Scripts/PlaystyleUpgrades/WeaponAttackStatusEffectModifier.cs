using System.Collections.Generic;
using Norsevar.Combat;
using Norsevar.Status_Effect_System;
using UnityEngine;

namespace Norsevar.Upgrade_System
{
    public class WeaponAttackStatusEffectModifier : PlaystyleModifierBase
    {

        #region Serialized Fields

        [SerializeField] private EWeaponAttackType attackType;
        [SerializeField] private List<BaseEffectData> effectsToApply;

        #endregion

        #region Public Methods

        public override void OnHit(Attack attack, List<GameObject> entities)
        {
            if (attack.GetAttackType() != attackType)
                return;

            base.OnHit(attack, entities);
        }

        #endregion

    }
}
