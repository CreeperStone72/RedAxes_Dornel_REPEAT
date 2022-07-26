namespace WaveFunctionCollapse {
    using DataTypes;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Utility;
    
    public class CollapsingGrid : DataGrid<ConstrainedTile> {
        private readonly List<ConstrainedTile>[,] _possibilities;
        private readonly ConstrainedTile[] _possibleValues;
        
        public CollapsingGrid(int width, int height, float cellSize, ConstrainedTile[] possibleValues, Vector3 origin = default) :
            base(width, height, cellSize, origin) {
            _possibleValues = possibleValues;
            _possibilities = new List<ConstrainedTile>[width, height];
            Clear();
        }

        public bool IsComplete => _possibilities.Cast<List<ConstrainedTile>>().All(cell => cell.Count <= 1);

        public void Generate() {
            if (IsComplete) Clear();

            while (!IsComplete) {
                int x = RandUtils.RandInt(0, width), y = RandUtils.RandInt(0, height);

                if (_possibilities[x, y].Count <= 1) continue;
                var value = RandUtils.PickAny(_possibilities[x, y]);
                SetAndCollapse(x, y, value);
            }
        }

        public string GetPossibilities(Vector3 worldPosition) {
            GetXY(worldPosition, out var x, out var y);
            return GetPossibilities(x, y);
        }
        
        public string GetPossibilities(int x, int y) { return string.Join(", ", _possibilities[x, y]); }

        protected sealed override void Clear() {
            base.Clear();
            for (var x = 0; x < width; x++) { for (var y = 0; y < height; y++) { _possibilities[x, y] = new List<ConstrainedTile>(_possibleValues); } }
        }

        public ConstrainedTile RandomSetAndCollapse(Vector3 worldPosition) {
            GetXY(worldPosition, out var x, out var y);
            var value = RandUtils.PickAny(_possibilities[x, y]);
            SetAndCollapse(x, y, value);
            return value;
        }
        
        public void SetAndCollapse(Vector3 worldPosition, ConstrainedTile value) {
            GetXY(worldPosition, out var x, out var y);
            SetAndCollapse(x, y, value);
        }

        public void SetAndCollapse(int x, int y, ConstrainedTile value) {
            if (!MathUtils.IsInRange(x, 0, width) || !MathUtils.IsInRange(y, 0, height)) return;
            if (!_possibilities[x, y].Contains(value)) return;
            this[x, y] = value;
            if (useDebug) debugTexts[x, y].color = value.tileColor;
            _possibilities[x, y].RemoveAll(tile => tile != value);
            Collapse(x, y);
        }

        private void Collapse(int x, int y) {
            int xW = Mathf.Max(x - 1, 0), xE = Mathf.Min(width - 1, x + 1);
            int yN = Mathf.Max(y - 1, 0), yS = Mathf.Min(height - 1, y + 1);

            var values = _possibilities[x, y];
            
            var validN = new HashSet<ConstrainedTile>();
            var validS = new HashSet<ConstrainedTile>();
            var validW = new HashSet<ConstrainedTile>();
            var validE = new HashSet<ConstrainedTile>();

            foreach (var value in values) {
                validN.UnionWith(_possibilities[x, yN].Where(tile => tile.IsNeighborValid(value)).ToList());
                validS.UnionWith(_possibilities[x, yS].Where(tile => tile.IsNeighborValid(value)).ToList());
                validW.UnionWith(_possibilities[xW, y].Where(tile => tile.IsNeighborValid(value)).ToList());
                validE.UnionWith(_possibilities[xE, y].Where(tile => tile.IsNeighborValid(value)).ToList());
            }

            if (validN.Count < _possibilities[x, yN].Count && yN != y) {
                _possibilities[x, yN] = validN.ToList();
                
                if (validN.Count == 1) SetAndCollapse(x, yN, _possibilities[x, yN][0]);
                else Collapse(x, yN);
            }
            
            if (validS.Count < _possibilities[x, yS].Count && yS != y) {
                _possibilities[x, yS] = validS.ToList();

                if (validS.Count == 1) SetAndCollapse(x, yS, _possibilities[x, yS][0]);
                else Collapse(x, yS);
            }
            
            if (validW.Count < _possibilities[xW, y].Count && xW != x) {
                _possibilities[xW, y] = validW.ToList();

                if (validW.Count == 1) SetAndCollapse(xW, y, _possibilities[xW, y][0]);
                else Collapse(xW, y);
            }
            
            if (validE.Count < _possibilities[xE, y].Count && xE != x) {
                _possibilities[xE, y] = validE.ToList();

                if (validE.Count == 1) SetAndCollapse(xE, y, _possibilities[xE, y][0]);
                else Collapse(xE, y);
            }
        }
    }
}