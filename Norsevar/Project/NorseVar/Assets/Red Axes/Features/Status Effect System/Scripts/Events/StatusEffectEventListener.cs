using System;

namespace Norsevar.Status_Effect_System
{
    [Serializable]
    public class StatusEffectEventListener : BaseGameEventListener<BaseEffectData, StatusEffectEvent, UnityStatusEffectEvent>
    {
    }
}
