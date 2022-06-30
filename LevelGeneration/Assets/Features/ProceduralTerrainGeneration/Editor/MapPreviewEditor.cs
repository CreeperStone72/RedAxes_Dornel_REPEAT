using ProceduralTerrainGeneration;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapPreview))]
public class MapPreviewEditor : Editor {
    public override void OnInspectorGUI() {
        var mapPreview = (MapPreview) target;
        if (DrawDefaultInspector()) AutoUpdate(mapPreview);
        if (GUILayout.Button("Generate")) Update(mapPreview);
    }
    
    private static void AutoUpdate(MapPreview mapPreview) { if (mapPreview.autoUpdate) Update(mapPreview); }
    
    private static void Update(MapPreview mapPreview) { mapPreview.DrawMapInEditor(); }
}
