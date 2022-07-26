namespace DataTypes {
    using System.Collections.Generic;
    using UnityEngine;
    
    public struct Face {
        public List<Vector3> Vertices { get; }
        public List<int> Triangles { get; }
        public List<Vector2> Uvs { get; }

        public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs) {
            Vertices = vertices;
            Triangles = triangles;
            Uvs = uvs;
        }
    }
}