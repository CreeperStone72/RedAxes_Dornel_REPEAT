namespace PoissonDiscSampling {
    using System.Collections.Generic;
    using UnityEngine;
    using Utility;
    
    public static class PoissonDiscSampling {
        public static List<Vector2> GeneratePoints(float radius, Vector2 sampleRegionSize, int numSamplesBeforeRejection = 30) {
            var cellSize = radius / MathUtils.Sqrt2;

            var grid = new int[Mathf.CeilToInt(sampleRegionSize.x / cellSize), Mathf.CeilToInt(sampleRegionSize.y / cellSize)];
            var points = new List<Vector2>();
            var spawnPoints = new List<Vector2> { sampleRegionSize / 2 };

            while (spawnPoints.Count > 0) {
                var spawnIndex = Random.Range(0, spawnPoints.Count);
                var spawnCentre = spawnPoints[spawnIndex];
                var candidateAccepted = false;

                for (var i = 0; i < numSamplesBeforeRejection; i++) {
                    var angle = MathUtils.RandomAngle;
                    var dir = MathUtils.AngleToDir(angle);
                    var candidate = spawnCentre + dir * Random.Range(radius, 2 * radius);

                    if (!IsValid(candidate, sampleRegionSize, cellSize, radius, points, grid)) continue;
                    
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[CellIndex(candidate.x, cellSize), CellIndex(candidate.y, cellSize)] = points.Count;
                    candidateAccepted = true;
                    break;
                }

                if (!candidateAccepted) spawnPoints.RemoveAt(spawnIndex);
            }

            return points;
        }

        private static bool IsValid(Vector2 candidate, Vector2 sampleRegionSize, float cellSize, float radius, List<Vector2> points, int[,] grid) {
            if (!(0 <= candidate.x && candidate.x < sampleRegionSize.x && 0 <= candidate.y && candidate.y < sampleRegionSize.y)) return false;

            var start = GetStart(candidate, cellSize);
            var end = GetEnd(candidate, cellSize, grid);

            for (var x = start.x; x <= end.x; x++) {
                for (var y = start.y; y <= end.y; y++) {
                    var pointIndex = grid[x, y] - 1;

                    if (pointIndex == -1) continue;
                        
                    var sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
                    if (sqrDst < radius * radius) return false;
                }
            }

            return true;
        }

        private static Vector2Int GetStart(Vector2 centre, float size) {
            int cellX = CellIndex(centre.x, size), cellY = CellIndex(centre.y, size);
            return new Vector2Int(Start(cellX), Start(cellY));
        }

        private static int Start(int cell) { return Mathf.Max(0, cell - 2); }

        private static Vector2Int GetEnd(Vector2 centre, float size, int[,] grid) {
            int cellX = CellIndex(centre.x, size), cellY = CellIndex(centre.y, size);
            return new Vector2Int(End(cellX, grid.GetLength(0)), End(cellY, grid.GetLength(1)));
        }

        private static int End(int cell, int max) { return Mathf.Min(cell + 2, max - 1); }
        
        private static int CellIndex(float candidate, float cellSize) { return (int) (candidate / cellSize); }
    }
}
