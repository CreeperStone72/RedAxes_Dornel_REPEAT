using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.AI
{

    [CreateAssetMenu(fileName = "New Pack", menuName = "Norsevar/AI/Wolf/Pack")]
    public class WolfPack : SerializedScriptableObject
    {

        #region Serialized Fields

        [HideLabel] [SerializeField] [PreviewField(100, ObjectFieldAlignment.Left)] [AssetsOnly] [HorizontalGroup("Row 1", 100)]
        private GameObject wolf;

        [Tooltip("Number of wolves in a pack.")] [LabelWidth(100)] [Range(1, 10)] [VerticalGroup("Row 1/Col 1")] [SerializeField]
        private int packSize;

        [Tooltip("Amount of rows for spawning wolves")] [VerticalGroup("Row 1/Col 1")] [LabelWidth(100)] [Range(1, 10)] [SerializeField]
        private int rows;

        [Tooltip("The spacing between each wolf as they are instantiated.")] [Range(1, 6)] [VerticalGroup("Row 1/Col 1")] [LabelWidth(100)]
        [SerializeField]
        private int spacing;

        [LabelWidth(100)] [Range(1, 5)] [Tooltip("Distance from the target at which wolves will 'orbit'.")] [SerializeField]
        private float radius;

        #endregion

        #region Properties

        public int PackSize => packSize;

        public GameObject Wolf => wolf;

        public int Rows => rows;

        public int Spacing => spacing;

        public float Radius => radius;

        #endregion

    }

}
