namespace ProceduralRockGeneration {
    using UnityEngine;

    public class ShapeGenerator {
        private ShapeSettings _settings;
        private INoiseFilter[] noiseFilters;
        public MinMax elevationMinMax;

        public void UpdateSettings(ShapeSettings settings) {
            _settings = settings;
            noiseFilters = new INoiseFilter[settings.noiseLayers.Length];
            for (var i = 0; i < noiseFilters.Length; i++) { noiseFilters[i] = NoiseFilterFactory.CreateNoiseFilter(settings.noiseLayers[i].noiseSettings); }
            elevationMinMax = new MinMax();
        }

        public float CalculateUnscaledElevation(Vector3 pointOnUnitSphere) {
            var firstLayerValue = 0f;
            var elevation = 0f;

            if (noiseFilters.Length > 0) {
                firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
                if (_settings.noiseLayers[0].enabled) elevation = firstLayerValue;
            }

            for (var i = 1; i < noiseFilters.Length; i++) {
                if (_settings.noiseLayers[i].enabled) {
                    var mask = _settings.noiseLayers[i].useFirstLayerAsMask ? firstLayerValue : 1f;
                    elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
                }
            }

            elevationMinMax.AddValue(elevation);
            
            return elevation;
        }

        public float GetScaledElevation(float unscaledElevation) {
            var elevation = Mathf.Max(0, unscaledElevation);
            elevation = _settings.radius * (1 + elevation);
            return elevation;
        }
    }
}