using UnityEngine;
using UnityEngine.Events;

namespace GD.Events
{

    public class BaseGameEventListener<P, E, R> : MonoBehaviour, IGameEventListener<P> where E : BaseGameEvent<P>
        where R : UnityEvent<P>
    {
        #region Serialized Fields

        [SerializeField]
        private E gameEvent;

        [SerializeField]
        private R response;

        #endregion

        #region Properties

        protected E GameEvent
        {
            get => gameEvent;
            set => gameEvent = value;
        }

        protected R Response
        {
            get => response;
            set => response = value;
        }

        #endregion

        #region Unity Methods

        private void OnEnable() { GameEvent?.RegisterListener( this ); }

        private void OnDisable() { GameEvent?.UnregisterListener( this ); }

        #endregion

        #region Public Methods

        public virtual void OnEventRaised( P parameters ) { Response?.Invoke( parameters ); }

        #endregion
    }

}