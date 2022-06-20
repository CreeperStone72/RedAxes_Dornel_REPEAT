using UnityEngine;

namespace Norsevar.AI
{

    [CreateAssetMenu(fileName = "New Ranged Combat Attack Type", menuName = "Norsevar/AI/Attack/Ranged")]
    public class RangedCombatAttackType : AttackType
    {

        #region Serialized Fields

        [SerializeField] [Range(20, 80)] private float throwAngle;
        [SerializeField] private GameObject projectile;

        #endregion

        #region Properties

        public float ThrowAngle => throwAngle;

        public GameObject Projectile => projectile;

        #endregion

    }

}
