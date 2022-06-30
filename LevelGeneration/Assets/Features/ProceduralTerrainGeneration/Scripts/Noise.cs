namespace ProceduralTerrainGeneration {
    using UnityEngine;
    
    public static class Noise {
        public enum NormalizeMode { Local, Global, }
        
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCentre) {
            var noiseMap = new float[mapWidth, mapHeight];

            var prng = new System.Random(settings.seed);
            var octaveOffsets = new Vector2[settings.octaves];

            float maxGlobalHeight = 0;
            float amplitude = 1;

            for (var i = 0; i < settings.octaves; i++) {
                var offsetX = prng.Next(-100000, 100000) + settings.offset.x + sampleCentre.x;
                var offsetY = prng.Next(-100000, 100000) - settings.offset.y + sampleCentre.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                maxGlobalHeight += amplitude;
                amplitude *= settings.persistance;
            }

            if (settings.scale <= 0) settings.scale = 0.0001f;

            float minLocalHeight = float.MaxValue, maxLocalHeight = float.MinValue;
            float halfWidth = mapWidth / 2f, halfHeight = mapHeight / 2f;
            
            for (var y = 0; y < mapHeight; y++) {
                for (var x = 0; x < mapWidth; x++) {
                    amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;
                    
                    for (var i = 0; i < settings.octaves; i++) {
                        var sampleX = (x - halfWidth + octaveOffsets[i].x) / settings.scale * frequency;
                        var sampleY = (y - halfHeight + octaveOffsets[i].y) / settings.scale * frequency;

                        var perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= settings.persistance;
                        frequency *= settings.lacunarity;
                    }

                    if (noiseHeight > maxLocalHeight) maxLocalHeight = noiseHeight;
                    if (noiseHeight < minLocalHeight) minLocalHeight = noiseHeight;

                    noiseMap[x, y] = noiseHeight;

                    if (settings.normalizeMode != NormalizeMode.Global) continue;
                    
                    var normalizedHeight = (noiseMap[x, y] + 1) / (2f * maxGlobalHeight / 1.75f);
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }

            if (settings.normalizeMode != NormalizeMode.Local) return noiseMap;

            for (var y = 0; y < mapHeight; y++) {
                for (var x = 0; x < mapWidth; x++) noiseMap[x, y] = Mathf.InverseLerp(minLocalHeight, maxLocalHeight, noiseMap[x, y]);
            }

            return noiseMap;
        }
    }

    [System.Serializable]
    public class NoiseSettings {
        public Noise.NormalizeMode normalizeMode;

        public float scale = 50;

        public int octaves = 6;
        [Range(0, 1)] public float persistance = .6f;
        public float lacunarity = 2;

        public int seed;
        public Vector2 offset;

        public void ValidateValues() {
            scale = Mathf.Max(scale, 0.01f);
            octaves = Mathf.Max(octaves, 1);
            persistance = Mathf.Clamp01(persistance);
            lacunarity = Mathf.Max(lacunarity, 1);
        }
    }
}
