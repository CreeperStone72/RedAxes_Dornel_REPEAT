using UnityEngine;
using UnityEngine.Events;

namespace Norsevar
{

    public class BaseGameEventListener<P, E, R> : MonoBehaviour, IGameEventListener<P> where E : BaseGameEvent<P> where R : UnityEvent<P>
    {

        #region Serialized Fields

        [SerializeField]
        private E gameEvent;

        [SerializeField]
        private R response;

        #endregion

        #region Properties

        private E GameEvent => gameEvent;

        private R Response => response;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            GameEvent.RegisterListener(this);
        }

        private void OnDisable()
        {
            GameEvent.UnregisterListener(this);
        }

        #endregion

        #region Public Methods

        public virtual void OnEventRaised(P pParameters)
        {
            Response?.Invoke(pParameters);
        }

        #endregion

    }

}
