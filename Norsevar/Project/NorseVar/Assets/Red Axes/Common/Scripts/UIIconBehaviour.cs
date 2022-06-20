using UnityEngine;

namespace Norsevar
{
    public class UIIconBehaviour : MonoBehaviour
    {
        private GameObject _activeGO;

        private GameObject _pcGO;
        private GameObject _consoleGO;

        private bool _usingGamepad;
        
        private void Awake()
        {
            _pcGO = transform.GetChild(0).gameObject;
            _consoleGO = transform.GetChild(1).gameObject;
        }

        private void OnEnable()
        {
            UpdateIcon();
        }

        private void UpdateIcon()
        {
            if (PlayerInputs.IsUsingGamepad)
            {
                _consoleGO.SetActive(true);
                _activeGO = _consoleGO;
                _usingGamepad = true;
            }
            else
            {
                _pcGO.SetActive(true);
                _activeGO = _pcGO;
                _usingGamepad = false;
            }
        }

        private void Update()
        {
            if (PlayerInputs.IsUsingGamepad ^ _usingGamepad)
            {
                _activeGO.SetActive(false);
                UpdateIcon();
            }
        }

        private void OnDisable()
        {
            _activeGO.SetActive(false);
        }
    }
}
