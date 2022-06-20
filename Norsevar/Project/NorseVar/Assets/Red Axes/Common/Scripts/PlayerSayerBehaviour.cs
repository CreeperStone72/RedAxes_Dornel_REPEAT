using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace Norsevar
{
    public class PlayerSayerBehaviour : MonoBehaviour
    {
        private void OnEnable()
        {
            NorseGame.Instance.Register(this);
        }

        private void OnDisable()
        {
            NorseGame.Instance.Unregister<PlayerSayerBehaviour>();
        }

        public void Say(string text)
        {
            DialogueManager.instance.BarkString(text, transform);
        }
    }
}
