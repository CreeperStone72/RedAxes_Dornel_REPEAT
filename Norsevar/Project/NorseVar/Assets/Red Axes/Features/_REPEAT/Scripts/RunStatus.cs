using FMOD.Studio;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Norsevar._REPEAT.Scripts
{
    public class RunStatus : MonoBehaviour {
        public GameScene hubScene;

        public RunVariables runVariables;

        public UnityEvent onFirstRespawn;

        public UnityEvent onSubsequentRespawn;

        private EventInstance _currentInstance;

        private static Scene ActiveScene => SceneManager.GetActiveScene();

        private bool IsHub => ActiveScene.name == hubScene.Name;

        private const string Hub = "event:/Music/MusicTimelines/HubMusicTimeline";
        private const string Ambiance = "event:/Music/MusicTimelines/AmbianceTimeline";

        private void Start() { CreateNewInstance(Hub); }
        
        private void CreateNewInstance(string path)
        {
            _currentInstance = FMODUnity.RuntimeManager.CreateInstance(path);
            _currentInstance.start();
            _currentInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        }

        public void OnRespawn() {
            if (!IsHub) return;
            runVariables.OnRespawn(ActiveScene, SetHubMusic, onFirstRespawn, onSubsequentRespawn);
        }

        public void ReloadScene() {
            StopMusic();
            SceneManager.LoadScene(ActiveScene.name);
        }
        
        public void SetHubMusic() { if (IsHub) SetMusic(Hub); }
        
        public void SetAmbiance() { SetMusic(Ambiance); }

        private void SetMusic(string path) {
            StopMusic();
            CreateNewInstance(path);
        }

        private void StopMusic() {
            _currentInstance.stop(STOP_MODE.ALLOWFADEOUT);
            _currentInstance.release();
        }
    }
}
