using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Norsevar.MusicAndSFX
{
    public class MusicVolumeControlScript : MonoBehaviour
    {

        #region Constants and Statics

        private const string _busPath = "bus:/Music";

        #endregion

        #region Private Fields

        private Bus _bus;
        private bool _isActive;

        #endregion

        #region Serialized Fields

        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private GameObject _volumeScreen;

        #endregion

        #region Unity Methods

        private void Start()
        {
            _bus = RuntimeManager.GetBus(_busPath);
        }

        private void Update()
        {
            if(Input.GetKeyUp("m"))
            {
                _isActive = !_isActive;
                _volumeScreen.SetActive(_isActive);
            }

            _bus.setVolume(_volumeSlider.value);        
        }

        #endregion

    }
}
