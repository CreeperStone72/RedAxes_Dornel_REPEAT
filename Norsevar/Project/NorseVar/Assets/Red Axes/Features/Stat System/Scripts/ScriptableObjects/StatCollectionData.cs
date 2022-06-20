using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.Stat_System
{
    [CreateAssetMenu(fileName = "StatDatabase", menuName = "Norsevar/Player/Stat System/Stat Database")]
    public class StatCollectionData : ScriptableGameObject
    {

        #region Properties

        public StatDefinitionDictionary Attributes => attribute;
        public StatDefinitionDictionary Resources => resource;

        #endregion

        [Title("Stats & Attributes", titleAlignment: TitleAlignments.Centered)]

        #region Public Fields

        [SerializeField] private StatDefinitionDictionary attribute;
        [SerializeField] private StatDefinitionDictionary resource;

        #endregion

    }
}
