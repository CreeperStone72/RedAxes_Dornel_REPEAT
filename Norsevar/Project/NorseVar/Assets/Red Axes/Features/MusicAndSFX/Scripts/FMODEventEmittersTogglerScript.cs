using FMOD.Studio;
using UnityEngine;

namespace Norsevar.MusicAndSFX
{
    public class FMODEventEmittersTogglerScript : MonoBehaviour
    {
        #region Constants and Statics

        private static int _musicIndex;
        private static bool _hubMusicActive;
        private EventInstance _currentInstance;

        #endregion

        #region Serialized Fields

        [SerializeField] private FMODUnity.EventReference[] fmodEventReferences;

        #endregion

        #region Unity Methods

        private void CreateNewInstance()
        {
            _currentInstance = FMODUnity.RuntimeManager.CreateInstance(fmodEventReferences[_musicIndex]);
            _currentInstance.start();
            _currentInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        }

        private void Start()
        {
            _musicIndex = 0;
            CreateNewInstance();
        }

        private void Update()
        {
            //So the music updates to play at where sound manager is, Only needed if music is 3D
            //_currentInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

            if (_musicIndex <= -1)
                return;

            //Rid of currently playing music w/ fadeout
            _currentInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _currentInstance.release();

            CreateNewInstance();

            _musicIndex = -1;
        }

        #endregion

        #region Public Methods
        public static void ChangeMusic(MusicEnums track)
        {
            switch (track)
            {
                case MusicEnums.Ambiance:
                    _musicIndex = 0;
                    break;
                case MusicEnums.Shop:
                    _musicIndex = 1;
                    break;
                case MusicEnums.Combat:
                    _musicIndex = 2;
                    break;
                case MusicEnums.Boss:
                    _musicIndex = 3;
                    break;
                case MusicEnums.Menu:
                    _musicIndex = 4;
                    break;
                case MusicEnums.Hub:
                    _musicIndex = 5;
                    break;
            }
        }

        public static void ActivateHubMusic()
        {
            _hubMusicActive = true;

            if(_musicIndex != 5)
                ChangeMusic(MusicEnums.Hub);
        }

        public static void DeactivateHubMusic()
        {
            ChangeMusic(MusicEnums.Ambiance);
            _hubMusicActive = false;
        }

        public static bool IsInHub()
        {
            return _hubMusicActive;
        }
        #endregion

    }
}
