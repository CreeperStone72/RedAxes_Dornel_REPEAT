namespace Utility {
    using UnityEngine;
    
    public static class Mouse {
        private const int LeftButton = 0;
        private const int RightButton = 1;
        private const int MiddleButton = 2;

        public static bool LeftClick => Input.GetMouseButton(LeftButton);
        public static bool RightClick => Input.GetMouseButton(RightButton);
        public static bool MiddleClick => Input.GetMouseButton(MiddleButton);

        public static bool LeftClickDown => Input.GetMouseButtonDown(LeftButton);
        public static bool RightClickDown => Input.GetMouseButtonDown(RightButton);
        public static bool MiddleClickDown => Input.GetMouseButtonDown(MiddleButton);

        public static bool LeftClickUp => Input.GetMouseButtonUp(LeftButton);
        public static bool RightClickUp => Input.GetMouseButtonUp(RightButton);
        public static bool MiddleClickUp => Input.GetMouseButtonUp(MiddleButton);
        
        public static float ScrollWheel => Input.GetAxis("Mouse ScrollWheel");

        public static bool IsScrolling => ScrollWheel != 0f;
        
        /// <summary>Scrolling down returns -1, scrolling up returns 1</summary>
        public static int ScrollDirection => (int) Mathf.Sign(ScrollWheel);
    }
    
    /// <summary>
    /// Original code by Code Monkey, altered by me to fix z = 0 issues
    /// Source : https://unitycodemonkey.com/utils.php
    /// Last accessed : 10/07
    /// </summary>
    public static class MouseUtils {
        // Get Mouse Position in World with Z = 0f
        public static Vector3 GetMouseWorldPosition(Camera worldCamera) {
            var pos = Input.mousePosition;
            pos.z = -worldCamera.transform.position.z;
            
            var vec = worldCamera.ScreenToWorldPoint(pos);
            vec.z = 0f;
            return vec;
        }
    }
}