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
                NorseGame.Instance.RaiseEvent(ENorseGameEvent.Dialogues_Erik_FullyEquipped);
                NorseGame.Instance.UnregisterAction(ENorseGameEvent.Player_EquipWeapon, OnPickup);
                NorseGame.Instance.UnregisterAction(ENorseGameEvent.Player_EquipShield, OnPickup);
                gameObject.SetActive(false);
            }
        }
    }
}
