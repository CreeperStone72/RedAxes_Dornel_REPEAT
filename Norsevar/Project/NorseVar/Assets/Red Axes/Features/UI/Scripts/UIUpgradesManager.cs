using System.Collections.Generic;
using Norsevar.Interaction;
using Norsevar.Upgrade_System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.UI
{
    public class UIUpgradesManager : MonoBehaviour
    {

        #region Private Fields

        private Dictionary<UpgradeData, GameObject> _upgrades;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _upgrades = new Dictionary<UpgradeData, GameObject>();
            ResetChildren();
        }

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

        [Button]
        private void HandleUpgradePickUp(Item upgradeData)
        {
            if (_upgrades.ContainsKey(upgradeData.UpgradeData))
                return;

            Transform child = transform.GetChild(_upgrades.Count);
            child.gameObject.SetActive(true);
            int sprite = upgradeData.UpgradeData.Icon;
            string dataName = upgradeData.UpgradeData.Name;
            child.GetComponent<UIUpgradeManager>().InitUpgrade(sprite, dataName);
            _upgrades.Add(upgradeData.UpgradeData, child.gameObject);
        }



        private void ResetChildren()
        {
            if (transform.childCount == 0)
                return;

            foreach (Transform child in transform)
                child.gameObject.SetActive(false);
        }

        #endregion

    }

}
