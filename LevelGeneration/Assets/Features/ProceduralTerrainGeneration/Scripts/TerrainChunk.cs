namespace ProceduralTerrainGeneration { 
    using Data;
    using Generators;
    using UnityEngine;
    
    using TDR = ThreadedDataRequester;
    
    public class TerrainChunk {
        private const float ColliderGenerationDistanceThreshold = 5f;
        public event System.Action<TerrainChunk, bool> OnVisibilityChanged;
        
        public Vector2 coord;
        
        private readonly GameObject _meshObject;
        private readonly Vector2 _sampleCentre;
        private Bounds _bounds;

        private readonly MeshRenderer _meshRenderer;
        private readonly MeshFilter _meshFilter;
        private readonly MeshCollider _meshCollider;

        private readonly LODInfo[] _details;
        private readonly LODMesh[] _lodMeshes;
        private readonly int _colliderLOD;
        
        private HeightMap _heightMap;
        private bool _heightMapReceived;

        private int _previousLODIndex = -1;

        private bool _hasSetCollider;

        private readonly float _maxViewDst;

        private readonly HeightMapSettings _hms;
        private readonly MeshSettings _ms;

        private readonly Transform _viewer;
        
        public TerrainChunk(Vector2 coord, HeightMapSettings hms, MeshSettings ms, LODInfo[] details, int colliderLOD, Transform p, Transform v, Material m) {
            this.coord = coord;
            _details = details;
            _colliderLOD = colliderLOD;
            _hms = hms;
            _ms = ms;
            _viewer = v;
            
            _sampleCentre = coord * ms.MeshWorldSize / ms.meshScale;
            
            var position = coord * ms.MeshWorldSize;
            _bounds = new Bounds(position, Vector2.one * ms.MeshWorldSize);

            _meshObject = new GameObject($"Terrain Chunk ({position.x} ; {position.y})");
            _meshRenderer = _meshObject.AddComponent<MeshRenderer>();
            _meshRenderer.material = m;
            _meshFilter = _meshObject.AddComponent<MeshFilter>();
            _meshCollider = _meshObject.AddComponent<MeshCollider>();
            
            _meshObject.transform.position = new Vector3(position.x, 0, position.y);
            _meshObject.transform.parent = p;
            SetVisible(false);

            _lodMeshes = new LODMesh[details.Length];

            for (var i = 0; i < details.Length; i++) {
                _lodMeshes[i] = new LODMesh(details[i].lod);
                _lodMeshes[i].UpdateCallback += UpdateTerrainChunk;

                if (i == colliderLOD) _lodMeshes[i].UpdateCallback += UpdateCollisionMesh;
            }

            _maxViewDst = details[details.Length - 1].visibleDstThreshold;
        }

        public void Load() { TDR.RequestData(() => HeightMapGenerator.GenerateHeightMap(_ms.NumVertsPerLine, _hms, _sampleCentre), OnHeightMapReceived); }

        private void OnHeightMapReceived(object heightMapObject) {
            _heightMap = (HeightMap) heightMapObject;
            _heightMapReceived = true;
            UpdateTerrainChunk();
        }

        private void OnMeshDataReceived(MeshData meshData) { _meshFilter.mesh = meshData.CreateMesh(); }

        private Vector2 ViewerPosition => new Vector2(_viewer.position.x, _viewer.position.z);

        public void UpdateTerrainChunk() {
            if (!_heightMapReceived) return;
            
            var viewerDstFromNearestEdge = Mathf.Sqrt(_bounds.SqrDistance(ViewerPosition));

            var wasVisible = IsVisible();
            var visible = viewerDstFromNearestEdge <= _maxViewDst;

            if (visible) {
                var lodIndex = 0;

                for (var i = 0; i < _details.Length - 1; i++) {
                    if (viewerDstFromNearestEdge > _details[i].visibleDstThreshold) lodIndex = i + 1;
                    else break;
                }

                if (lodIndex != _previousLODIndex) {
                    var lodMesh = _lodMeshes[lodIndex];
                        
                    if (lodMesh.hasMesh) {
                        _previousLODIndex = lodIndex;
                        _meshFilter.mesh = lodMesh.mesh;
                    } else if (!lodMesh.hasRequestedMesh) lodMesh.RequestMesh(_heightMap, _ms);
                }
            }

            if (wasVisible == visible) return;
                
            SetVisible(visible);
            OnVisibilityChanged?.Invoke(this, visible);
        }

        public void UpdateCollisionMesh() {
            if (_hasSetCollider) return;
            
            var sqrDstFromViewerToEdge = _bounds.SqrDistance(ViewerPosition);

            if (sqrDstFromViewerToEdge < _details[_colliderLOD].SqrVisibleDstThreshold) {
                if (!_lodMeshes[_colliderLOD].hasRequestedMesh) {
                    _lodMeshes[_colliderLOD].RequestMesh(_heightMap, _ms);
                }
            }

            if (!(sqrDstFromViewerToEdge < ColliderGenerationDistanceThreshold * ColliderGenerationDistanceThreshold)) return;
            if (!_lodMeshes[_colliderLOD].hasMesh) return;
                    
            _meshCollider.sharedMesh = _lodMeshes[_colliderLOD].mesh;
            _hasSetCollider = true;
        }

        private void SetVisible(bool visible) { _meshObject.SetActive(visible); }

        private bool IsVisible() { return _meshObject.activeSelf; }
    }

    internal class LODMesh {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        private readonly int _lod;
        public event System.Action UpdateCallback;

        public LODMesh(int lod) { _lod = lod; }

        public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings) {
            hasRequestedMesh = true;
            ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, _lod), OnMeshDataReceived);
        }

        private void OnMeshDataReceived(object meshDataObject) {
            mesh = ((MeshData) meshDataObject).CreateMesh();
            hasMesh = true;
            UpdateCallback?.Invoke();
        }
    }
}