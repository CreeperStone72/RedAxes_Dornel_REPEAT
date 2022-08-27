using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Norsevar.VFX;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Norsevar
{
    public class NorseGameEventsListener : MonoBehaviour, IGameEventListener
    {
        #region Private Fields

        private MMFeedbacks _mmFeedbacks;

        #endregion

        #region Serialized Fields

        [SerializeField] [Tooltip("Specify the game event (scriptable object) which will raise the event")]
        private List<NorseGameEvent> events;

        [SerializeField]
        private List<ENorseGameEvent> _eventEnums;

        [SerializeField] private bool useMMFeedbacks;

        [SerializeField] [ShowIf("useMMFeedbacks")]
        private bool isMMFeedbackInterrupt;

        [SerializeField] private UnityEvent response;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (useMMFeedbacks)
                _mmFeedbacks = GetComponent<MMFeedbacks>();
        }

        private void OnEnable()
        {
            foreach (NorseGameEvent @event in events)
                @event.RegisterListener(this);
            foreach (ENorseGameEvent eventEnum in _eventEnums)
                NorseGame.Instance.RegisterListener(eventEnum, this);
        }

        private void OnDisable()
        {
            foreach (NorseGameEvent @event in events)
                @event.UnregisterListener(this);
            foreach (ENorseGameEvent eventEnum in _eventEnums)
                NorseGame.Instance.UnregisterListener(eventEnum, this);
        }

        private void OnValidate()
        {
            if (useMMFeedbacks)
                _mmFeedbacks = GetComponent<MMFeedbacks>();
        }

        #endregion

        #region Public Methods

        public void OnEventRaised()
        {
            if (useMMFeedbacks)
            {
                if (!isMMFeedbackInterrupt) _mmFeedbacks.PlayFeedbacks();
                else _mmFeedbacks.StopFeedbacks();
            }

            response?.Invoke();
        }

        #endregion
    }
}