namespace WaveFunctionCollapse {
    using UnityEngine;
    using System;
    using System.Linq;
    
    [Serializable, CreateAssetMenu(menuName = "Norsevar/Wave Function Collapse/Constrained Tile")]
    public class ConstrainedTile : ScriptableObject {
        public string tileType;
        public float objectRadius = 10f;
        public Color tileColor = Color.white;
        public ConstrainedTile[] validNeighbors;

        // Constraint : A tile can always be put beside itself or a valid neighbor
        public bool RespectsConstraint(ConstrainedTile neighbor, float distance) { return IsNeighborValid(neighbor) && IsFarEnough(distance); }
        
        public bool IsNeighborValid(ConstrainedTile neighbor) { return validNeighbors.Contains(neighbor); }
        
        public bool IsFarEnough(float distance) { return distance >= objectRadius; }

        public override string ToString() { return tileType; }

        private void OnValidate() {
            if (validNeighbors.Length > 0) return;
            
            validNeighbors = new ConstrainedTile[1];
            validNeighbors[0] = this;
        }
    }
}