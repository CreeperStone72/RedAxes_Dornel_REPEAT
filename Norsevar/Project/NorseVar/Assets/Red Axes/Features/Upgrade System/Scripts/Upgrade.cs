using System;
using Norsevar.Stat_System;
using UnityEngine;

namespace Norsevar.Upgrade_System
{
    [Serializable]
    public class Upgrade
    {

        #region Constructors

        public Upgrade(UpgradeData data)
        {
            UpgradeData = data;
        }

        #endregion

        #region Properties

        public UpgradeData UpgradeData { get; private set; }

        #endregion

        #region Public Methods

        public void ApplyPlaystyleModifiers(UpgradeController playstyleModificationController)
        {
            if (UpgradeData == null || UpgradeData.PlaystyleModifiers == null)
                return;

            foreach (GameObject playstyleModifier in UpgradeData.PlaystyleModifiers)
                playstyleModificationController.AddPlaystyleModifier(playstyleModifier.GetComponent<PlaystyleModifierBase>());
        }

        public void ApplyStatModifiers(StatController statController)
        {
            if (UpgradeData == null || UpgradeData.StatModifiers == null)
                return;

            foreach ((EStatType statType, StatModifier statModifier) in UpgradeData.StatModifiers)
                statController.AddModifierToStat(statType, new StatModifier(statModifier.Value, statModifier.Type, this));
        }

        public void RemovePlayStyleModifiers(UpgradeController playstyleModificationController)
        {
            if (UpgradeData == null || UpgradeData.PlaystyleModifiers == null)
                return;

            foreach (GameObject playstyleModifier in UpgradeData.PlaystyleModifiers)
                playstyleModificationController.RemovePlaystyleModifier(playstyleModifier.GetComponent<PlaystyleModifierBase>());
        }

        public void RemoveStatModifiers(StatController statController)
        {
            if (UpgradeData == null || UpgradeData.StatModifiers == null)
                return;

            statController.RemoveAllModifiersOfSource(this);
        }

        #endregion

    }
}
