using UnityEngine;

namespace Norsevar.Combat
{
    [CreateAssetMenu(fileName = nameof(ChargingAttackData), menuName = "Norsevar/Player/Combat/Attacks/Charging Attack")]
    public class ChargingAttackData : AttackData
    {

        #region Serialized Fields

        [SerializeField] private float chargeTime;
        [SerializeField] private float chargeDistance;
        [SerializeField] private GameObject throwPrefab;

        #endregion

        #region Properties

        public float ChargeTime => chargeTime;
        public float ChargeDistance => chargeDistance;
        public GameObject ThrowPrefab => throwPrefab;

        #endregion

    }
}
