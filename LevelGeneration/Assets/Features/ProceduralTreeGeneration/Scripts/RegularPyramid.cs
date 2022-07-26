namespace ProceduralTreeGeneration {
    using DataTypes;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class RegularPyramid {
        public Vector3 origin;
        public float baseRadius;
        public float innerRadius;
        public float height;
        public int baseSides;
        public Material material;

        public Face ToFace(float inner, float outer, float heightA, float heightB, int point, bool reverse = false) {
            var pA = GetPoint(inner, origin, heightB, point, baseSides);
            var pB = GetPoint(inner, origin, heightB, (point < baseSides - 1) ? point + 1 : 0, baseSides);
            var pC = GetPoint(outer, origin, heightA, (point < baseSides - 1) ? point + 1 : 0, baseSides);
            var pD = GetPoint(outer, origin, heightA, point, baseSides);

            var vertices = new List<Vector3> { pA, pB, pC, pD };
            var triangles = new List<int> { 0, 1, 2, 2, 3, 0 };
            var uvs = new List<Vector2> { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };
            
            if (reverse) vertices.Reverse();
            
            return new Face(vertices, triangles, uvs);
        }

        public void OnValidate() {
            if (baseRadius < 1f) baseRadius = 1f;
            if (innerRadius < 0f) innerRadius = 0f;
            if (baseSides < 3) baseSides = 3;
        }

        private static Vector3 GetPoint(float size, Vector3 origin, float height, int index, int pointsOnCircle) {
            var angle = 2f * index * Mathf.PI / pointsOnCircle;
            return origin + new Vector3(size * Mathf.Cos(angle), height, size * Mathf.Sin(angle));
        }
    }
}