namespace ProceduralTerrainGeneration.Generators {
    using UnityEngine;
    
    public static class TextureGenerator {
        // ReSharper disable once MemberCanBePrivate.Global
        public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height) {
            var texture = new Texture2D(width, height) { filterMode = FilterMode.Point, wrapMode = TextureWrapMode.Clamp };
            texture.SetPixels(colorMap);
            texture.Apply();
            return texture;
        }

        public static Texture2D TextureFromHeightMap(HeightMap heightMap) {
            var width = heightMap.values.GetLength(0);
            var height = heightMap.values.GetLength(1);

            var colorMap = new Color[width * height];

            for (var y = 0; y < height; y++) {
                for (var x = 0; x < width; x++) {
                    var value = Mathf.InverseLerp(heightMap.minValue, heightMap.maxValue, heightMap.values[x, y]);
                    colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, value);
                }
            }

            return TextureFromColorMap(colorMap, width, height);
        }
    }
}
