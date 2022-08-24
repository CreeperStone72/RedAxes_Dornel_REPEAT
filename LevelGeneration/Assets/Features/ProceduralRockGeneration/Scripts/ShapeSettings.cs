namespace ProceduralRockGeneration {
    using UnityEngine;

    [CreateAssetMenu(menuName = "Norsevar/Procedural Rock Generation/Shape Settings")]
    public class ShapeSettings : ScriptableObject {
        public float radius = 1f;
        public NoiseLayer[] noiseLayers;

        [System.Serializable]
        public class NoiseLayer {
            public bool enabled = true;
            public bool useFirstLayerAsMask;
            public NoiseSettings noiseSettings;
        }
    }
}