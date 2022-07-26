using ProceduralRockGeneration;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Rock), true)]
public class RockEditor : Editor {
    private Rock _rock;
    private Editor _shapeEditor;
    private Editor _colorEditor;

    public override void OnInspectorGUI() {
        using (var check = new EditorGUI.ChangeCheckScope()) {
            base.OnInspectorGUI();
            if (check.changed) _rock.GenerateRock();
        }

        if (GUILayout.Button("Generate rock")) _rock.GenerateRock();

        DrawSettingsEditor(_rock.shapeSettings, _rock.OnShapeSettingsUpdated, ref _rock.shapeSettingsFoldout, ref _shapeEditor);
        DrawSettingsEditor(_rock.colorSettings, _rock.OnColorSettingsUpdated, ref _rock.colorSettingsFoldout, ref _colorEditor);
    }

    static void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor) {
        if (settings != null) {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);

            using var check = new EditorGUI.ChangeCheckScope();
            
            if (foldout) {
                CreateCachedEditor(settings, null, ref editor);
                editor.OnInspectorGUI();
                if (check.changed) onSettingsUpdated?.Invoke();
            }
        }
    }

    private void OnEnable() { _rock = (Rock) target; }
}