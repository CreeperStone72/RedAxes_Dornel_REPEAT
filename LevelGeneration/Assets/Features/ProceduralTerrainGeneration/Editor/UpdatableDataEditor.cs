using ProceduralTerrainGeneration.Data;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UpdatableData), true)]
public class UpdatableDataEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Update")) ManualUpdate();
    }

    private void ManualUpdate() {
        ((UpdatableData) target).NotifyOfUpdatedValues();
        EditorUtility.SetDirty(target);
    }
}