using UnityEngine;
using UnityEngine.SceneManagement;

namespace Norsevar
{
    public class LoadingManager : MonoBehaviour
    {

        #region Private Fields

        private int _sceneCount;
        private int _count;

        #endregion

        #region Serialized Fields

        [SerializeField] private GameLayout gameLayout;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #endregion

        #region Private Methods

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            bool levelScene = scene.name.Equals(gameLayout.GetCurrentLoadingSceneName());
            bool roomSelectionScene = scene.name.Equals(gameLayout.GetRoomSelectionSceneName());

            if (loadSceneMode == LoadSceneMode.Additive)
                ++_count;

            if (_count != 2 && !levelScene && !roomSelectionScene)
                return;

            Analytics.InitStartTime();
            UnloadLoadingScene();


        }

        private void UnloadLoadingScene()
        {
            SceneManager.UnloadSceneAsync(gameLayout.GetLoadingSceneName());
            Time.timeScale = 1;
        }

        #endregion

    }
}
