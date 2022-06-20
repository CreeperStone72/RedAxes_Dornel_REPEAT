using UnityEngine;

namespace Norsevar
{
    public class BeginningBehaviour : MonoBehaviour
    {
        private bool pickedUpOnce = false;
        
        private void Awake()
        {
            NorseGame.Instance.RegisterAction(ENorseGameEvent.Player_EquipWeapon, OnPickup);
            NorseGame.Instance.RegisterAction(ENorseGameEvent.Player_EquipShield, OnPickup);
        }

        private void OnPickup()
        {
            if (!pickedUpOnce) pickedUpOnce = true;
            else
            {
                NorseGame.Instance.Get<PlayerSayerBehaviour>().Say("Now i'm prepared to go!");
                NorseGame.Instance.UnregisterAction(ENorseGameEvent.Player_EquipWeapon, OnPickup);
                NorseGame.Instance.UnregisterAction(ENorseGameEvent.Player_EquipShield, OnPickup);
                gameObject.SetActive(false);
            }
        }
    }
}
