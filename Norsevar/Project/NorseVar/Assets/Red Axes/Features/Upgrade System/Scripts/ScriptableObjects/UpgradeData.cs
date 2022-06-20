using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Norsevar.Upgrade_System
{
    [CreateAssetMenu(fileName = "New Upgrade", menuName = "Norsevar/Upgrades/Upgrade")]
    public class UpgradeData : ScriptableGameObject
    {

        #region Serialized Fields

        [SerializeField] [Tooltip("UI Icon shown in the HUD after the upgrade was consumed.")]
        private int icon;

        [SerializeField]
        [Header("Play-Style Modifications")] [Tooltip(
            "Prefabs are instantiated by the Upgrade Controller when the Upgrade is consumed. " +
            "Root GameObject in the Prefab needs to have a script of type PlayStyleModifer applied.")]
        private List<GameObject> playstyleModifiersToApply;

        [Header("Character Stat Modifications")]
        [Tooltip("Modifiers defined here are applied to the character's stats when the Upgrade is consumed.")] [SerializeField]
        private StatDictionary statModifiers;

        #endregion

        #region Properties

        public StatDictionary StatModifiers => statModifiers;
        public int Icon => icon;
        public List<GameObject> PlaystyleModifiers => playstyleModifiersToApply;

        #endregion

        #region Unity Methods

        protected virtual void OnValidate()
        {
            //check if playstyle modifiers have a PlayStyleModifier component attached to the root gameObject
            if (PlaystyleModifiers is not { Count: > 0 })
                return;
            foreach (GameObject unused in PlaystyleModifiers.Where(gameObject => !gameObject.TryGetComponent<PlaystyleModifierBase>(out _)))
                Debug.LogError("Attached GameObject does not contain a PlayStyleModifier component!");
        }

        #endregion

    }
}
