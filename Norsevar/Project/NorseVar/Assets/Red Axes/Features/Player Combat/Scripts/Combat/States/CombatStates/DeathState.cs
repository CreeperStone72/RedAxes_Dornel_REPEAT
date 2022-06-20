using UnityEngine;

namespace Norsevar.Combat
{
    public class DeathState : PlayerBaseState
    {

        #region Constants and Statics

        private static readonly int Die = Animator.StringToHash("Death");

        #endregion

        #region Constructors

        public DeathState(PlayerController playerController, float timeoutTime, bool needsExitTime = false) : base(playerController, timeoutTime, needsExitTime)
        {

        }

        #endregion

        #region State Machine Methods

        public override void OnEnter()
        {
            base.OnEnter();
            PlayerController.Animator.SetTrigger(Die);
        }

        #endregion

    }
}
