using System.Collections;
using System.Collections.Generic;
using Norsevar.AI;
using Norsevar.Status_Effect_System;
using UnityEngine;

namespace Norsevar
{
    [RequireComponent(typeof(Collider))]
    public class TriggerStatusEffectApplier : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField] private BaseEffectData effectToApply;
        [SerializeField] [Min(0.1f)] private float applyInterval = 1f;

        #endregion

        #region Private Fields

        private Dictionary<GameObject, Coroutine> _coroutines;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _coroutines = new Dictionary<GameObject, Coroutine>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.TryGetComponent<IDamageable>(out var entity)) 
                return;
            
            ApplyStatusEffect(entity);
            _coroutines.Add(other.gameObject, StartCoroutine(ApplyAfterSeconds(applyInterval, entity)));
        }

        private void OnTriggerExit(Collider other)
        {
            if (!_coroutines.ContainsKey(other.gameObject)) 
                return;
            
            StopCoroutine(_coroutines[other.gameObject]);
            _coroutines.Remove(other.gameObject);
        }

        #endregion

        #region Private Methods

        private void ApplyStatusEffect(IDamageable entity)
        {
            entity.ReceiveStatusEffect(effectToApply, gameObject);
        }

        private IEnumerator ApplyAfterSeconds(float seconds, IDamageable entity)
        {
            while (true)
            {
                yield return new WaitForSeconds(seconds);
                ApplyStatusEffect(entity);
            }
        } 

        #endregion
    }
}
