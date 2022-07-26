using ProceduralLayoutGeneration;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ProceduralLayout))]
public class ProceduralLayoutEditor : Editor {
    public override void OnInspectorGUI() {
        var proceduralLayout = (ProceduralLayout) target;
        if (DrawDefaultInspector()) AutoUpdate(proceduralLayout);
        if (GUILayout.Button("Generate")) Update(proceduralLayout);
    }
    
    private static void AutoUpdate(ProceduralLayout proceduralLayout) { if (proceduralLayout.autoUpdate) Update(proceduralLayout); }
    
    private static void Update(ProceduralLayout proceduralLayout) { proceduralLayout.DrawMapInEditor(); }
}