namespace Utility {
    using UnityEngine;
    using static UnityEngine.Mathf;
    
    public static class MathUtils {
        public static readonly float Sqrt2 = Sqrt(2);

        public static float RandomAngle => Random.value * PI * 2;

        public static Vector2 AngleToDir(float angle) { return new Vector2(Sin(angle), Cos(angle)); }

        public static float Sqr(float x) { return x * x; }

        /// <returns>True if value ∈ [min;max[</returns>
        public static bool IsInRange(int value, int min, int max) { return min <= value && value < max; }

        public static int ModN(int value, int mod) { return value >= 0 ? value % mod : mod + value; }
    }
}