using System.Collections.Generic;
using UnityEngine;

namespace Norsevar
{

    public abstract class BaseGameEvent<P> : ScriptableGameObject
    {

        #region Private Fields

        private readonly List<IGameEventListener<P>> _listeners = new();

        #endregion

        #region Public Methods

        [ContextMenu("Raise Event")]
        public virtual void Raise(P pParameters)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--) _listeners[i].OnEventRaised(pParameters);
        }

        public void RegisterListener(IGameEventListener<P> pListener)
        {
            if (!_listeners.Contains(pListener))
                _listeners.Add(pListener);
        }

        public void UnregisterListener(IGameEventListener<P> pListener)
        {
            if (_listeners.Contains(pListener))
                _listeners.Remove(pListener);
        }

        #endregion

    }

}
