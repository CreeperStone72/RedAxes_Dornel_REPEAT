namespace ProceduralTreeGeneration {
    using DataTypes;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public static class ConeGenerator {
        // Unused method which was used to calculate the points at the base of the cone
        public static List<Vector2> BuildCirclePoints(Vector2 centre, float radius, int pointsOnCircle) {
            var step = 2f * Mathf.PI / pointsOnCircle;
            var points = new List<Vector2>();
            
            for (var angle = 0f; angle < 2f * Mathf.PI; angle += step) {
                var x = radius * Mathf.Cos(angle) + centre.x;
                var y = radius * Mathf.Sin(angle) + centre.y;
                points.Add(new Vector2(x, y));
            } 
            
            return points;
        }
        
        /// <summary>
        /// Generates and builds the mesh for a regular pyramid (or "low-poly cone")
        /// Original code by Game Dev Guide, with the following changes by me
        /// - Builds a regular pyramid instead of a regular prism
        /// - Works with any regular polygon instead of only hexagons
        /// SOURCE : https://www.youtube.com/watch?v=EPaSmQ2vtek
        /// LAST ACCESSED : 17/07
        /// </summary>
        /// <param name="pyramid">The settings for the pyramid to build</param>
        /// <returns>The mesh of the cone</returns>
        public static Mesh BuildConeMesh(RegularPyramid pyramid) {
            var faces = DrawFaces(pyramid);
            return CombineFaces(faces);
        }

        private static List<Face> DrawFaces(RegularPyramid pyramid) {
            var faces = new List<Face>();

            // Top
            for (var point = 0; point < pyramid.baseSides; point++) {
                faces.Add(pyramid.ToFace(pyramid.innerRadius, pyramid.baseRadius, 0f, pyramid.height, point));
                // faces.Add(CreateFace(innerRadius, outerRadius, height / 2f, height / 2f, point, pointsOnCircle));
            }

            // Bottom
            for (var point = 0; point < pyramid.baseSides; point++) {
                faces.Add(pyramid.ToFace(pyramid.innerRadius, pyramid.baseRadius, 0f, 0f, point, true));
                //faces.Add(CreateFace(innerRadius, outerRadius, -height / 2f, -height / 2f, point, pointsOnCircle, true));
            }

            // Inner sides
            for (var point = 0; point < pyramid.baseSides; point++) {
                if (pyramid.innerRadius == 0f) break;
                faces.Add(pyramid.ToFace(pyramid.innerRadius, pyramid.innerRadius, pyramid.height, 0f, point));
                // faces.Add(CreateFace(innerRadius, innerRadius, height / 2f, -height / 2f, point, pointsOnCircle));
            }

            /*
            // Outer sides
            for (var point = 0; point < pointsOnCircle; point++) {
                faces.Add(CreateFace(outerRadius, outerRadius, height / 2f, -height / 2f, point, pointsOnCircle, true));
            }
            */
            
            return faces;
        }

        private static Mesh CombineFaces(List<Face> faces) {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uvs = new List<Vector2>();
            var mesh = new Mesh();

            for (var i = 0; i < faces.Count; i++) {
                vertices.AddRange(faces[i].Vertices);
                uvs.AddRange(faces[i].Uvs);

                var offset = 4 * i;

                triangles.AddRange(faces[i].Triangles.Select(triangle => triangle + offset));
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}
