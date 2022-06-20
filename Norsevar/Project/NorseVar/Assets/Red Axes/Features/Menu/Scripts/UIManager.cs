using UnityEngine;
using UnityEngine.UIElements;

namespace Norsevar.Menu
{
    public class UIManager : MonoBehaviour
    {

        #region Private Fields

        private VisualElement _rootMenu;
        private VisualElement _rootSettings;
        private MenuManager _menu;
        private SettingsManager _settings;

        #endregion

        #region Serialized Fields

        [SerializeField] private UIDocument menuUiDocument;
        [SerializeField] private UIDocument settingsUiDocument;
        [SerializeField] private GameLayout gameLayout;
        [SerializeField] private Canvas presentationCanvas;
        [SerializeField] private Gif[] gifs;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _rootMenu = menuUiDocument.rootVisualElement;
            _rootSettings = settingsUiDocument.rootVisualElement;
        }

        private void OnEnable()
        {
            _menu = new MenuManager(this, menuUiDocument);
            _settings = new SettingsManager(this, settingsUiDocument, gifs);

            DisableSettings();
        }

        private void OnDisable()
        {
            _menu.Destroy();
            _settings.Destroy();
        }

        #endregion

        #region Private Methods

        private void DisableMenu()
        {
            _rootMenu.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        #endregion

        #region Public Methods

        public void DisableSettings()
        {
            _rootSettings.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        public void EnableMenu()
        {
            _rootMenu.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        }

        public void EnableSettings()
        {
            _rootSettings.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        }

        public void LoadGame()
        {
            Analytics.AddRoundPlayed();
            gameLayout.NewGame();
        }

        public void ShowPresentationCanvas()
        {
            DisableMenu();
            presentationCanvas?.gameObject.SetActive(true);
        }

        #endregion

        public void Quit()
        {
            GameLayout.Quit();
        }
    }
}
