using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.AI
{

    [CreateAssetMenu(fileName = "New Enemy", menuName = "Norsevar/AI/Enemy")]
    public class Enemy : ScriptableGameObject
    {

        #region Serialized Fields

        [HideLabel] [SerializeField]
        private AIStats stats;

        [HideLabel] [SerializeField]
        private AIHealth health;

        [InlineEditor(InlineEditorObjectFieldModes.Hidden)] [SerializeField]
        private AIStates states;

        [InlineEditor(InlineEditorObjectFieldModes.Hidden)] [SerializeField]
        private AttackType attackType;

        [HideLabel] [PreviewField(55)] [SerializeField]
        private Texture icon;

        #endregion

        #region Properties

        public AIStats Stats => stats;

        public AIHealth Health => health;

        public AIStates States => states;

        public AttackType AttackType => attackType;

        public Texture Icon => icon;

        #endregion

    }

}
