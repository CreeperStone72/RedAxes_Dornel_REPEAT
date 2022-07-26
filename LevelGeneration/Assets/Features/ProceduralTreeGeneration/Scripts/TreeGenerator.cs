namespace ProceduralTreeGeneration {
    using System.Collections.Generic;
    using UnityEngine;
    using Utility;
    
    public class TreeGenerator : MonoBehaviour {
        public bool standaloneBuild = true;
        public Transform parent;
        [SerializeField] private TreeSettings trunkSettings;
        [SerializeField] private TreeSettings foliageSettings;
        [Space(5)]
        [SerializeField] private float minFoliageHeight;
        [SerializeField] private float maxFoliageHeight;
        [Space(5)]
        [SerializeField] private float minFoliageInterval;
        [SerializeField] private float maxFoliageInterval;
        [Space(5)]
        [SerializeField] private int minFoliageCount;
        [SerializeField] private int maxFoliageCount;
        [Space(5)]
        [SerializeField] private float shrinkFactor = .9f;
        
        private RegularPyramid _trunk, _foliage;

        [HideInInspector] public List<GameObject> trees;

        private void Start() {
            trees = new List<GameObject>();
            if (standaloneBuild) GenerateTree();
        }

        public void GenerateTree() {
            _trunk = new RegularPyramid();
            _foliage = new RegularPyramid();

            var parentTree = new GameObject("Tree");
            parentTree.transform.parent = parent;
            
            ApplySettings();
            var tree = BuildTree(parentTree);
            trees.Add(tree);
        }

        #region Apply Settings

        private void ApplySettings() {
            ApplyTrunkSettings();
            ApplyFoliageSettings();
        }
        
        private void ApplyTrunkSettings() {
            _trunk.baseRadius = trunkSettings.GetBottomRadius();
            _trunk.innerRadius = trunkSettings.GetTopRadius();
            _trunk.height = trunkSettings.GetHeight();
            _trunk.baseSides = trunkSettings.GetSideCount();
            _trunk.material = trunkSettings.material;
        }
        
        private void ApplyFoliageSettings() {
            _foliage.baseRadius = foliageSettings.GetBottomRadius();
            _foliage.innerRadius = foliageSettings.GetTopRadius();
            _foliage.height = foliageSettings.GetHeight();
            _foliage.baseSides = foliageSettings.GetSideCount();
            _foliage.material = foliageSettings.material;
        }
        
        #endregion

        #region Build Tree

        private GameObject BuildTree(GameObject parentTree) {
            PlaceFoliage(parentTree);
            PlaceTrunk(parentTree);
            return parentTree;
        }

        private void PlaceFoliage(GameObject parentTree) {
            _foliage.origin.y += GetFoliageHeight();
            var interval = GetFoliageInterval();
            
            for (var index = 0; index < GetFoliageCount() - 1; index++) {
                BuildPyramid("Foliage", _foliage, parentTree);
                _foliage.origin.y += interval;
                _foliage.baseRadius *= shrinkFactor;
                _foliage.innerRadius *= shrinkFactor;
                _foliage.height *= shrinkFactor;
            }
            
            // Making sure that the top foliage is closed
            _foliage.innerRadius = 0f;
            BuildPyramid("Foliage", _foliage, parentTree);
        }

        private void PlaceTrunk(GameObject parentTree) {
            var max = _foliage.origin.y + _foliage.height;

            if (_trunk.height >= max) {
                _trunk.height = max;
                _trunk.innerRadius = 0f;
            }
            
            BuildPyramid("Trunk", _trunk, parentTree);
        }

        #endregion

        private static void BuildPyramid(string goName, RegularPyramid pyramid, GameObject parentTree) {
            var go = new GameObject(goName, typeof(MeshFilter), typeof(MeshRenderer));
            go.transform.parent = parentTree.transform;
            go.GetComponent<MeshFilter>().mesh = ConeGenerator.BuildConeMesh(pyramid);
            go.GetComponent<MeshRenderer>().sharedMaterial = pyramid.material;
        }
        
        private float GetFoliageHeight() { return RandUtils.Rand(minFoliageHeight, maxFoliageHeight); }

        private float GetFoliageInterval() { return RandUtils.Rand(minFoliageInterval, maxFoliageInterval); }

        private int GetFoliageCount() { return RandUtils.RandInt(minFoliageCount, maxFoliageCount); }
    }
}
