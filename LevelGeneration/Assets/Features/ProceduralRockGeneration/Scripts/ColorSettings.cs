namespace ProceduralRockGeneration {
    using UnityEngine;

    [CreateAssetMenu(menuName = "Norsevar/Procedural Rock Generation/Color Settings")]
    public class ColorSettings : ScriptableObject {
        public Material material;
        public BiomeColorSettings biomeColorSettings;
        public Gradient oceanColor;

        [System.Serializable]
        public class BiomeColorSettings {
            public Biome[] biomes;
            public NoiseSettings noise;
            public float noiseOffset;
            public float noiseStrength;
            [Range(0, 1)] public float blendAmount;
            
            [System.Serializable]
            public class Biome {
                public Gradient gradient;
                public Color tint;
                [Range(0, 1)] public float startHeight;
                [Range(0, 1)] public float tintPercent;
            }
        }
    }

}