using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.AI
{

    public abstract class AttackType : SerializedScriptableObject
    {

        #region Serialized Fields

        [SerializeField] [Tooltip("Layers the attack can hit.")]
        private LayerMask hittableLayers;

        [SerializeField] [Range(0, 100)] private int baseDamage;

        #endregion

        #region Properties

        public LayerMask HittableLayers => hittableLayers;

        public int BaseDamage => baseDamage;

        #endregion

    }

}
