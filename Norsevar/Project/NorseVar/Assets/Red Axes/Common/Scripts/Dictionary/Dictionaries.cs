using System;
using Norsevar.Stat_System;
using Norsevar.Status_Effect_System;
using Norsevar.VFX;
using UnityEngine;
using UnityEngine.Events;

namespace Norsevar
{
    [Serializable]
    public class StatDictionary : UnitySerializedDictionary<EStatType, StatModifier>
    {
    }

    [Serializable]
    public class StatDefinitionDictionary : UnitySerializedDictionary<EStatType, StatDefinition>
    {
    }

    [Serializable]
    public class NorseGameEventDictionary : UnitySerializedDictionary<int, NorseGameEvent>
    {
    }

    [Serializable]
    public class AuraDictionary : UnitySerializedDictionary<EAuraType, Aura>
    {
    }

    [Serializable]
    public class AnimationTypeStatusEffectDictionary : UnitySerializedDictionary<EAnimationEventType, BaseEffectData>
    {
    }

    [Serializable]
    public class AnimationTypeStatModifierDictionary : UnitySerializedDictionary<EAnimationEventType, StatModifier>
    {
    }

    [Serializable]
    public class AnimationTypeGameObjectDictionary : UnitySerializedDictionary<EAnimationEventType, GameObject>
    {
    }

    [Serializable]
    public class KeyToUnityEventBinderDictionary : UnitySerializedDictionary<KeyCode, KeyEventUnityEventDictionary>
    {
    }

    [Serializable]
    public class KeyEventUnityEventDictionary : UnitySerializedDictionary<KeyEventType, UnityEvent>
    {
    }
}
