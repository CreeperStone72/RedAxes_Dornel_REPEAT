using System.Collections.Generic;
using Norsevar.Status_Effect_System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.UI
{
    public class UIEffectManager : MonoBehaviour
    {

        #region Private Fields

        private Dictionary<EStatusEffectType, GameObject> _currentEffects;

        #endregion

        #region Serialized Fields

        [SerializeField] private GameObject uiEffectPrefab;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _currentEffects = new Dictionary<EStatusEffectType, GameObject>();

            if (transform.childCount <= 0)
                return;

            foreach (Transform child in transform)
                Destroy(child.gameObject);
        }

        #endregion

        #region Public Methods

        [Button]
        public void AddEffect(BaseEffectData effectData)
        {
            if (_currentEffects.ContainsKey(effectData.Type))
                return;

            GameObject instantiate = Instantiate(uiEffectPrefab, transform);

            instantiate.GetComponentInChildren<ItemInfo>().SetSprite(effectData.Sprite, effectData.StackCount);

            _currentEffects.Add(effectData.Type, instantiate);
        }

        [Button]
        public void RemoveEffect(BaseEffectData effectData)
        {
            if (!_currentEffects.ContainsKey(effectData.Type))
                return;

            GameObject currentEffect = _currentEffects[effectData.Type];

            Destroy(currentEffect);

            _currentEffects.Remove(effectData.Type);
        }

        #endregion

    }
}
