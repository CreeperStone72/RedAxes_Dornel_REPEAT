using UnityEngine;
using UnityEngine.Events;

namespace Norsevar
{

    public class NorseGameEventListener : MonoBehaviour, IGameEventListener
    {

        #region Serialized Fields

        [SerializeField] [Tooltip("Specify the game event (scriptable object) which will raise the event")]
        private GameEvent @event;

        [SerializeField]
        private UnityEvent response;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            @event.RegisterListener(this);
        }

        private void OnDisable()
        {
            @event.UnregisterListener(this);
        }

        #endregion

        #region Public Methods

        public void OnEventRaised()
        {
            response?.Invoke();
        }

        #endregion

    }

}
