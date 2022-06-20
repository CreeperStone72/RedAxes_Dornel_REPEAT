using UnityEngine;
using UnityEngine.SceneManagement;

namespace Norsevar
{

    public class SceneController : MonoBehaviour
    {

        #region Serialized Fields

        [SerializeField] private GameObject canvas;

        [SerializeField] private GameLayout gameLayout;

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
        }

        #endregion

        #region Private Methods

        private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (SceneManager.sceneCount > 1)
                canvas.SetActive(false);
        }

        #endregion

        #region Public Methods

        public void LoadNewGame()
        {
            gameLayout.NewGame();

        }

        #endregion

    }

}
