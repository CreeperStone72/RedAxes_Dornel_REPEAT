using UnityEngine;

namespace Norsevar.Stat_System
{
    [CreateAssetMenu(fileName = "StatModifierDefinition", menuName = "Norsevar/Player/Stat System/Stat Modifier Definition")]
    public class StatModifierDefinition : ScriptableGameObject
    {

        #region Private Fields

        private StatModifier _statModifier;

        #endregion

        #region Serialized Fields

        [SerializeField] private EStatType targetStat;
        [SerializeField] private float value;
        [SerializeField] private EModifierType modifierType;

        #endregion

        #region Properties

        public EStatType TargetStat => targetStat;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            _statModifier = new StatModifier(value, modifierType);
        }

        #endregion

        #region Public Methods

        public StatModifier GetStatModifier(object source = null)
        {
            _statModifier.Source = source;
            return _statModifier;
        }

        #endregion

    }
}
