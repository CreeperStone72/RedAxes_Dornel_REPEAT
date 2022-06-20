using System.Collections.Generic;
using Norsevar.Status_Effect_System;
using UnityEngine;

namespace Norsevar.Combat
{
    public class DamageInfo
    {

        #region Constructors

        public DamageInfo()
        {
        }

        public DamageInfo(DamageInfo copy)
        {
            SourceGameObject = copy.SourceGameObject;
            DamageType = copy.DamageType;
            DamageValue = copy.DamageValue;
            CanBeBlocked = copy.CanBeBlocked;

            if (copy.EffectsToApply != null)
                EffectsToApply = new List<BaseEffectData>(copy.EffectsToApply);
        }

        #endregion

        #region Properties

        public bool IsCrit { get; set; }

        #endregion

        public GameObject SourceGameObject;
        public EDamageType DamageType;
        public float DamageValue;
        public bool CanBeBlocked = true;
        public List<BaseEffectData> EffectsToApply = new();
    }
}
