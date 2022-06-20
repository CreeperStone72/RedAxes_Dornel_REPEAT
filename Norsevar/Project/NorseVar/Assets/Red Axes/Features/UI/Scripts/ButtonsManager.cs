using System;
using Norsevar.Room_Generation;
using UnityEngine;
using UnityEngine.UI;

namespace Norsevar.UI
{
    public class ButtonsManager : MonoBehaviour
    {

        #region Delegates and Events

        public static event Action<int, int> OnButtonClicked;

        #endregion

        #region Serialized Fields

        [SerializeField] private GameObject button;

        #endregion

        #region Unity Methods

        private void Start()
        {
            int i = 0;
            foreach (RoomType roomType in RoomGeneratorApi.Instance.GetChildren())
            {
                switch (roomType.RoomName)
                {
                    case "Merchant":
                        Analytics.AddMerchantRoom();
                        break;
                    case "Upgrade":
                        Analytics.AddUpgradeRoom();
                        break;
                    default:
                        Analytics.AddFightRoom();
                        break;
                }

                GameObject instantiate = Instantiate(button, transform);

                PanelManager panelManager = instantiate.GetComponent<PanelManager>();
                panelManager.SetText(roomType.RoomName);
                panelManager.SetSprite(roomType.RoomSprite);

                int index = i;
                instantiate.GetComponent<Button>().onClick.AddListener(() => HandleAButtonClicked(index, roomType.Identifier));

                i++;
            }

        }

        #endregion

        #region Private Methods

        private static void HandleAButtonClicked(int index, int type)
        {
            OnButtonClicked?.Invoke(index, type);
        }

        #endregion

    }
}
