using System.Collections;
using UnityEngine;

namespace Norsevar
{
    public class AutoDeactivateAfterSecondsBehaviour : MonoBehaviour
    {

        #region Private Fields

        private Coroutine _coroutine;

        #endregion

        #region Serialized Fields

        [SerializeField] private bool useRealSeconds;

        [SerializeField] private float waitSeconds;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            _coroutine = StartCoroutine(DisableAfterSeconds());
        }

        private void OnDisable()
        {
            StopCoroutine(_coroutine);
        }

        #endregion

        #region Private Methods

        private IEnumerator DisableAfterSeconds()
        {
            if (useRealSeconds) yield return new WaitForSecondsRealtime(waitSeconds);
            else yield return new WaitForSeconds(waitSeconds);

            gameObject.SetActive(false);
        }

        #endregion

    }
}
