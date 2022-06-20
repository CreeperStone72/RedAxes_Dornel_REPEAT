using System;
using System.Collections.Generic;
using Norsevar.Combat;
using Norsevar.Stat_System;
using Norsevar.Status_Effect_System;
using UnityEngine;

namespace Norsevar.Upgrade_System
{
    [Serializable]
    public abstract class PlaystyleModifierBase : MonoBehaviour
    {

        #region Protected Fields

        protected Transform ownerTransform;
        protected StatController statController;
        protected StatusEffectController statusEffectController;

        #endregion

        #region Public Methods

        public virtual void HandleAnimationEvent(Attack attack, EAnimationEventType animationEventType)
        {
        }

        public virtual void Initialize(
            Transform              pOwnerTransform,
            StatController         pStatController,
            StatusEffectController pStatusEffectController)
        {
            ownerTransform = pOwnerTransform;
            statController = pStatController;
            statusEffectController = pStatusEffectController;
        }

        public virtual void OnHit(Attack attack, List<GameObject> entities)
        {
        }

        #endregion

    }
}
