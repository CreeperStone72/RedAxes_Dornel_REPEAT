namespace ProceduralRockGeneration {
    using System.Collections.Generic;
    using UnityEngine;
    using Utility;
    
    public class RockGenerator : MonoBehaviour {
        public bool generateOnRun = true;
        [SerializeField] private Transform parent;
        [SerializeField] private ShapeSettings shapeSettings;
        [SerializeField] private ColorSettings colorSettings;
        [SerializeField] private float maxRadius = 5f;

        [HideInInspector] public List<GameObject> rocks;
        
        private void Awake() { if (parent == null) parent = transform; }

        private void Start() {
            rocks = new List<GameObject>();
            if (generateOnRun) GenerateRock();
        }

        public void GenerateRock() {
            var rock = SetupRock();
            rock.GetComponent<Rock>().GenerateRock(true);
            rocks.Add(rock);
        }

        private GameObject SetupRock() {
            shapeSettings.radius = RandUtils.Rand(1f, maxRadius);
            var rock = new GameObject("Rock", typeof(Rock)) { transform = { parent = parent } };
            rock.GetComponent<Rock>().shapeSettings = shapeSettings;
            rock.GetComponent<Rock>().colorSettings = colorSettings;
            return rock;
        }
    }
}