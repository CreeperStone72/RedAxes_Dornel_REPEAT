using System.Collections.Generic;
using UnityEngine;

namespace Norsevar
{
    public class CameraManager : Singleton<CameraManager>
    {

        #region Private Fields

        private List<Camera> _cameras;

        #endregion

        #region Protected Methods

        protected override void OnAwake()
        {
            _cameras = new List<Camera>();
            _cameras.AddRange(FindObjectsOfType<Camera>());
        }

        #endregion

        #region Public Methods

        public void SwapCamera(string pCamera)
        {
            foreach (Camera cam in _cameras)
                cam.enabled = false;

            Camera find = _cameras.Find(pCam => pCam.gameObject.name == pCamera);
            find.enabled = true;
        }

        #endregion

    }
}
