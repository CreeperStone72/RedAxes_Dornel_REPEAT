using System.Linq;
using UnityEngine;

namespace Utility
{
    public static class MeshFactory
    {
        public static Mesh BuildMesh(Bounds b) { return BuildMesh((int) b.min.x, (int) b.min.z, (int) b.size.x, (int) b.size.z); }

        public static Mesh BuildMesh(Vector3[] vertices, int[] triangles)
        {
            return new Mesh
            {
                vertices = vertices,
                triangles = triangles,
            };
        }
        
        public static Mesh BuildMesh(int x, int y, int width, int length)
        {
            return new Mesh
            {
                vertices = BuildVertices(x, y, width, length),
                triangles = BuildTriangles(x, y, width, length),
            };
        }

        public static Mesh RemoveArea(Mesh mesh, Bounds area)
        {
            var vertices = mesh.vertices;
            var triangles = mesh.triangles;

            return new Mesh { vertices = vertices, triangles = triangles, };
        }

        #region Mesh construction methods

        /// <summary>
        /// Generates the vertices for our mesh
        /// Original code by Catlike Coding
        /// SOURCE : https://catlikecoding.com/unity/tutorials/procedural-grid/
        /// LAST ACCESSED : 24/06
        /// Note : The Vector is (i, 0, j) instead of (i, j, 0) to make them flat 
        /// </summary>
        /// <returns>An array of vertices for a length * width grid starting at (x, y)</returns>
        private static Vector3[] BuildVertices(int x, int y, int width, int length)
        {
            var vertices = new Vector3[(length + 1) * (width + 1)];
                
            for (int index = 0, j = y; j <= y + length; j++)
            {
                for (var i = x; i <= x + width; i++, index++) vertices[index] = new Vector3(i, 0, j);
            }

            return vertices;
        }

        /// <summary>
        /// Generates the triangles for our mesh
        /// Original code by Catlike Coding
        /// SOURCE : https://catlikecoding.com/unity/tutorials/procedural-grid/
        /// LAST ACCESSED : 24/06
        /// </summary>
        /// <returns>An array of indices for a length * grid starting at (x, y)</returns>
        private static int[] BuildTriangles(int x, int y, int width, int length)
        {
            var triangles = new int[width * length * 6];

            for (int ti = 0, vi = 0, j = y; j < y + length; j++, vi++)
            {
                for (var i = x; i < x + width; i++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + width + 1;
                    triangles[ti + 5] = vi + width + 2;
                }
            }

            return triangles;
        }

        #endregion

        /// <summary>
        /// Deletes a specific triangle from a mesh without 
        /// Original code by TheJetstream, with a solution by OlivierHoel and edited by myself to create a new mesh
        /// SOURCE : https://answers.unity.com/questions/1669063/deleting-single-triangle-from-mesh.html
        /// LAST ACCESSED : 25/06
        /// </summary>
        /// <param name="mesh">The mesh to edit</param>
        /// <param name="id">The id of the triangle to remove</param>
        /// <returns>The modified mesh</returns>
        private static Mesh DeleteTriangle(Mesh mesh, int id)
        {
            var vertices = mesh.vertices;
            var triangles = mesh.triangles.ToList();
 
            // Modification by OlivierHoel : RemoveAt already changes the id-th element
            triangles.RemoveAt(id);
            triangles.RemoveAt(id);
            triangles.RemoveAt(id);

            return new Mesh { vertices = vertices, triangles = Enumerable.ToArray(triangles) };
        }
    }
}