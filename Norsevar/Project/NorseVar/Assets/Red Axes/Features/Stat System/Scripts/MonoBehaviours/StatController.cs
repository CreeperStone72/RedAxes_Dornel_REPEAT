using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using Object = UnityEngine.Object;

namespace Norsevar.Stat_System
{
    public class StatController
    {

        #region Delegates and Events

        public static event Action<IReadOnlyDictionary<EStatType, Stat>> OnInitialized;

        #endregion

        #region Private Fields

        private readonly StatCollectionData _statCollectionData;
        private readonly Dictionary<EStatType, Stat> _stats;

        #endregion

        #region Constructors

        public StatController(StatCollectionData statData)
        {
            _statCollectionData = statData;
            _stats = new Dictionary<EStatType, Stat>();
            Initialize();
        }

        #endregion

        #region Properties

        public IReadOnlyDictionary<EStatType, Stat> Stats => _stats;

        #endregion

        #region Private Methods

        private void Initialize()
        {
            foreach ((EStatType statType, StatDefinition statDefinition) in _statCollectionData.Attributes)
                _stats.Add(statType, new Stat(statDefinition));

            foreach ((EStatType eStatType, StatDefinition statDefinition) in _statCollectionData.Resources)
                _stats.Add(eStatType, new Resource(statDefinition));

            OnInitialized?.Invoke(Stats);
        }

        #endregion

        #region Public Methods

        public bool AddModifierToStat(EStatType statType, StatModifier modifier)
        {
            if (!_stats.ContainsKey(statType) || _stats[statType] == null)
                return false;

            _stats[statType].AddModifier(modifier);
            return true;
        }

        public bool AddModifierToStat(StatModifierDefinition statModifierDefinition, object source = null)
        {
            return AddModifierToStat(statModifierDefinition.TargetStat, statModifierDefinition.GetStatModifier(source));
        }

        public Stat GetStatOfType(EStatType type)
        {
            return _stats.ContainsKey(type) ? _stats[type] : null;
        }

        public float? GetStatValue(EStatType statType)
        {
            if (!_stats.ContainsKey(statType) || _stats[statType] == null)
                return null;

            return _stats[statType].Value;
        }

        public void RemoveAllModifiersOfSource(object source)
        {
            _stats.ForEach(statPair => statPair.Value.RemoveAllModifiersFromSource(source));
        }

        public bool RemoveAllModifiersOfSourceFromStat(EStatType statType, Object source)
        {
            if (!_stats.ContainsKey(statType) || _stats[statType] == null)
                return false;

            return _stats[statType].RemoveAllModifiersFromSource(source);
        }

        public bool RemoveModifierFromStat(EStatType statType, StatModifier modifier)
        {
            if (!_stats.ContainsKey(statType) || _stats[statType] == null)
                return false;

            return _stats[statType].RemoveModifier(modifier);
        }

        #endregion


        public Stat this[EStatType index] => _stats[index];
    }
}
