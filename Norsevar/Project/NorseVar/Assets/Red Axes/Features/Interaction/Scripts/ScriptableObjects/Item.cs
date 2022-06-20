using Norsevar.Upgrade_System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.Interaction
{

    [CreateAssetMenu(menuName = "Norsevar/Item", fileName = "Item")]
    public class Item : SerializedScriptableObject
    {

        #region Serialized Fields

        [SerializeField]
        private Sprite icon;

        [SerializeField]
        private new string name;

        [SerializeField] [TextArea(3, 6)]
        private string description;

        [SerializeField]
        private int cost;

        [SerializeField] private Mesh mesh;
        [SerializeField] private float scale = 1;
        [SerializeField] private Material material;

        [SerializeField] private UpgradeData upgradeData;
        [SerializeField] private ItemRarity itemRarity;

        #endregion

        #region Properties

        public Material Material => material;

        public Sprite Icon => icon;

        public string Name => name;

        public string Description => description;

        public int Cost => cost;

        public Mesh Mesh => mesh;
        public float Scale => scale;

        public UpgradeData UpgradeData => upgradeData;

        public ItemRarity ItemRarity => itemRarity;

        #endregion

    }

}
