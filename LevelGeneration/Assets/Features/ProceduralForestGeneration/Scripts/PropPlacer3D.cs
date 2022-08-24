namespace ProceduralForestGeneration {
    using ProceduralPropPlacement;
    using System.Linq;
    using UnityEngine;

    [RequireComponent(typeof(PoissonCollapse))]
    public class PropPlacer3D : MonoBehaviour {
        public bool standaloneRun;
        
        [SerializeField] private float heightThreshold;
        [SerializeField] private MeshCollider terrainCollider;
        
        private void Start() { if (standaloneRun) PlaceProps(); }

        public void PlaceProps() {
            GetComponent<PoissonCollapse>().DrawPoints();
            AdjustHeights();
        }

        private void AdjustHeights() {
            var maxHeight = terrainCollider.sharedMesh.bounds.max.y;
            
            foreach (var child in GetComponentsInChildren<Transform>().Skip(1)) {
                var position = child.position;
                var childPosition = new Vector3(position.x, maxHeight, position.z);
                var ray = new Ray(childPosition, Vector3.down);

                childPosition.y = heightThreshold;

                if (terrainCollider.Raycast(ray, out var hit, 2f * maxHeight)) {
                    var height = hit.point.y;
                    if (height < heightThreshold) {
                        Destroy(child.gameObject);
                        continue;
                    }
                    childPosition.y = height;
                }

                child.position = childPosition;
            }
        }
    }
}