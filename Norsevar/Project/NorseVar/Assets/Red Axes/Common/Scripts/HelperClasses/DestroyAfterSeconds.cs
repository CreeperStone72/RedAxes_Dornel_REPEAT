using System.Collections;
using UnityEngine;

namespace Norsevar
{
    public class DestroyAfterSeconds : MonoBehaviour
    {

        #region Serialized Fields

        public float seconds;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            StartCoroutine(DestroyCoroutine());
        }

        #endregion

        #region Private Methods

        private IEnumerator DestroyCoroutine()
        {
            yield return new WaitForSeconds(seconds);
            Destroy(gameObject);
        }

        #endregion

    }
}
