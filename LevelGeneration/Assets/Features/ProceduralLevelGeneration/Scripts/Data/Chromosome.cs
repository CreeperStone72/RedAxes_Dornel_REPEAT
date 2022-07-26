namespace ProceduralLevelGeneration.Data {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    
    [Serializable]
    public class Chromosome {
        public List<Room> rooms;
        private List<Mesh> _meshes;

        public Chromosome() {
            rooms = new List<Room>();
            _meshes = new List<Mesh>();
        }

        public Room this[int i] => rooms[i];

        public int Count => rooms.Count;

        /// <summary>
        /// Calculates the surface area by the mesh
        /// Original code by DMGregory
        /// SOURCE : https://gamedev.stackexchange.com/questions/165643/how-to-calculate-the-surface-area-of-a-mesh
        /// LAST ACCESSED : 26/06
        /// </summary>
        /// <returns>The level's area</returns>
        public float Area() {
            var mesh = ToMesh();
            var triangles = mesh.triangles;
            var vertices = mesh.vertices;

            var sum = 0.0;

            for (var i = 0; i < triangles.Length; i += 3) {
                var corner = vertices[triangles[i]];
                var a = vertices[triangles[i + 1]] - corner;
                var b = vertices[triangles[i + 2]] - corner;

                sum += Vector3.Cross(a, b).magnitude;
            }

            return (float) (sum / 2.0);
        }

        public float NarrowCount => rooms.Count(room => room.IsNarrow);

        public float TinyCount => rooms.Count(room => room.IsTiny);

        public void Add(Room room) {
            if (!IntersectsAny(room)) return;
            rooms.Add(room);
            _meshes.Add(room.ToMesh());
        }

        public Mesh ToMesh() {
            RefreshMeshes();
            var lmf = new LevelMeshFactory();
            lmf.SetVertices(_meshes);
            lmf.SetTriangles(rooms, _meshes);
            return lmf.ToMesh();
        }

        public Bounds GetRectBounds() {
            Vector3 min = Vector3.positiveInfinity, max = Vector3.negativeInfinity;

            foreach (var bounds in _meshes.Select(mesh => mesh.bounds)) {
                min = Vector3.Min(min, bounds.min);
                max = Vector3.Max(max, bounds.max);
            }

            return BoundsFactory.GetBounds(min, max);
        }

        public List<Mesh> ToMeshes() {
            RefreshMeshes();
            return _meshes;
        }

        private bool IntersectsAny(Room room) { return rooms.Count == 0 || rooms.Any(r => r.Intersects(room)); }

        private void RefreshMeshes() {
            var temp = new Room[rooms.Count];
            rooms.CopyTo(temp);
            
            _meshes.Clear();
            rooms.Clear();
            foreach (var room in temp) Add(room);
        }

        public override string ToString() { return $"[{ string.Join("\n", rooms.Select(room => room.ToString())) }]"; }
    }
}