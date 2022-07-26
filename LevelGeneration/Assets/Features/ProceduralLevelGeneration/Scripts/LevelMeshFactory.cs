namespace ProceduralLevelGeneration {
    using Data;
    using DataTypes;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Utility;

    public class LevelMeshFactory {
        private readonly HashSet<Vector3> _vertices;
        private readonly HashSet<MeshSquare> _squares;
        
        public LevelMeshFactory() {
            _vertices = new HashSet<Vector3>();
            _squares = new HashSet<MeshSquare>();
        }

        public void SetVertices(List<Mesh> meshes) { meshes.SelectMany(mesh => mesh.vertices).ToList().ForEach(vertex => _vertices.Add(vertex)); }

        public void SetTriangles(List<Room> rooms, List<Mesh> meshes) {
            for (var index = 0; index < meshes.Count; index++) {
                var vertices = meshes[index].vertices;
                var width = rooms[index].Width;
                
                for (var i = 0; i < vertices.Length - width - 2; i++) {
                    int a = IndexOf(vertices[i]), b = IndexOf(vertices[i + 1]);
                    int c = IndexOf(vertices[i + width + 1]), d = IndexOf(vertices[i + width + 2]);
                    
                    _squares.Add(new MeshSquare(a, b, c, d, _vertices.ToArray()));
                }
            }
        }
        
        public Mesh ToMesh() { return MeshFactory.BuildMesh(_vertices.ToArray(), ToTriangles()); }

        private int[] ToTriangles() {
            var triangles = new int[_squares.Count * 6];
            var index = 0;

            foreach (var indices in _squares.Select(square => square.GetTriangleIndices())) {
                triangles[index    ] = indices[ index      % 6];
                triangles[index + 1] = indices[(index + 1) % 6];
                triangles[index + 2] = indices[(index + 2) % 6];
                triangles[index + 3] = indices[(index + 3) % 6];
                triangles[index + 4] = indices[(index + 4) % 6];
                triangles[index + 5] = indices[(index + 5) % 6];

                index += 6;
            }
            
            return triangles;
        }

        private int IndexOf(Vector3 vertex) { return _vertices.ToList().IndexOf(vertex); }
    }
}