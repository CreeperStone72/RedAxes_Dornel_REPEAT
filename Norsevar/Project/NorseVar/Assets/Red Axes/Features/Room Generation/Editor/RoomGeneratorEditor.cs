using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Norsevar.Room_Generation
{
    using HLG = HorizontalLayoutGroup;

    [CustomEditor(typeof(RoomGenerator))]
    public class RoomGeneratorEditor : UnityEditor.Editor
    {

        #region Private Methods

        private static void ButtonPanelParameters(ref RoomGenerator generator)
        {
            EditorGUILayout.LabelField("Button panel parameters", EditorStyles.boldLabel);
            generator.ButtonModel = (GameObject)EditorGUILayout.ObjectField(
                "Button model",
                generator.ButtonModel,
                typeof(GameObject),
                true);
            generator.ButtonPanel = (HLG)EditorGUILayout.ObjectField("Button panel", generator.ButtonPanel, typeof(HLG), true);
        }

        private void RoomTypes()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("roomTypes"), true);
            serializedObject.ApplyModifiedProperties();
        }

        private static void ToggleParameters(ref RoomGenerator generator)
        {
            generator.enableMinimap = EditorGUILayout.Toggle("Enable minimap", generator.enableMinimap);
            if (generator.enableMinimap)
                generator.enableButtonPanel = EditorGUILayout.Toggle("Enable button panel", generator.enableButtonPanel);
        }

        private static void TreeModelParameters(ref RoomGenerator generator)
        {
            EditorGUILayout.LabelField("Tree model parameters", EditorStyles.boldLabel);
            generator.Depth = EditorGUILayout.IntField("Depth", generator.Depth);
            generator.Scale = EditorGUILayout.IntField("Scale", generator.Scale);
            generator.RoomModel = (GameObject)EditorGUILayout.ObjectField("Room model", generator.RoomModel, typeof(GameObject), true);
        }

        private static void UIParameters(ref RoomGenerator generator)
        {
            EditorGUILayout.LabelField("UI parameters", EditorStyles.boldLabel);
            generator.MinimapCamera = (Camera)EditorGUILayout.ObjectField("Camera", generator.MinimapCamera, typeof(Camera), true);
            generator.TimeToZoom = EditorGUILayout.FloatField("Time to zoom", generator.TimeToZoom);
            generator.ZoomFOV = EditorGUILayout.FloatField("Default FOV", generator.ZoomFOV);
            generator.DefaultFOV = EditorGUILayout.FloatField("Zoom out FOV", generator.DefaultFOV);
            generator.TimeToMove = EditorGUILayout.FloatField("Time to move", generator.TimeToMove);
        }

        #endregion

        #region Public Methods

        public override void OnInspectorGUI()
        {
            var generator = target as RoomGenerator;

            if (generator == null)
            {
                base.OnInspectorGUI();
                return;
            }

            ToggleParameters(ref generator);

            EditorGUILayout.Space();

            TreeModelParameters(ref generator);

            if (generator.enableMinimap)
            {
                EditorGUILayout.Space();

                UIParameters(ref generator);

                if (generator.enableButtonPanel)
                {
                    EditorGUILayout.Space();

                    ButtonPanelParameters(ref generator);
                }
            }

            EditorGUILayout.Space();

            RoomTypes();
        }

        #endregion

    }
}
