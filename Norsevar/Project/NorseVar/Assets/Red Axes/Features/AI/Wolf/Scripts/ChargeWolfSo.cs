using UnityEngine;

namespace Norsevar.AI
{
    [CreateAssetMenu(fileName = "New Wolf", menuName = "Norsevar/AI/Wolf/Charge")]
    public class ChargeWolfSo : ScriptableObject
    {

        #region Serialized Fields

        [Header("Searching")] [SerializeField]
        private float spottingDistance;
        [SerializeField] private float repositionVelocity;

        [Header("Hit")] [SerializeField]
        private float hitWait;

        [Header("Charge")] [SerializeField]
        private float minChargeDistance;
        [SerializeField] private float waitForCharge;
        [SerializeField] private float chargeVelocity;
        [SerializeField] private float maxChargeDistance;

        [Header("Idle")] [SerializeField]
        private float idleVelocity;
        [SerializeField] private float idleWanderDistance;
        [SerializeField] private float minIdleWait;
        [SerializeField] private float maxIdleWait;

        #endregion

        #region Properties

        public float SpottingDistance => spottingDistance;
        public float MinIdleWait => minIdleWait;
        public float MaxIdleWait => maxIdleWait;
        public float IdleWanderDistance => idleWanderDistance;
        public float RepositionVelocity => repositionVelocity;
        public float MinChargeDistance => minChargeDistance;
        public float WaitForCharge => waitForCharge;
        public float HitWait => hitWait;
        public float ChargeVelocity => chargeVelocity;
        public float IdleVelocity => idleVelocity;
        public float MaxChargeDistance => maxChargeDistance;

        #endregion

    }
}
