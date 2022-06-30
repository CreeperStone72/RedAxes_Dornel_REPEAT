namespace ProceduralTerrainGeneration.Data {
    using System;
    using UnityEngine;
    
    [CreateAssetMenu(menuName = "Norsevar/Procedural Terrain Generation/Texture Data")]
    public class TextureData : UpdatableData {
        private const int MaxLayers = 8;
        
        [Space(5)]
        
        //public enum DrawMode { Tint, Texture, TintedTexture, }

        //public DrawMode drawMode;
        
        public Layer[] layers;
        
        public void ApplyToMaterial(Material material, float minHeight, float maxHeight) {
            for (var i = 0; i < layers.Length; i++) {
                var height = LerpHeight(minHeight, maxHeight, layers[i].startHeight);
                material.SetFloat($"Height{i + 1}", height * layers[i].blendStrength);

                SetTintedTextureMaterial(material, i);
                
                /*
                var tint = layers[i].tint * layers[i].tintStrength;
                
                switch (drawMode) {
                    case DrawMode.Tint: SetTintMaterial(material, tint, i); break;
                    case DrawMode.Texture: SetTextureMaterial(material, tint, i); break;
                    case DrawMode.TintedTexture: SetTintedTextureMaterial(material, i); break;
                    default: throw new ArgumentOutOfRangeException();
                }
                */
            }
        }

        //private static void SetTintMaterial(Material material, Color tint, int i) { material.SetColor($"Color{i + 1}", tint); }

        /*
        private void SetTextureMaterial(Material material, Color fallback, int i) {
            if (layers[i].texture != null) {
                material.SetTexture($"Texture{i + 1}", layers[i].texture);
                material.SetFloat($"Blend{i + 1}", layers[i].blendStrength);
            } else material.SetColor($"Color{i + 1}", fallback);
        }
        */

        private void SetTintedTextureMaterial(Material material, int i) {
            material.SetTexture($"Texture{i + 1}", layers[i].texture);
            material.SetFloat($"TextureScale{i + 1}", layers[i].textureScale);
            material.SetColor($"Tint{i + 1}", layers[i].tint);
            material.SetFloat($"TintStrength{i + 1}", layers[i].tintStrength);
            material.SetFloat($"BlendStrength{i + 1}", layers[i].blendStrength);
        }

        protected override void OnValidate() {
            base.OnValidate();

            if (layers.Length > MaxLayers) {
                var temp = new Layer[MaxLayers];
                for (var i = 0; i < MaxLayers; i++) temp[i] = layers[i];
                layers = temp;
            }
        }

        private static float LerpHeight(float minHeight, float maxHeight, float value) { return Mathf.Lerp(minHeight, maxHeight, value); }
    }

    [Serializable]
    public class Layer {
        public Texture2D texture;
        public Color tint;
        [Range(0, 1)] public float tintStrength;
        [Range(0, 1)] public float startHeight;
        [Range(0, 1)] public float blendStrength;
        public float textureScale;
    }
}