using System.Collections;
using UnityEngine;

namespace Norsevar
{
    public class EnableChildAfterSeconds : MonoBehaviour
    {

        #region Serialized Fields

        public float seconds;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            SetChildrenActive(false);
            StartCoroutine(EnableChild());
        }

        #endregion

        #region Private Methods

        private IEnumerator EnableChild()
        {
            yield return new WaitForSeconds(seconds);
            SetChildrenActive(true);
        }

        private void SetChildrenActive(bool active)
        {
            foreach (object o in transform)
            {
                if (o is Transform t)
                    t.gameObject.SetActive(active);
            }
        }

        #endregion

    }
}
