using UnityEngine;

namespace Norsevar.Upgrade_System
{

    public class UpgradePickup : MonoBehaviour, IConsumable
    {

        #region Serialized Fields

        [SerializeField] private UpgradeData upgradeData;

        #endregion

        #region Public Methods

        public void Consume(IConsumer consumer)
        {
            consumer.ApplyUpgrade(new Upgrade(upgradeData));
            Destroy(gameObject);
        }

        #endregion

    }

}
