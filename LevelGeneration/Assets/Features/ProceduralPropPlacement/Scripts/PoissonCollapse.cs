using System;

namespace ProceduralPropPlacement {
    using PoissonDiscSampling;
    using ProceduralRockGeneration;
    using ProceduralTreeGeneration;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    using WaveFunctionCollapse;

    [RequireComponent(typeof(RockGenerator), typeof(TreeGenerator))]
    public class PoissonCollapse : MonoBehaviour {
        public bool standaloneRun;
        
        public Transform cameraTransform;

        public ConstrainedTile skippedTile;

        public UnityEvent[] spawnProp;
        
        [Header("Poisson Disc Sampling settings")]
        public SamplingSettings samplingSettings;
        public float displayRadius = 1f;
        
        private List<Vector2> _points;

        [Header("Wave Function Collapse settings")]
        public List<ConstrainedTile> possibleValues;

        private CollapsingGrid _grid;

        private void Awake() {
            var width = (int) samplingSettings.regionSize.x;
            var height = (int) samplingSettings.regionSize.y;

            var cellSize = possibleValues.Where(v => v.objectRadius != 0)
                                             .Aggregate(float.MaxValue, (cur, v) => Mathf.Min(cur, v.objectRadius));
            
            samplingSettings.radius = cellSize;
            
            _points = samplingSettings.SamplePoints();

            _grid = new CollapsingGrid(width, height, cellSize, possibleValues.ToArray(), samplingSettings.Origin) { useDebug = false };
        }

        private void Start() {
            if (standaloneRun) {
                CenterCamera();
                DrawPoints();
            }
        }

        private void CenterCamera() {
            var position = cameraTransform.position;
            position.x = samplingSettings.regionCentre.x;
            position.z = samplingSettings.regionCentre.y;
            cameraTransform.position = position;
        }

        public void DrawPoints() {
            foreach (var point in _points) {
                var value = _grid.RandomSetAndCollapse(point);
                if (value == skippedTile) continue;
                CreateProp(point, value);
            }
        }

        private void CreateProp(Vector2 worldPosition, ConstrainedTile value) {
            var propPosition = new Vector3(worldPosition.x, 0, worldPosition.y);

            for (var i = 0; i < possibleValues.Count; i++) {
                var tile = possibleValues[i];
                
                if (tile == skippedTile) continue;
                if (tile != value) continue;
                
                spawnProp[i - 1].Invoke();
            }

            //var prop = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            var prop = value.tileType switch {
                "Rock" => GetComponent<RockGenerator>().rocks.Last(),
                "Tree" => GetComponent<TreeGenerator>().trees.Last(),
                _ => throw new IndexOutOfRangeException()
            };

            prop.name = value.tileType;
            prop.transform.position = propPosition;
            prop.transform.parent = transform;
            
            // PDS display with WFC colors instead of actual props
            //prop.transform.localScale = Vector3.one * displayRadius;
            //prop.GetComponent<MeshRenderer>().material.color = value.tileColor;
        }
    }
}