using UnityEngine;
using UnityEngine.Events;

namespace GD.Events
{

    public class GameEventListener : MonoBehaviour
    {
        #region Serialized Fields

        [SerializeField]
        private string description;

        [SerializeField]
        [Tooltip( "Specify the game event (scriptable object) which will raise the event" )]
        private GameEvent Event;

        [SerializeField]
        private UnityEvent Response;

        #endregion

        #region Unity Methods

        private void OnEnable() { Event.RegisterListener( this ); }

        private void OnDisable() { Event.UnregisterListener( this ); }

        #endregion

        #region Public Methods

        public virtual void OnEventRaised() { Response?.Invoke(); }

        #endregion
    }

}