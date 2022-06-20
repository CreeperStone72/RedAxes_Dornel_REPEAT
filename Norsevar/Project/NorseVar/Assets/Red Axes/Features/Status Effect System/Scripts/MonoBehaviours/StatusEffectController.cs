using System.Collections.Generic;
using UnityEngine;

namespace Norsevar.Status_Effect_System
{
    [RequireComponent(typeof(IEffectable))]
    public class StatusEffectController : MonoBehaviour
    {

        #region Private Fields

        private IEffectable _effectableObject;
        private Dictionary<EStatusEffectType, BaseEffectData> _activeEffects;
        private List<EStatusEffectType> _effectsToRemove;

        #endregion

        #region Serialized Fields

        [SerializeField] private AuraActivator auraActivator;
        [SerializeField] private StatusEffectEvent onEffectApplyEvent;
        [SerializeField] private StatusEffectEvent onEffectRemoveEvent;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _effectableObject = GetComponent<IEffectable>();
            _activeEffects = new Dictionary<EStatusEffectType, BaseEffectData>();
            _effectsToRemove = new List<EStatusEffectType>();
        }

        private void Update()
        {
            if (auraActivator is null)
                return;

            //Tick all active effects and remove effects that are over
            foreach ((EStatusEffectType effectType, BaseEffectData effectData) in _activeEffects)
            {
                effectData.TickEffect();

                if (effectData.IsActive)
                    continue;

                if (onEffectRemoveEvent) onEffectRemoveEvent.Raise(effectData);
                auraActivator.DisableAura(effectData.AuraType);
                _effectsToRemove.Add(effectType);
                effectData.Destroy();
            }

            //Cannot remove from dictionary during iteration. Hence, Helper List is used to remove effects.
            if (_effectsToRemove.Count <= 0)
                return;

            foreach (EStatusEffectType effectType in _effectsToRemove)
                _activeEffects.Remove(effectType);

            _effectsToRemove.Clear();
        }

        #endregion

        #region Public Methods

        public void ApplyStatusEffect(BaseEffectData data, object source = null)
        {
            if (auraActivator is null)
                return;

            if (_activeEffects.ContainsKey(data.Type))
            {
                _activeEffects[data.Type].AddStack(data);
                return;
            }

            BaseEffectData newEffect = Instantiate(data);

            newEffect.Initialize(_effectableObject);
            _activeEffects.Add(newEffect.Type, newEffect);

            if (onEffectApplyEvent) onEffectApplyEvent.Raise(newEffect);
            auraActivator.EnableAura(data.AuraType);
        }

        public void ApplyStatusEffect(List<BaseEffectData> effects, object source = null)
        {
            if (effects == null)
                return;

            foreach (BaseEffectData effect in effects)
                ApplyStatusEffect(effect, source);
        }

        public bool IsEffectActive(EStatusEffectType effectType)
        {
            return _activeEffects.ContainsKey(effectType);
        }

        #endregion

    }
}
