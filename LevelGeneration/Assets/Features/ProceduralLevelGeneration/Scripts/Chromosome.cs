using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace ProceduralLevelGeneration
{
    [Serializable]
    public class Chromosome
    {
        public List<Room> rooms;
        private readonly List<Mesh> _meshes;

        public Chromosome()
        {
            rooms = new List<Room>();
            _meshes = new List<Mesh>();
        }

        public void Add(Room room) { rooms.Add(room); }

        public List<Mesh> ToMeshes()
        {
            _meshes.Add(rooms[0].ToMesh());
            
            for (var current = 1; current < rooms.Count; current++)
            {
                _meshes.Add(rooms[current].ToMesh());

                for (var past = 0; past < current; past++)
                {
                    if (rooms[current].IsOnTop()) _meshes[past] = Reduce(rooms[past], rooms[current], _meshes[past], _meshes[current]);
                    else _meshes[current] = Reduce(rooms[current], rooms[past], _meshes[current], _meshes[past]);
                }
            }
            
            return _meshes;
        }

        #region Mesh-cutting methods

            private static Mesh Reduce(Room meshRoom, Room maskRoom, Mesh mesh, Mesh mask)
            {
                var overlap = GetOverlappingBounds(mesh.bounds, mask.bounds);
                return new Mesh { vertices = ReduceVertices(mesh.vertices, overlap), triangles = ReduceTriangles(meshRoom, maskRoom, mesh, mask) };
            }

            private static Vector3[] ReduceVertices(Vector3[] vertices, Bounds mask)
            {
                var reducedVertices = new List<Vector3>();

                foreach (var vertex in vertices)
                {
                    if (!mask.Contains(vertex)) reducedVertices.Add(vertex);
                    else if (Borders(mask, vertex)) reducedVertices.Add(vertex);
                }

                return reducedVertices.ToArray();
            }

            private static int[] ReduceTriangles(Room meshRoom, Room maskRoom, Mesh mesh, Mesh mask)
            {
                var reducedTriangles = new List<int>();

                for (var index = 0; index < mesh.vertices.Length - meshRoom.GetWidth() - 2; index++)
                {
                    if (IsQuadShared(meshRoom, maskRoom, mesh, mask, index)) continue;
                    reducedTriangles.AddRange(GetQuadTriangle(meshRoom, index));
                }

                return reducedTriangles.ToArray();
            }

        #endregion

        public float GetRoomArea(int index)
        {
            if (index > _meshes.Count) return -1;
            
            var area = rooms[index].GetArea();
            
            if (!rooms[index].IsOnTop())
            {
                for (var i = 0; i < index; i++)
                {
                    var overlap = GetOverlappingBounds(rooms[index].ToMesh().bounds, rooms[i].ToMesh().bounds);
                    area -= overlap.size.x * overlap.size.z;
                }
            }
            else
            {
                for (var i = index + 1; i < _meshes.Count; i++)
                {
                    if (!rooms[i].IsOnTop()) continue;
                    var overlap = GetOverlappingBounds(rooms[index].ToMesh().bounds, rooms[i].ToMesh().bounds);
                    area -= overlap.size.x * overlap.size.z;
                }
            }

            return area;
        }
        
        private static bool Borders(Bounds mask, Vector3 point)
        {
            var xExtrema = Equals(point.x, mask.min.x) || Equals(point.x, mask.max.x);
            var yExtrema = Equals(point.y, mask.min.y) || Equals(point.y, mask.max.y);
            var zExtrema = Equals(point.z, mask.min.z) || Equals(point.z, mask.max.z);

            return xExtrema || yExtrema || zExtrema;
        }

        private static bool Equals(float a, float b) { return Math.Abs(a - b) == 0; }

        private static Bounds GetOverlappingBounds(Bounds b1, Bounds b2)
        {
            var min = Vector3.Min(b1.min, b2.min);
            var max = Vector3.Max(b1.max, b2.max);

            var size = max - min;
            var center = min + (size / 2);

            return new Bounds(center, size);
        }

        private static Vector3[] GetQuad(Room room, Mesh mesh, int index)
        {
            return new []
            {
                mesh.vertices[index],
                mesh.vertices[index + 1],
                mesh.vertices[index + room.GetWidth() + 1],
                mesh.vertices[index + room.GetWidth() + 2],
            };
        }

        private static int[] GetQuadTriangle(Room room, int index)
        {
            return new []
            {
                index,
                index + room.GetWidth() + 1,
                index + 1,
                index + 1,
                index + room.GetWidth() + 1,
                index + room.GetWidth() + 2,
            };
        }

        private static bool IsQuadShared(Room meshRoom, Room maskRoom, Mesh mesh, Mesh mask, int index)
        {
            var meshQuad = GetQuad(meshRoom, mesh, index);
            return FindQuad(maskRoom, mask, meshQuad) != -1;
        }

        private static int FindQuad(Room room, Mesh mesh, Vector3[] reference)
        {
            for (var index = 0; index < mesh.vertices.Length - room.GetWidth() - 2; index++)
            {
                var quad = GetQuad(room, mesh, index);
                if (quad.SequenceEqual(reference)) return index;
            }

            return -1;
        }
    }
}