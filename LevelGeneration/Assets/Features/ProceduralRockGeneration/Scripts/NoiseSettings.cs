namespace ProceduralRockGeneration {
    using UnityEngine;
    using System;

    [Serializable]
    public class NoiseSettings {
        public enum FilterType { Simple, Rigid, };

        public FilterType filterType;

        public SimpleNoiseSettings Settings {
            get {
                return filterType switch {
                    FilterType.Simple => simpleNoiseSettings,
                    FilterType.Rigid => rigidNoiseSettings,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }

        [ConditionalHide("filterType", 0)]
        public SimpleNoiseSettings simpleNoiseSettings;
        
        [ConditionalHide("filterType", 1)]
        public RigidNoiseSettings rigidNoiseSettings;

        [Serializable]
        public class SimpleNoiseSettings {
            public float strength = 1f;
            [Range(1, 8)] public int numLayers = 1;
            public float baseRoughness = 1f;
            public float roughness = 2f;
            public float persistence = .5f;
            public Vector3 centre;
            public float minValue;
        }

        [Serializable]
        public class RigidNoiseSettings : SimpleNoiseSettings {
            public float weightMultiplier = .8f;
        }
    }
}