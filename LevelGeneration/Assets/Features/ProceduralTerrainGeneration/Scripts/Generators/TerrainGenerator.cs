namespace ProceduralTerrainGeneration.Generators {
    using Data;
    using System.Collections.Generic;
    using UnityEngine;

    public class TerrainGenerator : MonoBehaviour {
        private const float MovementThreshold = 25f;
        private const float SqrMovementThreshold = MovementThreshold * MovementThreshold;
        
        public int colliderLODIndex;
        
        public Transform viewer;
        
        public Material mapMaterial;
        
        [Header("Settings and data")]
        public MeshSettings meshSettings;
        public HeightMapSettings heightMapSettings;
        public TextureData textureData;
        
        [Space(5)]
        
        public LODInfo[] detailLevels;

        private Vector2 _viewerPosition;
        private Vector2 _viewerPositionOld;
        
        private float _meshWorldSize;
        private int _chunksVisibleInViewDst;

        private readonly Dictionary<Vector2, TerrainChunk> _terrainChunks = new Dictionary<Vector2, TerrainChunk>();
        private readonly List<TerrainChunk> _visibleTerrainChunks = new List<TerrainChunk>();

        private void Start() {
            textureData.ApplyToMaterial(mapMaterial, heightMapSettings.MinHeight, heightMapSettings.MaxHeight);
            
            var maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
            _meshWorldSize = meshSettings.MeshWorldSize;
            _chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / _meshWorldSize);
            
            UpdateVisibleChunks();
        }

        private void Update() {
            var position = viewer.position;
            _viewerPosition = new Vector2(position.x, position.z);

            if (_viewerPosition != _viewerPositionOld) { _visibleTerrainChunks.ForEach(chunk => chunk.UpdateCollisionMesh()); }

            if ((_viewerPositionOld - _viewerPosition).sqrMagnitude > SqrMovementThreshold) {
                _viewerPositionOld = _viewerPosition;
                UpdateVisibleChunks();
            }
        }

        private void UpdateVisibleChunks() {
            var alreadyUpdatedChunkCoords = new HashSet<Vector2>();

            for (var i = _visibleTerrainChunks.Count - 1; i >= 0; i--) {
                alreadyUpdatedChunkCoords.Add(_visibleTerrainChunks[i].coord);
                _visibleTerrainChunks[i].UpdateTerrainChunk();
            }

            var currentChunkCoordX = Mathf.RoundToInt(_viewerPosition.x / _meshWorldSize);
            var currentChunkCoordY = Mathf.RoundToInt(_viewerPosition.y / _meshWorldSize);

            for (var yOffset = -_chunksVisibleInViewDst; yOffset <= _chunksVisibleInViewDst; yOffset++) {
                for (var xOffset = -_chunksVisibleInViewDst; xOffset <= _chunksVisibleInViewDst; xOffset++) {
                    var viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (alreadyUpdatedChunkCoords.Contains(viewedChunkCoord)) continue;
                    
                    if (_terrainChunks.ContainsKey(viewedChunkCoord)) _terrainChunks[viewedChunkCoord].UpdateTerrainChunk();
                    else {
                        var chunk = new TerrainChunk(
                                        viewedChunkCoord,
                                        heightMapSettings,
                                        meshSettings,
                                        detailLevels, 
                                        colliderLODIndex,
                                        transform,
                                        viewer,
                                        mapMaterial
                                    );
                        
                        _terrainChunks.Add(viewedChunkCoord, chunk);
                        chunk.OnVisibilityChanged += OnTerrainChunkVisibilityChanged;
                        chunk.Load();
                    }
                }
            }
        }

        private void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible) {
            if (isVisible) _visibleTerrainChunks.Add(chunk);
            else _visibleTerrainChunks.Remove(chunk);
        }
    }

    [System.Serializable]
    public struct LODInfo {
        [Range(0, MeshSettings.NumSupportedLoDs - 1)] public int lod;
        public float visibleDstThreshold;
            
        public float SqrVisibleDstThreshold => visibleDstThreshold * visibleDstThreshold;
    }
}