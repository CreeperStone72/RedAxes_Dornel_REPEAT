using System;
using System.Collections;
using UnityEngine;

namespace Norsevar
{
    public static class CoroutineHelper
    {

        #region Private Methods

        private static IEnumerator ExecuteInRealSeconds(Action action, float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
            action?.Invoke();
        }

        private static IEnumerator ExecuteInSeconds(Action action, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }

        #endregion

        #region Public Methods

        public static void ExecuteInRealSeconds(this MonoBehaviour caller, Action action, float seconds)
        {
            caller.StartCoroutine(ExecuteInRealSeconds(action, seconds));
        }

        public static void ExecuteInSeconds(this MonoBehaviour caller, Action action, float seconds)
        {
            caller.StartCoroutine(ExecuteInSeconds(action, seconds));
        }

        #endregion

    }
}
