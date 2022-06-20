using System;
using UnityEngine;
using UnityEngine.Events;

namespace Norsevar.Status_Effect_System
{

    [CreateAssetMenu(fileName = "StatusEffectEvent", menuName = "Norsevar/Status Effect System/Events/Status Effect Event")]
    public class StatusEffectEvent : BaseGameEvent<BaseEffectData>
    {
        //public override void Raise(PickupData parameters)
        //{
        //    base.Raise(parameters);
        // //}
    }

    [Serializable]
    public class UnityStatusEffectEvent : UnityEvent<BaseEffectData>
    {
    }
}
