using UnityEngine;

namespace Norsevar.Room_Generation
{
    public static class MinimapCameraManager
    {

        #region Private Methods

        private static void SnapToRoot(LevelTree levelTree, Transform cameraTransform)
        {
            var root = levelTree.GetOrigin();
            root.y = cameraTransform.position.y;
            cameraTransform.position = root;
        }

        private static void ZoomIn()
        {

        }

        private static void ZoomOut()
        {

        }

        #endregion

        #region Public Methods

        public static void MoveCamera(LevelTree levelTree, Transform cameraTransform)
        {
            ZoomOut();
            SnapToRoot(levelTree, cameraTransform);
            ZoomIn();
        }

        #endregion

    }
}
