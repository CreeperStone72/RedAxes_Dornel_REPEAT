using Norsevar.Combat;
using UnityEngine;

namespace Norsevar.Upgrade_System
{
    public class AnimationEventSpawnModifier : PlaystyleModifierBase
    {

        #region Serialized Fields

        [SerializeField] private EAnimationEventType animationType;
        [SerializeField] private GameObject spawnPrefab;
        [SerializeField] private float aliveForSeconds;

        #endregion

        #region Public Methods

        public override void HandleAnimationEvent(Attack weaponAttack, EAnimationEventType animationEventType)
        {
            if (animationEventType != animationType)
                return;
            GameObject obj = Instantiate(spawnPrefab, ownerTransform.position, ownerTransform.rotation);
            Destroy(obj, aliveForSeconds);
        }

        #endregion

    }
}
