using PoissonDiscSampling;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SamplingSettings))]
public class SamplingSettingsEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Update")) ManualUpdate();
    }

    private void ManualUpdate() {
        ((SamplingSettings) target).NotifyOfUpdatedValues();
        EditorUtility.SetDirty(target);
    }
}