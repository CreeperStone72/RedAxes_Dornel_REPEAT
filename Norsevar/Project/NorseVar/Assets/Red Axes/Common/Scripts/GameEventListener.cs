using UnityEngine;
using UnityEngine.Events;

namespace Norsevar
{
    public class GameEventListener : MonoBehaviour, IGameEventListener
    {

        #region Serialized Fields

        [SerializeField] private ENorseGameEvent gameEvent;
        [SerializeField] private UnityEvent @event;
        [SerializeField] private int seconds;


        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            NorseGame.Instance[gameEvent].RegisterListener(this);
        }

        private void OnDisable()
        {
            NorseGame.Instance[gameEvent].UnregisterListener(this);
        }

        #endregion

        #region Public Methods

        public void OnEventRaised()
        {
            this.ExecuteInSeconds(() => @event.Invoke(), seconds);
        }

        #endregion

    }
}
