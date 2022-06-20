using UnityEngine;

namespace Norsevar.UI
{
    public class PlayerUIManager : MonoBehaviour
    {

        #region Serialized Fields

        [SerializeField] private GameObject gameUI;
        [SerializeField] private GameObject pauseUI;

        #endregion

        #region Unity Methods

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ToggleUI();
        }

        #endregion

        #region Private Methods

        public void ToggleUI()
        {
            gameUI.SetActive(pauseUI.activeSelf);
            pauseUI.SetActive(!gameUI.activeSelf);

            Time.timeScale = pauseUI.activeSelf ? 0 : 1;
        }

        #endregion

    }
}
