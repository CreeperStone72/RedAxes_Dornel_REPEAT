using UnityEngine;

namespace Norsevar.AI
{

    [CreateAssetMenu(fileName = "New Close Combat Attack Type", menuName = "Norsevar/AI/Attack/Close")]
    public class CloseCombatAttackType : AttackType
    {

        #region Serialized Fields

        [SerializeField] [Range(0, 10)] private float range;

        #endregion

        #region Properties

        public float Range => range;

        #endregion

    }

}
