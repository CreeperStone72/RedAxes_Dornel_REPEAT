using System;
using UnityEngine;

namespace Norsevar.Status_Effect_System
{


    public class AuraActivator : MonoBehaviour
    {

        #region Serialized Fields

        [SerializeField] private AuraDictionary auras;

        #endregion

        #region Public Methods

        public void DisableAura(EAuraType type)
        {
            if (!auras.ContainsKey(type))
                return;

            auras[type].DeactivateAura();
        }

        public void EnableAura(EAuraType type)
        {
            if (!auras.ContainsKey(type))
                return;

            auras[type].ActivateAura();
        }

        #endregion

    }

    [Serializable]
    public class Aura
    {

        #region Private Fields

        private int _activatorCount;

        #endregion

        #region Serialized Fields

        [SerializeField] private GameObject auraGameObject;

        #endregion

        #region Public Methods

        public void ActivateAura()
        {
            _activatorCount++;
            auraGameObject.SetActive(true);
        }

        public void DeactivateAura()
        {
            _activatorCount = Mathf.Max(0, --_activatorCount);

            if (_activatorCount == 0)
                auraGameObject.SetActive(false);
        }

        #endregion

    }
}
