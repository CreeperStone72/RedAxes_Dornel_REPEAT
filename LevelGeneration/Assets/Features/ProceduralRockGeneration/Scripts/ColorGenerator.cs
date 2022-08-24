namespace ProceduralRockGeneration {
    using UnityEngine;
    
    public class ColorGenerator {
        private ColorSettings settings;
        private Texture2D texture;
        private const int TextureResolution = 50;
        private INoiseFilter biomeNoiseFilter;
        
        public void UpdateSettings(ColorSettings settings) {
            this.settings = settings;
            if (texture == null || texture.height != settings.biomeColorSettings.biomes.Length) {
                texture = new Texture2D(TextureResolution * 2, settings.biomeColorSettings.biomes.Length, TextureFormat.RGBA32, false);
            }

            biomeNoiseFilter = NoiseFilterFactory.CreateNoiseFilter(settings.biomeColorSettings.noise);
        }

        public void UpdateElevation(MinMax elevationMinMax) {
            settings.material.SetVector("_elevationMinMax", new Vector4(elevationMinMax.Min, elevationMinMax.Max));
        }

        public float BiomePercentFromPoint(Vector3 pointOnUnitSphere) {
            var heightPercent = (pointOnUnitSphere.y + 1) / 2f;
            heightPercent += (biomeNoiseFilter.Evaluate(pointOnUnitSphere) - settings.biomeColorSettings.noiseOffset) * settings.biomeColorSettings.noiseStrength;
            var biomeIndex = 0f;
            var numBiomes = settings.biomeColorSettings.biomes.Length;
            var blendRange = settings.biomeColorSettings.blendAmount / 2f + .001f;

            for (var i = 0; i < numBiomes; i++) {
                var distance = heightPercent - settings.biomeColorSettings.biomes[i].startHeight;
                var weight = Mathf.InverseLerp(-blendRange, blendRange, distance);
                biomeIndex *= 1 - weight;
                biomeIndex += i * weight;
            }

            return biomeIndex / Mathf.Max(1, numBiomes - 1);
        }

        public void UpdateColors() {
            var colors = new Color[texture.width * texture.height];
            var colorIndex = 0;

            foreach (var biome in settings.biomeColorSettings.biomes) {
                for (var i = 0; i < TextureResolution * 2; i++) {
                    Color gradientColor;
                    
                    if (i < TextureResolution) { gradientColor = settings.oceanColor.Evaluate(i / (TextureResolution - 1)); }
                    else { gradientColor = biome.gradient.Evaluate((i - TextureResolution) / (TextureResolution - 1)); }
                    
                    var tintColor = biome.tint;
                    colors[colorIndex] = gradientColor * (1 - biome.tintPercent) + tintColor * biome.tintPercent;
                    colorIndex++;
                }
            }

            texture.SetPixels(colors);
            texture.Apply();
            settings.material.SetTexture("_texture", texture);
        }
    }
}