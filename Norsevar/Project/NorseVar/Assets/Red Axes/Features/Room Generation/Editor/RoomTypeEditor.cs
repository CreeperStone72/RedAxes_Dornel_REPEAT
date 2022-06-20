using System;
using UnityEditor;
using UnityEngine;

namespace Norsevar.Room_Generation
{

    [CustomEditor(typeof(RoomType))]
    public class RoomTypeEditor : UnityEditor.Editor
    {

        #region Private Methods

        private static void ConstantParameters(ref int constantWeight)
        {
            EditorGUILayout.LabelField("Constant growth parameters", EditorStyles.boldLabel);
            constantWeight = EditorGUILayout.IntField("Weight", constantWeight);
        }

        private static void DrawPersistenceRange(ref Persistence persistence, ref Vector2Int range)
        {
            switch (persistence)
            {
                case Persistence.StartingRoom:
                    range = Vector2Int.zero;
                    break;
                case Persistence.Segment:
                    range.x = EditorGUILayout.IntField("Start", range.x);
                    range.y = EditorGUILayout.IntField("End", range.y);
                    break;
                case Persistence.SemiPermanent:
                    range.x = EditorGUILayout.IntField("Start", range.x);
                    range.y = int.MaxValue;
                    break;
                case Persistence.Permanent:
                    range.x = 0;
                    range.y = int.MaxValue;
                    break;
                default:
                    range = Vector2Int.zero;
                    break;
            }
        }

        private static void LinearGrowthParameters(ref bool resetOnAppearance, ref float growthFactor, ref int defaultWeight)
        {
            EditorGUILayout.LabelField("Linear growth parameters", EditorStyles.boldLabel);
            resetOnAppearance = EditorGUILayout.Toggle("Reset on appearance", resetOnAppearance);
            growthFactor = EditorGUILayout.FloatField("Growth factor", growthFactor);
            defaultWeight = EditorGUILayout.IntField("Default weight", defaultWeight);
        }

        private static void WeightInformation(ref Persistence persistence, ref WeightInformation weightInformation)
        {
            EditorGUILayout.LabelField("Weight function parameters", EditorStyles.boldLabel);

            DrawPersistenceRange(ref persistence, ref weightInformation.range);

            weightInformation.function = (WeightFunction)EditorGUILayout.EnumPopup("Weight function", weightInformation.function);

            EditorGUILayout.Space();

            switch (weightInformation.function)
            {
                case WeightFunction.Constant:
                    ConstantParameters(ref weightInformation.defaultWeight);
                    break;
                case WeightFunction.LinearGrowth:
                    LinearGrowthParameters(
                        ref weightInformation.resetOnAppearance,
                        ref weightInformation.growthFactor,
                        ref weightInformation.defaultWeight);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion

        #region Public Methods

        public override void OnInspectorGUI()
        {
            // var roomType = target as RoomType;
            //
            // if (roomType == null)
            // {
            //     base.OnInspectorGUI();
            //     return;
            // }
            //
            // roomType.RoomName = EditorGUILayout.TextField("Name", roomType.RoomName);
            //
            // roomType.RoomColor = EditorGUILayout.ColorField("Color", roomType.RoomColor);
            //
            // Persistence roomTypeRoomPersistence = roomType.RoomPersistence;
            // roomType.RoomPersistence = (Persistence)EditorGUILayout.EnumPopup("Persistence", roomTypeRoomPersistence);
            //
            // roomType.ValidateData();
            //
            // EditorGUILayout.Space();
            //
            // if (roomTypeRoomPersistence == Persistence.StartingRoom)
            //     return;
            // WeightInformation roomTypeWeightInformation = roomType.WeightInformation;
            // WeightInformation(ref roomTypeRoomPersistence, ref roomTypeWeightInformation);
        }

        #endregion

    }
}
