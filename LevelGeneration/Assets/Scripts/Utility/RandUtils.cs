namespace Utility {
    using System.Collections.Generic;
    using UnityEngine;
    using Random = UnityEngine.Random;
    
    public static class RandUtils {
        public static T PickOne<T>(T a, T b, float p = .5f) { return Rand01() <= p ? a : b; }

        public static T PickAny<T>(List<T> choices) { return choices[RandInt(choices.Count)]; }
        
        public static float Rand01() { return Random.value; }

        public static float Rand(float min, float max) { return Random.Range(min, max); }

        public static int RandInt(int max) { return RandInt(0, max); }
        
        public static int RandInt(int min, int max) { return Random.Range(min, max); }

        public static Color RandColor() { return Random.ColorHSV(); }
    }
}