using System;
using Norsevar.Interaction;
using Norsevar.Status_Effect_System;

namespace Norsevar
{
    public class AnalyticsMono : Singleton<AnalyticsMono>
    {

        #region Unity Methods

        private void OnEnable()
        {
            ItemBehaviour.OnItemPickUp += HandleUpgradePickUp;
        }

        private void OnDisable()
        {
            ItemBehaviour.OnItemPickUp -= HandleUpgradePickUp;
        }

        #endregion

        #region Private Methods

        private static void HandleUpgradePickUp(Item obj)
        {
            Analytics.AddUpgradeCollected();
        }

        #endregion

        #region Public Methods

        public void AddEffect(BaseEffectData data)
        {
            switch (data.Type)
            {
                case EStatusEffectType.Poison:
                    Analytics.AddPoisoned();
                    break;
                case EStatusEffectType.Weak:
                    break;
                case EStatusEffectType.Vulnerable:
                    break;
                case EStatusEffectType.Stun:
                    break;
                case EStatusEffectType.Slow:
                    break;
                case EStatusEffectType.SpeedBoost:
                    break;
                case EStatusEffectType.Invincible:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void AddCrow()
        {
            Analytics.AddCrowFound();
        }

        #endregion

        protected override void OnAwake()
        {
        }
    }
}
