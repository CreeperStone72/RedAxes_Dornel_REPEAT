namespace PoissonDiscSampling {
    using DataTypes;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    // ReSharper disable once InconsistentNaming
    public class PDS_Test : MonoBehaviour {
        public SamplingSettings samplingSettings;
        public float displayRadius = 1f;

        private List<Vector2> _points;

        private void OnValidate() {
            if (samplingSettings != null) Subscribe(samplingSettings, () => _points = samplingSettings.SamplePoints());
            if (displayRadius <= 0) displayRadius = 0.001f;
        }

        private void OnDrawGizmos() {
            Gizmos.DrawWireCube(XYtoYZ(samplingSettings.regionCentre), XYtoYZ(samplingSettings.regionSize));

            if (_points == null) return;
            
            foreach (var point in _points) Gizmos.DrawSphere(XYtoYZ(point), displayRadius);
        }

        private static Vector3 XYtoYZ(Vector2 vector2) { return new Vector3(vector2.x, 0, vector2.y); }
        
        private static void Subscribe(UpdatableData data, Action callback) {
            data.OnValuesUpdated -= callback;
            data.OnValuesUpdated += callback;
        }
    }
}
