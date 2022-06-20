using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.Stat_System
{
    [CreateAssetMenu(fileName = "StatDefinition", menuName = "Norsevar/Player/Stat System/Stat Definition")]
    public class StatDefinition : ScriptableGameObject
    {

        #region Properties

        public float BaseValue => baseValue;
        public float Cap => cap;

        #endregion

        [Title("Stat Definition", titleAlignment: TitleAlignments.Centered)] [SerializeField]

        #region Serialized Fields

        private float baseValue;
        [SerializeField] private float cap = -1;

        #endregion

    }
}
