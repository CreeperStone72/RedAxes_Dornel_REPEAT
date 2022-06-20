using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.Interaction
{

    [CreateAssetMenu(menuName = "Norsevar/NPC/Items", fileName = "New Item")]
    public class ItemPool : SerializedScriptableObject
    {

        #region Serialized Fields

        [HideLabel] [PropertyOrder] [InlineProperty] [SerializeField]
        private Item[] items;

        #endregion

        #region Properties

        public Item[] Items => items;

        #endregion

        #region Public Methods

        public Item GetRandomItem()
        {
            int index = Random.Range(0, items.Length);

            return items[index];
        }

        #endregion

    }

}
