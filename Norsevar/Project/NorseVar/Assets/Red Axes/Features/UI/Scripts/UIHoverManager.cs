using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Norsevar.UI
{
    public class UIHoverManager : Singleton<UIHoverManager>
    {

        #region Delegates and Events

        public static event Action<string[]> OnHoverUI;

        #endregion

        #region Private Fields

        private string[] _previous;

        private int _uiLayer;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            TimeTickSystem.Instance.RegisterListener(TimeTickSystem.TickRateMultiplierType.Four, HandleTick);
        }

        private void OnDisable()
        {
            TimeTickSystem.Instance.UnregisterListener(TimeTickSystem.TickRateMultiplierType.Four, HandleTick);
        }

        #endregion

        #region Private Methods

        private static IEnumerable<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new(EventSystem.current)
            {
                position = Input.mousePosition
            };
            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(eventData, results);
            return results;
        }

        private void HandleTick()
        {
            OnHoverUI?.Invoke(IsPointerOverUIElement());
        }

        private string[] IsPointerOverUIElement()
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults());
        }


        private string[] IsPointerOverUIElement(IEnumerable<RaycastResult> eventSystemRaycastResults)
        {
            string[] names = (from curRaycastResult in eventSystemRaycastResults
                              where curRaycastResult.gameObject.layer == _uiLayer
                              select curRaycastResult.gameObject.name).ToArray();
            return names;
        }

        #endregion

        #region Protected Methods

        protected override void OnAwake()
        {
            _uiLayer = LayerMask.NameToLayer("UI");

        }

        #endregion

    }
}
