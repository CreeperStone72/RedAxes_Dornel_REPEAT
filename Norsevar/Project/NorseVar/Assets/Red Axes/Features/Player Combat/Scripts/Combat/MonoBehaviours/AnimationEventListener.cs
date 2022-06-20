using JetBrains.Annotations;
using UnityEngine;

namespace Norsevar.Combat
{
    [RequireComponent(typeof(Animator))]
    public class AnimationEventListener : MonoBehaviour
    {

        #region Serialized Fields

        [SerializeField] private PlayerController playerController;

        #endregion

        #region Private Methods

        //Called by animation event (DO NOT RENAME!)

        [UsedImplicitly]
        private void AnimationEventTrigger(AnimationEvent info)
        {
            playerController.HandleAnimationEvent(info);
        }

        [UsedImplicitly]
        private void OnBreathe(AnimationEvent info)
        {
            playerController.HandlePlayerBreatheEvent(info);
        }

        [UsedImplicitly]
        private void OnFootTouchGround(AnimationEvent info)
        {
            playerController.HandleFootTouchSnowEvent(info);
        }

        #endregion

    }
}
