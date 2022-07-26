namespace ProceduralTreeGeneration {
    using UnityEngine;

    public class PyramidGenerator : MonoBehaviour {
        [SerializeField] private RegularPyramid pyramid;
        [SerializeField] private MeshFilter filter;
        [SerializeField] private MeshRenderer pyramidRenderer;

        private void DrawPyramid() {
            filter.mesh = ConeGenerator.BuildConeMesh(pyramid);
            pyramidRenderer.sharedMaterial = pyramid.material;
        }

        private void OnValidate() {
            pyramid.OnValidate();
            DrawPyramid();
        }
    }
}
