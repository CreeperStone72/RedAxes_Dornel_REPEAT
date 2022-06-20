using UnityEngine;

namespace Norsevar.Combat
{
    public class UpgradePickupState : PlayerBaseState
    {

        #region Constants and Statics

        private static readonly int ReceiveUpgrade = Animator.StringToHash("ReceiveUpgrade");

        #endregion

        #region Constructors

        public UpgradePickupState(PlayerController playerController, float timeoutTime, bool needsExitTime = false) : base(playerController, timeoutTime, needsExitTime)
        {
        }

        #endregion

        #region State Machine Methods

        public override void OnEnter()
        {
            base.OnEnter();
            PlayerController.Animator.SetTrigger(ReceiveUpgrade);
            PlayerController.ExecuteInSeconds(() => fsm.RequestStateChange(PlayerStateType.Idle), 1f);
        }

        #endregion

    }
}
