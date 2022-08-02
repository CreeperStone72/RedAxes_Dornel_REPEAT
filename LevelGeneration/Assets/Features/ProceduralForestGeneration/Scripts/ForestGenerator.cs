using ProceduralTerrainGeneration;
using UnityEngine;
using Utility;

namespace ProceduralForestGeneration {
    public class ForestGenerator : MonoBehaviour {
        private MapPreview _map;
        private PropPlacer3D _props;

        private void Start() {
            _map = GetComponentInChildren<MapPreview>();
            _props = GetComponentInChildren<PropPlacer3D>();

            RandomizeLand();
            _props.PlaceProps();
        }

        private void RandomizeLand() {
            _map.heightMapSettings.noiseSettings.seed = RandUtils.RandInt(0, 100000);
            _map.heightMapSettings.noiseSettings.offset = new Vector2(RandUtils.Rand(-100f, 100f), RandUtils.Rand(-100f, 100f));
            _map.DrawMapInEditor();
        }
    }
}