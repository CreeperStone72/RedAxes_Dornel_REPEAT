using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Norsevar.UI
{
    public class PanelManager : MonoBehaviour
    {

        #region Serialized Fields

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;

        #endregion

        #region Public Methods

        public void SetSprite(Sprite roomTypeSprite)
        {
            image.sprite = roomTypeSprite;
        }


        public void SetText(string roomTypeName)
        {
            text.SetText(roomTypeName);
        }

        #endregion

    }
}
