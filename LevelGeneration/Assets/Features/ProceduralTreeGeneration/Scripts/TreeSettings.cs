namespace ProceduralTreeGeneration {
    using UnityEngine;
    using Utility;

    [System.Serializable]
    public class TreeSettings {
        public Material material;
        [Space(5)]
        [SerializeField] private int minSideCount;
        [SerializeField] private int maxSideCount;
        [Space(5)]
        [SerializeField] private float minBottomRadius;
        [SerializeField] private float maxBottomRadius;
        [Space(5)]
        [SerializeField] private float minTopRadius;
        [SerializeField] private float maxTopRadius;
        [Space(5)]
        [SerializeField] private float minHeight;
        [SerializeField] private float maxHeight;

        public int GetSideCount() { return RandUtils.RandInt(minSideCount, maxSideCount); }

        public float GetBottomRadius() { return RandUtils.Rand(minBottomRadius, maxBottomRadius); }
        
        public float GetTopRadius() { return RandUtils.Rand(minTopRadius, maxTopRadius); }
        
        public float GetHeight() { return RandUtils.Rand(minHeight, maxHeight); }
    }
}