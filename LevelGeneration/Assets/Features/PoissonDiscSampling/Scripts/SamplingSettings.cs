using System.Collections.Generic;
using ProceduralTerrainGeneration.Data;
using UnityEngine;

namespace PoissonDiscSampling {
    [CreateAssetMenu(menuName = "Norsevar/Poisson Disc Sampling/Sampling Settings")]
    public class SamplingSettings : UpdatableData {
        public float radius = 1f;
        public Vector2 regionCentre = Vector2.zero;
        public Vector2 regionSize = Vector2.one;
        public int rejectionSamples = 30;

        public List<Vector2> SamplePoints() {
            var points = PoissonDiscSampling.GeneratePoints(radius, regionSize, rejectionSamples);
            for (var i = 0; i < points.Count; i++) points[i] -= regionSize / 2 - regionCentre;
            return points;
        }

        #if UNITY_EDITOR
        
        protected override void OnValidate() {
            base.OnValidate();
            if (radius <= 0f) radius = 0.001f;
            if (rejectionSamples < 1) rejectionSamples = 1;
        }
        
        #endif
    }
}