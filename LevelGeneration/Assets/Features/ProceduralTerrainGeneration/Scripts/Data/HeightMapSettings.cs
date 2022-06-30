namespace ProceduralTerrainGeneration.Data {
    using UnityEngine;
    
    [CreateAssetMenu(menuName = "Norsevar/Procedural Terrain Generation/Height Map Settings")]
    public class HeightMapSettings : UpdatableData {
        public NoiseSettings noiseSettings;
        
        public bool useFalloff;

        public float heightMultiplier;
        public AnimationCurve heightCurve;

        public float MinHeight => heightMultiplier * heightCurve.Evaluate(0);

        public float MaxHeight => heightMultiplier * heightCurve.Evaluate(1);

        #if UNITY_EDITOR
        
        protected override void OnValidate() {
            base.OnValidate();
            noiseSettings.ValidateValues();
        }
        
        #endif
    }
}