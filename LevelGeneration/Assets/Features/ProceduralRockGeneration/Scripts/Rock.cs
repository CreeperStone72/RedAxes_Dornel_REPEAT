namespace ProceduralRockGeneration {
    using UnityEngine;
    using Utility;

    public class Rock : MonoBehaviour {
        public bool autoUpdate = true;
        [Range(2, 256)] public int resolution = 10;

        public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back, };

        public FaceRenderMask faceRenderMask;

        [HideInInspector] public ShapeSettings shapeSettings;
        [HideInInspector] public ColorSettings colorSettings;

        [HideInInspector] public bool shapeSettingsFoldout;
        [HideInInspector] public bool colorSettingsFoldout;

        private readonly ShapeGenerator _shapeGenerator = new ShapeGenerator();
        private readonly ColorGenerator _colorGenerator = new ColorGenerator();
        
        [SerializeField, HideInInspector] private MeshFilter[] meshFilters;
        private RockFace[] _rockFaces;

        private void Initialize() {
            _shapeGenerator.UpdateSettings(shapeSettings);
            _colorGenerator.UpdateSettings(colorSettings);
            
            if (meshFilters == null || meshFilters.Length == 0) meshFilters = new MeshFilter[6];
            _rockFaces = new RockFace[6];

            Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back, };

            for (var i = 0; i < 6; i++) {
                if (meshFilters[i] == null) {
                    var meshObj = new GameObject($"Face {i}");
                    meshObj.transform.parent = transform;

                    meshObj.AddComponent<MeshRenderer>();
                    meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                    meshFilters[i].sharedMesh = new Mesh();
                }

                meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colorSettings.material;

                _rockFaces[i] = new RockFace(_shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
                var renderFace = faceRenderMask == FaceRenderMask.All || (int) faceRenderMask - 1 == i;
                meshFilters[i].gameObject.SetActive(renderFace);
            }
        }

        public void GenerateRock(bool randomize = false) {
            if (randomize) foreach (var layer in shapeSettings.noiseLayers) layer.noiseSettings.Settings.centre = RandomCenter();
            Initialize();
            GenerateMesh();
            GenerateColors();
        }

        public void OnShapeSettingsUpdated() {
            if (!autoUpdate) return;
            Initialize();
            GenerateMesh();
        }

        public void OnColorSettingsUpdated() {
            if (!autoUpdate) return;
            Initialize();
            GenerateColors();
        }

        private void GenerateMesh() {
            for (var i = 0; i < 6; i++) { if (meshFilters[i].gameObject.activeSelf) _rockFaces[i].ConstructMesh(); }
            _colorGenerator.UpdateElevation(_shapeGenerator.elevationMinMax);
        }

        private void GenerateColors() {
            _colorGenerator.UpdateColors();
            for (var i = 0; i < 6; i++) { if (meshFilters[i].gameObject.activeSelf) _rockFaces[i].UpdateUVs(_colorGenerator); }
        }

        private static Vector3 RandomCenter() {
            var x = RandUtils.Rand(-1000000, 1000000);
            var y = RandUtils.Rand(-1000000, 1000000);
            var z = RandUtils.Rand(-1000000, 1000000);
            return new Vector3(x, y, z);
        }
    }
}