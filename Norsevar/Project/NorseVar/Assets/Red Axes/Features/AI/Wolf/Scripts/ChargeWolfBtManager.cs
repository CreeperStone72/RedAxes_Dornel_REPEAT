using BehaviorDesigner.Runtime;
using Norsevar.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace Norsevar.AI
{
    public class ChargeWolfBtManager : BtManager
    {

        #region Serialized Fields

        [SerializeField] private ChargeWolfSo chargeWolf;
        [SerializeField] private UnityEvent preAttackFeedback;
        [SerializeField] private UnityEvent preAttackFeedbackCancel;

        private SharedBool _isCharging;

        #endregion

        #region Protected Methods

        protected override void OnAwake()
        {
            SetVariable("ChargeVelocity", chargeWolf.ChargeVelocity);
            SetVariable("DeathWait", chargeWolf.HitWait);
            SetVariable("IdleSpeed", chargeWolf.IdleVelocity);
            SetVariable("RepositionVelocity", chargeWolf.RepositionVelocity);
            SetVariable("SpottingDistance", chargeWolf.SpottingDistance);
            SetVariable("IdleMoveRadius", chargeWolf.IdleWanderDistance);
            SetVariable("MaxChargeDistance", chargeWolf.MaxChargeDistance);
            SetVariable("MaxIdleWait", chargeWolf.MaxIdleWait);
            SetVariable("MinChargeDistance", chargeWolf.MinChargeDistance);
            SetVariable("MinIdleWait", chargeWolf.MinIdleWait);
            SetVariable("WaitForChargeTime", chargeWolf.WaitForCharge);
            SetVariable("PreAttackFeedback", preAttackFeedback);


            _isCharging = (SharedBool) behaviorTree.GetVariable("IsCharging");
        }


        public override void Kickback(Vector3 dirToTarget, float? damageInfo)
        {
            base.Kickback(dirToTarget, damageInfo);
            if (!_isCharging.Value) return;
            preAttackFeedbackCancel?.Invoke();
        }

        #endregion

    }
}
