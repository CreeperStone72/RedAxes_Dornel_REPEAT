using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Norsevar.UI
{
    public class ItemInfo : MonoBehaviour
    {

        #region Serialized Fields

        [SerializeField] private Image effectSpriteRenderer;
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;

        #endregion

        #region Public Methods

        [Button]
        public void SetSprite(Sprite sprite, int effectDataStackCount)
        {
            effectSpriteRenderer.sprite = sprite;
            textMeshProUGUI.SetText(effectDataStackCount > 1 ? effectDataStackCount.ToString() : string.Empty);
        }

        #endregion

    }
}
