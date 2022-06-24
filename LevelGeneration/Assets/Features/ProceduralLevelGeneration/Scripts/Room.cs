using UnityEngine;

// ReSharper disable InconsistentNaming
// ReSharper disable once CheckNamespace
namespace ProceduralLevelGeneration
{
    /// <summary>
    /// Data representation of a room as a five-tuple
    /// SOURCE : J. A. Brown, B. Lutfullin and P. Oreshin,
    /// "Procedural content generation of level layouts for Hotline Miami",
    /// 2017 9th Computer Science and Electronic Engineering (CEEC), 2017,
    /// pp. 106-111, doi: 10.1109/CEEC.2017.8101608.
    /// LAST ACCESSED : 24/06
    /// </summary>
    [System.Serializable]
    public class Room
    {
        /// <summary>
        /// Placement type of a room
        /// <br />
        /// O - Places the new room on top of current ones
        /// U - Places the new room under current ones
        /// </summary>
        public enum PlacementType { O, U }

        /// <summary>Starting x-coordinate of a room</summary>
        public int x;
        
        /// <summary>Starting y-coordinate of a room</summary>
        public int y;

        /// <summary>Length of the room</summary>
        public int l;
        
        /// <summary>Width of the room</summary>
        public int w;
        
        /// <summary>Placement type of a room</summary>
        public PlacementType t;

        public Room(int x, int y, int l, int w, PlacementType t)
        {
            this.x = x;
            this.y = y;
            this.l = l;
            this.w = w;
            this.t = t;
        }

        public int GetWidth() { return w; }

        public float GetArea() { return l * w; }
        
        public bool IsOnTop() { return t == PlacementType.O; }
        
        #region Mesh generation methods

            public Mesh ToMesh() { return new Mesh { vertices = BuildVertices(), triangles = BuildTriangles() }; }

            /// <summary>
            /// Generates the vertices for our mesh
            /// Original code by Catlike Coding (https://catlikecoding.com/unity/tutorials/procedural-grid/)
            /// LAST ACCESSED : 24/06
            /// Note : The Vector is (i, 0, j) instead of (i, j, 0) to make them flat 
            /// </summary>
            /// <returns></returns>
            private Vector3[] BuildVertices()
            {
                var vertices = new Vector3[(l + 1) * (w + 1)];
                
                for (int index = 0, j = y; j <= y + l; j++)
                    for (var i = x; i <= x + w; i++, index++)
                        vertices[index] = new Vector3(i, 0, j);

                return vertices;
            }

            /// <summary>
            /// Generates the triangles for our mesh
            /// Original code by Catlike Coding (https://catlikecoding.com/unity/tutorials/procedural-grid/)
            /// LAST ACCESSED : 24/06
            /// </summary>
            /// <returns></returns>
            private int[] BuildTriangles()
            {
                var triangles = new int[w * l * 6];

                for (int ti = 0, vi = 0, j = y; j < y + l; j++, vi++)
                {
                    for (var i = x; i < x + w; i++, ti += 6, vi++)
                    {
                        triangles[ti] = vi;
                        triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                        triangles[ti + 4] = triangles[ti + 1] = vi + w + 1;
                        triangles[ti + 5] = vi + w + 2;
                    }
                }

                return triangles;
            }

        #endregion
    }
}