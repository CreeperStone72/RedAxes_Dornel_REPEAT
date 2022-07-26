namespace ProceduralTerrainGeneration.Data {
    using DataTypes;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Norsevar/Procedural Terrain Generation/Mesh Settings")]
    public class MeshSettings : UpdatableData {
        public const int NumSupportedLoDs = 5;
        private const int NumSupportedChunkSizes = 9;
        private const int NumSupportedFlatShadedChunkSizes = 3;
        private static readonly int[] SupportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240, };
        
        public float meshScale = 2.5f;
        public bool useFlatShading;
        
        [Range(0, NumSupportedChunkSizes - 1)] public int chunkSizeIndex;
        [Range(0, NumSupportedFlatShadedChunkSizes - 1)] public int fSChunkSizeIndex;
        
        // Number of vertices per line of mesh rendered at LOD = 0
        // Includes the two extra vertices (excluded from final mesh) for calculating normals
        public int NumVertsPerLine => SupportedChunkSizes[useFlatShading ? fSChunkSizeIndex : chunkSizeIndex] + 5;

        public float MeshWorldSize => (NumVertsPerLine - 3) * meshScale;
    }
}