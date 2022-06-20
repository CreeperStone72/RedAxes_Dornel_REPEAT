using UnityEngine;
using UnityEngine.Events;

namespace Norsevar
{

    public enum KeyEventType
    {
        Down,
        Up
    }

    public class KeyToUnityEventBinder : MonoBehaviour
    {

        #region Serialized Fields

        public KeyToUnityEventBinderDictionary dictionary;

        #endregion

        #region Unity Methods

        private void Update()
        {

            foreach ((KeyCode keyCode, KeyEventUnityEventDictionary keyEventUnityEventDictionary) in dictionary)
            {

                foreach ((KeyEventType keyEventType, UnityEvent unityEvent) in keyEventUnityEventDictionary)
                {
                    if (keyEventType == KeyEventType.Up)
                    {
                        if (Input.GetKeyUp(keyCode)) unityEvent?.Invoke();
                    }
                    else
                    {
                        if (Input.GetKeyDown(keyCode)) unityEvent?.Invoke();
                    }
                }

            }
        }

        #endregion

    }

}
