namespace Utility {
    using UnityEngine;

    /// <summary>
    /// Original code by Code Monkey
    /// Source : https://unitycodemonkey.com/utils.php
    /// Last accessed : 10/07
    /// </summary>
    public static class TextUtils {
        public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default,
                                               int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft,
                                               TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000) {
            color ??= Color.white;
            return CreateWorldText(parent, text, localPosition, fontSize, (Color) color, textAnchor, textAlignment, sortingOrder);
        }

        public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize,
                                               Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder) {
            var gameObject = new GameObject("World_Text", typeof(TextMesh));
            
            var transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            
            var textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            
            return textMesh;
        }
    }
}