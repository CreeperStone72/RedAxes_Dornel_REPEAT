namespace WaveFunctionCollapse {
    using UnityEngine;
    using Utility;
    
    public class WaveFunctionCollapse : MonoBehaviour {
        [SerializeField] private Camera worldCamera;
        
        [SerializeField] private int width;
        [SerializeField] private int height;

        [SerializeField] private float cellSize = 10f;

        [SerializeField] private ConstrainedTile[] possibleValues;
        
        private CollapsingGrid _grid;
        private int _scrollCount;
        private TextMesh _tileType;

        private void Start() {
            _grid = new CollapsingGrid(width, height, cellSize, possibleValues);
            _scrollCount = 0;
            _tileType = TextUtils.CreateWorldText(Type, null, new Vector3(0, height * cellSize + 5f), 20);

            worldCamera.transform.position = new Vector3(width * cellSize * .5f, height * cellSize * .5f, -height * cellSize);
        }

        private void Update() {
            if (Mouse.IsScrolling) {
                // Reversed scrolling so that scrolling down goes down the array instead of up
                _scrollCount -= Mouse.ScrollDirection;
                _scrollCount = MathUtils.ModN(_scrollCount, possibleValues.Length);
                _tileType.text = Type;
            }
            
            if (Mouse.LeftClickDown) {
                _grid.SetAndCollapse(MouseUtils.GetMouseWorldPosition(worldCamera), possibleValues[_scrollCount]);
            }

            if (Mouse.RightClickDown) {
                Debug.Log(_grid.GetPossibilities(MouseUtils.GetMouseWorldPosition(worldCamera)));
            }
        }

        public void GenerateGrid() { _grid.Generate(); }

        private string Type => $"Selected tile : {possibleValues[_scrollCount]}";
    }
}