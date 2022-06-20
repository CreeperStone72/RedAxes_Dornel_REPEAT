using UnityEngine;

namespace Norsevar.AI
{

    public class DebugTextView : MonoBehaviour
    {

        #region Private Fields

        private Camera _main;
        private Transform _transform;

        #endregion

        #region Unity Methods

        private void Start()
        {
            _main = Camera.main;
            _transform = transform;
        }

        private void Update()
        {
            _transform.LookAt(_main.transform);

            Quaternion transformRotation = _transform.rotation;
            transformRotation.eulerAngles += new Vector3(0, 180, 0);

            _transform.rotation = transformRotation;
        }

        #endregion

    }

}
