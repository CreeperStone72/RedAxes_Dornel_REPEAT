using UnityEngine;

namespace Norsevar
{
    public class DisableChildOnFightBehavior : MonoBehaviour
    {
        private GameObject child;

        private void Awake()
        {
            child = transform.GetChild(0).gameObject;
            NorseGame.Instance.RegisterAction(ENorseGameEvent.World_FightStart, OnFightStart);
            NorseGame.Instance.RegisterAction(ENorseGameEvent.World_FightEnd, OnFightEnd);
        }
        
        private void OnDestroy()
        {
            NorseGame.Instance.UnregisterAction(ENorseGameEvent.World_FightStart, OnFightStart);
            NorseGame.Instance.UnregisterAction(ENorseGameEvent.World_FightEnd, OnFightEnd);
        }

        private void OnFightStart()
        {
            child.SetActive(false);
        }

        private void OnFightEnd()
        {
            child.SetActive(true);
        }
    }
}
