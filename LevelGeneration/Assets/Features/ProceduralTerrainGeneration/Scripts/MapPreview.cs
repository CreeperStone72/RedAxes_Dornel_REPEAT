namespace ProceduralTerrainGeneration {
    using Data;
    using Generators;
    using System;
    using UnityEngine;
    
    public class MapPreview : MonoBehaviour {
        public enum DrawMode { NoiseMap, Mesh, FalloffMap, }
        
        public bool autoUpdate;

        [Header("Preview")]
        public Renderer textureRenderer;
        public MeshFilter meshFilter;
        public MeshRenderer meshRenderer;

        [Space(5)]
        
        public DrawMode drawMode;

        [Header("Settings and data")]
        public MeshSettings meshSettings;
        public HeightMapSettings heightMapSettings;
        public TextureData textureData;

        [Space(5)]
        
        public Material terrainMaterial;
        
        [Space(5)]
        
        [Range(0, MeshSettings.NumSupportedLoDs - 1)] public int editorPreviewLOD;
        
        public void DrawMapInEditor() {
            textureData.ApplyToMaterial(terrainMaterial, heightMapSettings.MinHeight, heightMapSettings.MaxHeight);
            var heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.NumVertsPerLine, heightMapSettings, Vector2.zero);
            
            switch (drawMode) {
                case DrawMode.NoiseMap:   DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));                                 break;
                case DrawMode.Mesh:       DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD)); break;
                case DrawMode.FalloffMap: DrawTexture(TextureGenerator.TextureFromHeightMap(GenerateFalloffHeightMap()));                break;
                default:                  throw new ArgumentOutOfRangeException();
            }
        }

        private void DrawTexture(Texture2D texture) {
            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f;
            
            textureRenderer.gameObject.SetActive(true);
            meshFilter.gameObject.SetActive(false);
        }

        private void DrawMesh(MeshData meshData) {
            meshFilter.sharedMesh = meshData.CreateMesh();
            textureRenderer.gameObject.SetActive(false);
            meshFilter.gameObject.SetActive(true);
        }

        private void OnValuesUpdated() { if (!Application.isPlaying) DrawMapInEditor(); }

        private void OnTextureValuesUpdated() { textureData.ApplyToMaterial(terrainMaterial, heightMapSettings.MinHeight, heightMapSettings.MaxHeight); }

        private void OnValidate() {
            if (meshSettings != null)      Subscribe(meshSettings, OnValuesUpdated);
            if (heightMapSettings != null) Subscribe(heightMapSettings, OnValuesUpdated);
            if (textureData != null)       Subscribe(textureData, OnTextureValuesUpdated);
        }

        private static void Subscribe(UpdatableData data, Action callback) {
            data.OnValuesUpdated -= callback;
            data.OnValuesUpdated += callback;
        }

        private HeightMap GenerateFalloffHeightMap() { return new HeightMap(GenerateFalloffMap(), 0, 1); }
        
        private float[,] GenerateFalloffMap() { return FalloffGenerator.GenerateFalloffMap(meshSettings.NumVertsPerLine); }
    }
}
