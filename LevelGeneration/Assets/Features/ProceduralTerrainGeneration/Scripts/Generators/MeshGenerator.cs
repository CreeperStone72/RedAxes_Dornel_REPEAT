namespace ProceduralTerrainGeneration.Generators {
    using Data;
    using UnityEngine;
    
    public static class MeshGenerator {
        public static MeshData GenerateTerrainMesh(float[,] heightMap, MeshSettings meshSettings, int levelOfDetail) {
            var skipIncrement = levelOfDetail == 0 ? 1 : levelOfDetail * 2;
            var vertsPerLine = meshSettings.NumVertsPerLine;

            var topLeft = new Vector2(-1, 1) * meshSettings.MeshWorldSize / 2f;

            var meshData = new MeshData(vertsPerLine, skipIncrement, meshSettings.useFlatShading);

            var vertexIndicesMap = new int[vertsPerLine][];
            for (var index = 0; index < vertsPerLine; index++) vertexIndicesMap[index] = new int[vertsPerLine];

            var meshVertexIndex = 0;
            var outOfMeshVertexIndex = -1;

            bool IsOutOfMesh(int x, int y) => 0 == x || x == vertsPerLine - 1 || 0 == y || y == vertsPerLine - 1;

            bool IsSkipped(int x, int y) => 2 < x && x < vertsPerLine - 3 && 2 < y && y < vertsPerLine - 3 &&
                                            ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

            for (var y = 0; y < vertsPerLine; y++) {
                for (var x = 0; x < vertsPerLine; x++) {
                    if (IsOutOfMesh(x, y)) {
                        vertexIndicesMap[x][y] = outOfMeshVertexIndex;
                        outOfMeshVertexIndex--;
                    } else if (!IsSkipped(x, y)) {
                        vertexIndicesMap[x][y] = meshVertexIndex;
                        meshVertexIndex++;
                    }
                }
            }

            for (var y = 0; y < vertsPerLine; y++) {
                for (var x = 0; x < vertsPerLine; x++) {
                    if (IsSkipped(x, y)) continue;
                    
                    var isMeshEdge       = y == 1 || y == vertsPerLine - 2 || x == 1 || x == vertsPerLine - 2 && !IsOutOfMesh(x, y);
                    var isMain           = (x - 2) % skipIncrement == 0 && (y - 2) % skipIncrement == 0 && !IsOutOfMesh(x, y) && !isMeshEdge;
                    var isEdgeConnection = (y == 2 || y == vertsPerLine - 3 || x == 2 || x == vertsPerLine - 3) && !IsOutOfMesh(x, y) && !isMeshEdge && !isMain;
                        
                    var vertexIndex = vertexIndicesMap[x][y];
                    var percent = new Vector2(x - 1, y - 1) / (vertsPerLine - 3);
                    
                    var vertexPosition2D = topLeft + new Vector2(percent.x, -percent.y) * meshSettings.MeshWorldSize;
                    var height = heightMap[x, y];

                    if (isEdgeConnection) {
                        var isVertical = x == 2 || x == vertsPerLine - 3;
                        var dstToMainVertexA = (isVertical ? y - 2 : x - 2) % skipIncrement;
                        var dstToMainVertexB = skipIncrement - dstToMainVertexA;
                        var dstPercentFromAToB = dstToMainVertexA / (float) skipIncrement;

                        var heightMainVertexA = heightMap[isVertical ? x : x - dstToMainVertexA, isVertical ? y - dstToMainVertexA : y];
                        var heightMainVertexB = heightMap[isVertical ? x : x + dstToMainVertexB, isVertical ? y + dstToMainVertexB : y];

                        height = heightMainVertexA * (1 - dstPercentFromAToB) + heightMainVertexB * dstPercentFromAToB;
                    }
                        
                    meshData.AddVertex(new Vector3(vertexPosition2D.x, height, vertexPosition2D.y), percent, vertexIndex);

                    var createTriangle = x < vertsPerLine - 1 && y < vertsPerLine - 1 && (!isEdgeConnection || (x != 2 && y != 2));

                    if (!createTriangle) continue;
                    
                    var currentIncrement = isMain && x != vertsPerLine - 3 && y != vertsPerLine - 3 ? skipIncrement : 1; 
                            
                    var a = vertexIndicesMap[x                   ][y                   ];
                    var b = vertexIndicesMap[x + currentIncrement][y                   ];
                    var c = vertexIndicesMap[x                   ][y + currentIncrement];
                    var d = vertexIndicesMap[x + currentIncrement][y + currentIncrement];
                    
                    meshData.AddTriangle(a, d, c);
                    meshData.AddTriangle(d, a, b);
                }
            }
            
            meshData.ProcessMesh();

            return meshData;
        }
    }

    public class MeshData {
        private Vector3[] _vertices;
        private readonly int[] _triangles;
        private Vector2[] _uvs;
        private Vector3[] _bakedNormals;

        private readonly Vector3[] _outOfMeshVertices;
        private readonly int[] _outOfMeshTriangles;

        private int _triangleIndex;
        private int _outOfMeshTriangleIndex;

        private readonly bool _useFlatShading;

        public MeshData(int numVertsPerLine, int skipIncrement, bool useFlatShading) {
            _useFlatShading = useFlatShading;

            var numMeshEdgeVertices       = (numVertsPerLine - 2) * 4 - 4;
            var numEdgeConnectionVertices = (skipIncrement - 1) * (numVertsPerLine - 5) / skipIncrement * 4;
            var numMainVerticesPerLine    = (numVertsPerLine - 5) / skipIncrement + 1;
            var numMainVertices           = numMainVerticesPerLine * numMainVerticesPerLine;
            
            _vertices = new Vector3[numMeshEdgeVertices + numEdgeConnectionVertices + numMainVertices];
            _uvs = new Vector2[_vertices.Length];

            var numMeshEdgeTriangles = 8 * (numVertsPerLine - 4);
            var numMainTriangles     = (numMainVerticesPerLine - 1) * (numMainVerticesPerLine - 1) * 2;
            
            _triangles          = new int[(numMeshEdgeTriangles + numMainTriangles) * 3];
            _outOfMeshVertices  = new Vector3[numVertsPerLine * 4 - 4];
            _outOfMeshTriangles = new int[24 * (numVertsPerLine - 2)];
        }

        public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex) {
            if (vertexIndex < 0) _outOfMeshVertices[-vertexIndex - 1] = vertexPosition;
            else {
                _vertices[vertexIndex] = vertexPosition;
                _uvs[vertexIndex] = uv;
            }
        }

        public void AddTriangle(int a, int b, int c) {
            if (a < 0 || b < 0 || c < 0) {
                _outOfMeshTriangles[_outOfMeshTriangleIndex] = a;
                _outOfMeshTriangles[_outOfMeshTriangleIndex + 1] = b;
                _outOfMeshTriangles[_outOfMeshTriangleIndex + 2] = c;
                _outOfMeshTriangleIndex += 3;
            } else {
                _triangles[_triangleIndex] = a;
                _triangles[_triangleIndex + 1] = b;
                _triangles[_triangleIndex + 2] = c;
                _triangleIndex += 3;
            }
        }

        private Vector3[] CalculateNormals() {
            var vertexNormals = new Vector3[_vertices.Length];
            var triangleCount = _triangles.Length / 3;

            for (var i = 0; i < triangleCount; i++) {
                var normalTriangleIndex = i * 3;
                var vertexIndexA = _triangles[normalTriangleIndex];
                var vertexIndexB = _triangles[normalTriangleIndex + 1];
                var vertexIndexC = _triangles[normalTriangleIndex + 2];

                var triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
                vertexNormals[vertexIndexA] += triangleNormal;
                vertexNormals[vertexIndexB] += triangleNormal;
                vertexNormals[vertexIndexC] += triangleNormal;
            }
            
            var borderTriangleCount = _outOfMeshTriangles.Length / 3;

            for (var i = 0; i < borderTriangleCount; i++) {
                var normalTriangleIndex = i * 3;
                var vertexIndexA = _outOfMeshTriangles[normalTriangleIndex];
                var vertexIndexB = _outOfMeshTriangles[normalTriangleIndex + 1];
                var vertexIndexC = _outOfMeshTriangles[normalTriangleIndex + 2];

                var triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);

                if (vertexIndexA >= 0) vertexNormals[vertexIndexA] += triangleNormal;
                if (vertexIndexB >= 0) vertexNormals[vertexIndexB] += triangleNormal;
                if (vertexIndexC >= 0) vertexNormals[vertexIndexC] += triangleNormal;
            }
            

            for (var i = 0; i < vertexNormals.Length; i++) vertexNormals[i].Normalize();

            return vertexNormals;
        }

        private Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC) {
            var pointA = indexA < 0 ? _outOfMeshVertices[-indexA - 1] : _vertices[indexA];
            var pointB = indexB < 0 ? _outOfMeshVertices[-indexB - 1] : _vertices[indexB];
            var pointC = indexC < 0 ? _outOfMeshVertices[-indexC - 1] : _vertices[indexC];

            return Vector3.Cross(pointB - pointA, pointC - pointA).normalized;
        }

        public void ProcessMesh() {
            if (_useFlatShading) FlatShading();
            else BakeNormals();
        }

        private void BakeNormals() { _bakedNormals = CalculateNormals(); }

        private void FlatShading() {
            var flatShadedVertices = new Vector3[_triangles.Length];
            var flatShadedUvs = new Vector2[_triangles.Length];

            for (var i = 0; i < _triangles.Length; i++) {
                flatShadedVertices[i] = _vertices[_triangles[i]];
                flatShadedUvs[i] = _uvs[_triangles[i]];
                _triangles[i] = i;
            }

            _vertices = flatShadedVertices;
            _uvs = flatShadedUvs;
        }
        
        public Mesh CreateMesh() {
            var mesh = new Mesh { vertices = _vertices, triangles = _triangles, uv = _uvs };
            
            if (_useFlatShading) mesh.RecalculateNormals();
            else mesh.normals = _bakedNormals;
            
            return mesh;
        }
    }
}
