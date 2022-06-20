using System;
using System.Collections.Generic;
using Norsevar.VFX;
using UnityEngine;

namespace Norsevar
{

    public class NorseGame : Singleton<NorseGame>
    {

        #region Private Fields

        private Dictionary<Type, object> _dictionary;

        private NorseGameEventBindingsSO _norseGameEventBindings;

        #endregion

        #region Properties

        private NorseGameEventBindingsSO NorseGameEventBindings
        {
            get
            {
                if (_norseGameEventBindings == null)
                    _norseGameEventBindings = Resources.Load<NorseGameEventBindingsSO>("Data/NorseGameEventBindings");

                return _norseGameEventBindings;
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnAwake()
        {
            _dictionary = new Dictionary<Type, object>();
        }

        #endregion

        #region Public Methods

        public T Get<T>() where T : class
        {
            if ( !_dictionary.ContainsKey( typeof(T) ) )
                return null;
            
            T res = _dictionary[typeof(T)] as T;
            return res;
        }

        public void RaiseEvent(ENorseGameEvent id, params object[] args)
        {
            NorseGameEventBindings[(int)id].Raise(args);
        }
        
        public void RegisterListener(ENorseGameEvent id, IGameEventListener listener)
        {
            NorseGameEventBindings[(int)id].RegisterListener(listener);
        }
        
        public void UnregisterListener(ENorseGameEvent id, IGameEventListener listener)
        {
            NorseGameEventBindings[(int)id].UnregisterListener(listener);
        }
        
        public void RegisterAction(ENorseGameEvent id, Action action)
        {
            NorseGameEventBindings[(int)id].RegisterAction(action);
        }
        
        public void UnregisterAction(ENorseGameEvent id, Action action)
        {
            NorseGameEventBindings[(int)id].UnregisterAction(action);
        }

        public void Register<T>(T instance) where T : class
        {
            if (_dictionary.ContainsKey(typeof(T)))
                throw new Exception("A NorseGameMonoBehavior of type " + typeof(T) + " has been Instantiated more than once!");

            _dictionary.Add(typeof(T), instance);
        }

        public void Unregister<T>() where T : class
        {
            _dictionary.Remove(typeof(T));
        }

        #endregion

        public NorseGameEvent this[ENorseGameEvent id] => NorseGameEventBindings[(int)id];
    }

}
