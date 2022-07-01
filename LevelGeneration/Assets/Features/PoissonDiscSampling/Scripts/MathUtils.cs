using UnityEngine;

namespace Utility {
    using static Mathf;
    
    public static class MathUtils {
        public static readonly float Sqrt2 = Sqrt(2);

        public static float RandomAngle => Random.value * PI * 2;

        public static Vector2 AngleToDir(float angle) { return new Vector2(Sin(angle), Cos(angle)); }
    }
}