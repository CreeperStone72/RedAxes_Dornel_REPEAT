using Norsevar.AI;
using Norsevar.Combat;
using Norsevar.Status_Effect_System;
using UnityEngine;
using UnityEngine.Events;

namespace Norsevar.Animals
{
    public class PlayerHitReactBehavior : MonoBehaviour, IDamageable
    {

        #region Serialized Fields

        public UnityEvent UnityEvent;

        #endregion

        #region Public Methods

        public float ReceiveDamage(DamageInfo damageInfo)
        {
            UnityEvent?.Invoke();
            return -1;
        }

        public void ReceiveHeal(float amount)
        {
        }

        public void ReceiveStatusEffect(BaseEffectData effect, GameObject source)
        {
        }

        #endregion

    }
}
