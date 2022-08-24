namespace ProceduralRockGeneration {
    using UnityEngine;

    public class SimpleNoiseFilter : INoiseFilter {
        private readonly NoiseSettings.SimpleNoiseSettings settings;
        private readonly Noise noise = new Noise();
        
        public SimpleNoiseFilter(NoiseSettings.SimpleNoiseSettings settings) { this.settings = settings; }

        public float Evaluate(Vector3 point) {
            var noiseValue = 0f;
            var frequency = settings.baseRoughness;
            var amplitude = 1f;

            for (var i = 0; i < settings.numLayers; i++) {
                var v = noise.Evaluate(point * frequency + settings.centre);
                noiseValue += (v + 1) * .5f * amplitude;
                frequency *= settings.roughness;
                amplitude *= settings.persistence;
            }

            noiseValue -= settings.minValue;
            return noiseValue * settings.strength;
        }
    }
}