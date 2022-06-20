using System.Collections;
using UnityEngine;

namespace Norsevar
{

    public class LoadingController : MonoBehaviour
    {

        #region Private Fields

        private int _index;

        #endregion

        #region Unity Methods

        private void Start()
        {
            foreach (Transform child in transform) child.localScale = Vector3.one;
        }

        private void OnEnable()
        {
            StartCoroutine(LoadingCircle());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        #endregion

        #region Private Methods

        private void ActivateNext()
        {
            transform.GetChild(_index++).localScale = Vector3.one;
            _index %= transform.childCount;
            transform.GetChild(_index).localScale = Vector3.one * 2;
        }

        private IEnumerator LoadingCircle()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(.1f);
                ActivateNext();
            }
        }

        #endregion

    }

}
