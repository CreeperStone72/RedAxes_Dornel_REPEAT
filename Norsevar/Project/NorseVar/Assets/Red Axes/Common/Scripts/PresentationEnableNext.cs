using UnityEngine;
using UnityEngine.Serialization;

namespace Norsevar
{
    public class PresentationEnableNext : MonoBehaviour
    {

        #region Private Fields

        private int _index;

        #endregion

        #region Serialized Fields

        [SerializeField] [FormerlySerializedAs("ResetOnEnable")]
        private bool resetOnEnable = true;

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetButtonDown("LB")) Prev();
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetButtonDown("RB")) Next();
        }

        private void OnEnable()
        {
            if (resetOnEnable) ResetIndex();
        }

        #endregion

        #region Private Methods

        private void Next()
        {
            transform.GetChild(_index++).gameObject.SetActive(false);
            _index %= transform.childCount;
            transform.GetChild(_index).gameObject.SetActive(true);
        }

        private void Prev()
        {
            transform.GetChild(_index--).gameObject.SetActive(false);
            _index %= transform.childCount;
            transform.GetChild(_index).gameObject.SetActive(true);
        }

        private void ResetIndex()
        {
            _index = 0;
        }

        #endregion

    }
}
