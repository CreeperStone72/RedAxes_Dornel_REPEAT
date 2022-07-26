namespace ProceduralLayoutGeneration {
    using DataTypes;
    using ProceduralTerrainGeneration.Data;
    using ProceduralTerrainGeneration.Generators;
    using System;
    using UnityEngine;

    public class ProceduralLayout : MonoBehaviour {
        public bool autoUpdate;
        
        [Header("Settings")]
        [SerializeField] private HeightMapSettings heightMapSettings;
        [SerializeField] private MeshSettings meshSettings;
        
        [Header("Preview")]
        [SerializeField] private Renderer textureRenderer;
        
        public void DrawMapInEditor() {
            var heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.NumVertsPerLine, heightMapSettings, Vector2.zero);
            var texture = TextureGenerator.TextureFromHeightMap(heightMap);
            
            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;
        }
        
        private void OnValuesUpdated() { if (!Application.isPlaying) DrawMapInEditor(); }
        
        private void OnValidate() {
            if (meshSettings != null)      Subscribe(meshSettings, OnValuesUpdated);
            if (heightMapSettings != null) Subscribe(heightMapSettings, OnValuesUpdated);
        }
        
        private static void Subscribe(UpdatableData data, Action callback) {
            data.OnValuesUpdated -= callback;
            data.OnValuesUpdated += callback;
        }
    }
}
