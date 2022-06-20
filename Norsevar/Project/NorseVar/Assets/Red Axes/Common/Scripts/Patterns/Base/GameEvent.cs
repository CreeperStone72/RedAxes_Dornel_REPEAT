using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar
{

    [CreateAssetMenu(fileName = "Parameterless", menuName = "Norsevar/Events/Parameterless")]
    public class GameEvent : SerializedScriptableObject
    {

        #region Private Fields

        private readonly List<IGameEventListener> _listeners = new();

        #endregion

        #region Public Methods

        [ContextMenu("Raise Event")]
        public virtual void Raise()
        {
            for (int i = _listeners.Count - 1; i >= 0; i--) _listeners[i].OnEventRaised();
        }

        public void RegisterListener(IGameEventListener listener)
        {
            if (!_listeners.Contains(listener))
                _listeners.Add(listener);
        }

        public void UnregisterListener(IGameEventListener listener)
        {
            if (_listeners.Contains(listener))
                _listeners.Remove(listener);
        }

        #endregion

    }

}
