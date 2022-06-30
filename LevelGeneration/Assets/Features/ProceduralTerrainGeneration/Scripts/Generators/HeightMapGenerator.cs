namespace ProceduralTerrainGeneration.Generators {
    using Data;
    using UnityEngine;
    
    public static class HeightMapGenerator {
        public static HeightMap GenerateHeightMap(int size, HeightMapSettings hms, Vector2 v2) { return GenerateHeightMap(size, size, hms, v2); }
        
        public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre) {
            var values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCentre);
            var heightCurveThreadsafe = new AnimationCurve(settings.heightCurve.keys);

            float minValue = float.MaxValue, maxValue = float.MinValue;

            var falloffMap = new float[width,height];
            if (settings.useFalloff) falloffMap = FalloffGenerator.GenerateFalloffMap(width);

            for (var i = 0; i < width; i++) {
                for (var j = 0; j < height; j++) {
                    if (settings.useFalloff) values[i, j] -= falloffMap[i, j];
                    
                    values[i, j] *= heightCurveThreadsafe.Evaluate(values[i, j]) * settings.heightMultiplier;

                    if (values[i, j] > maxValue) maxValue = values[i, j];
                    if (values[i, j] < minValue) minValue = values[i, j];
                }
            }

            return new HeightMap(values, minValue, maxValue);
        }
    }
    
    public struct HeightMap {
        public readonly float[,] values;
        public readonly float minValue;
        public readonly float maxValue;

        public HeightMap(float[,] values, float minValue, float maxValue) {
            this.values = values;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }
}