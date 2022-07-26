namespace DataTypes {
    using UnityEngine;
    using Utility;
    
    /// <summary>
    /// 2D Grid system which supports world position
    /// Original code by Code Monkey, altered by me to be generic and use custom indexer instead of methods
    /// Source : https://www.youtube.com/watch?v=waEsGu--9P8
    /// Last accessed : 10/07
    /// </summary>
    /// <typeparam name="T">The type of data stored in the grid</typeparam>
    public class DataGrid<T> {
        public bool useDebug;
        protected readonly int width;
        protected readonly int height;
        private readonly float _cellSize;
        private readonly Vector3 _originPosition;

        private readonly T[,] _grid;
        protected readonly TextMesh[,] debugTexts;
        
        public DataGrid(int width, int height, float cellSize, Vector3 originPosition = default) {
            this.width = width;
            this.height = height;
            _cellSize = cellSize;
            _originPosition = originPosition;

            _grid = new T[width, height];
            
            if (useDebug) {
                debugTexts = new TextMesh[width, height];

                for (var x = 0; x < _grid.GetLength(0); x++) {
                    for (var y = 0; y < _grid.GetLength(1); y++) {
                        debugTexts[x, y] = TextUtils.CreateWorldText($"{_grid[x, y]}", null, GetWorldPositionCentre(x, y), 20, Color.white, TextAnchor.MiddleCenter);
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                    }
                }
            
                Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
            }
        }

        public T this[Vector3 worldPosition] {
            get { GetXY(worldPosition, out var x, out var y); return this[x, y]; }
            set { GetXY(worldPosition, out var x, out var y); this[x, y] = value; }
        }

        public T this[int x, int y] {
            get => MathUtils.IsInRange(x, 0, width) && MathUtils.IsInRange(y, 0, height) ? _grid[x, y] : default;
            set {
                if (!MathUtils.IsInRange(x, 0, width) || !MathUtils.IsInRange(y, 0, height)) return;
                _grid[x, y] = value;
                if (useDebug) debugTexts[x, y].text = _grid[x, y] != null ? _grid[x, y].ToString() : "";
            }
        }

        private Vector3 GetWorldPositionCentre(int x, int y) { return GetWorldPosition(x, y) + new Vector3(_cellSize, _cellSize) * .5f; }
        
        private Vector3 GetWorldPosition(int x, int y) { return new Vector3(x, y) * _cellSize + _originPosition; }

        protected void GetXY(Vector3 worldPosition, out int x, out int y) {
            x = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize);
            y = Mathf.FloorToInt((worldPosition - _originPosition).y / _cellSize);
        }

        protected virtual void Clear() { for (var x = 0; x < width; x++) { for (var y = 0; y < height; y++) { this[x, y] = default; } } }
    }
}