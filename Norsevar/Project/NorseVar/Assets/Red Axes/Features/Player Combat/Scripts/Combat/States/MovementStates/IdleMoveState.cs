using Norsevar.Stat_System;
using UnityEngine;

namespace Norsevar.Combat
{
    public class IdleMoveState : PlayerBaseState
    {

        #region Constants and Statics

        private static readonly int MovementSpeedMultiplier = Animator.StringToHash("MovementSpeedMultiplier");

        #endregion

        #region Private Fields

        private readonly Stat _movementSpeedMultStat;

        #endregion

        #region Protected Fields

        protected readonly PlayerMovementBehaviour PlayerMovement;
        protected readonly Animator Animator;
        protected readonly CharacterController CharacterController;
        protected bool CanApplyDirection = true;

        #endregion

        #region Constructors

        public IdleMoveState(PlayerController playerController, float timeoutTime, bool needsExitTime = false) : base(playerController, timeoutTime, needsExitTime)
        {
            PlayerMovement = playerController.Movement;
            Animator = playerController.Animator;
            CharacterController = playerController.Movement.Controller;

            _movementSpeedMultStat = playerController[EStatType.MovementSpeedMultiplier];
        }

        #endregion

        #region State Machine Methods

        public override void OnLogic()
        {
            //Update the movement speed based on the Stat -> todo this does not have to be done every frame!
            Animator.SetFloat(MovementSpeedMultiplier, _movementSpeedMultStat.Value - 1);

            if (CanApplyDirection)
                PlayerMovement.UpdateDirection();
        }

        #endregion

        #region Public Methods

        public override void OnAnimatorMove()
        {
            CharacterController.Move(Animator.deltaPosition);
        }

        #endregion

    }
}
