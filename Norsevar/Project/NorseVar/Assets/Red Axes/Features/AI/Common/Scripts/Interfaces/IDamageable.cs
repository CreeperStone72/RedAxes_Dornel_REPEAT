using Norsevar.Combat;
using Norsevar.Status_Effect_System;
using UnityEngine;

namespace Norsevar.AI
{

    public interface IDamageable
    {

        #region Public Methods

        float ReceiveDamage(DamageInfo damageInfo);
        void ReceiveHeal(float amount);
        void ReceiveStatusEffect(BaseEffectData effect, GameObject source);

        #endregion

    }

}
