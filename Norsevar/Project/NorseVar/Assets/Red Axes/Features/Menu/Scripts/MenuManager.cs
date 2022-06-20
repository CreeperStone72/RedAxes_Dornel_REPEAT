#nullable enable
using FMODUnity;
using UnityEngine;
using UnityEngine.UIElements;

namespace Norsevar.Menu
{
    public class MenuManager
    {

        #region Private Fields

        private readonly UIManager _uiManager;

        private readonly Button _playButton;
        private readonly Button _settingsButton;
        private readonly Button? _presentationButton;
        private readonly Button _quitButton;

        #endregion

        #region Constructors

        public MenuManager(UIManager uiManager, UIDocument uiDocument)
        {
            VisualElement root = uiDocument.rootVisualElement;

            _playButton = root.Q<Button>("play-button");
            _settingsButton = root.Q<Button>("settings-button");
            _presentationButton = root.Q<Button>("presentation-button");
            _quitButton = root.Q<Button>("quit-button");


            _playButton.clicked += PlayClicked;
            _settingsButton.clicked += SettingsClicked;

            _playButton.RegisterCallback<PointerEnterEvent>(OnHover);
            _settingsButton.RegisterCallback<PointerEnterEvent>(OnHover);

            if (_presentationButton is not null)
            {
                _presentationButton.RegisterCallback<PointerEnterEvent>(OnHover);
                _presentationButton.clicked += PresentationClicked;
            }

            _quitButton.clicked += QuitClicked;
            _quitButton.RegisterCallback<PointerEnterEvent>(OnHover);

            _uiManager = uiManager;
        }

        #endregion

        #region Private Methods

        private void OnHover(PointerEnterEvent evt)
        {
        }

        private void PlayClicked()
        {
            ButtonClickSfx();
            _uiManager.LoadGame();
        }

        private void PresentationClicked()
        {
            ButtonClickSfx();
            _uiManager.ShowPresentationCanvas();
        }

        private void QuitClicked()
        {
            ButtonClickSfx();
            _uiManager.Quit();

        }

        private void SettingsClicked()
        {
            ButtonClickSfx();
            _uiManager.EnableSettings();
        }

        #endregion

        #region Public Methods

        public static void ButtonClickSfx()
        {
            RuntimeManager.PlayOneShot("event:/SFX/UI/Buttons/UI_Button02", new Vector3(0, 0, 0));
        }

        public void Destroy()
        {
            _playButton.clicked -= PlayClicked;
            _settingsButton.clicked -= SettingsClicked;
            if (_presentationButton != null)
                _presentationButton.clicked -= PresentationClicked;
            _quitButton.clicked -= QuitClicked;

            _playButton.UnregisterCallback<PointerEnterEvent>(OnHover);
            _settingsButton.UnregisterCallback<PointerEnterEvent>(OnHover);
            _quitButton.UnregisterCallback<PointerEnterEvent>(OnHover);
            _presentationButton?.UnregisterCallback<PointerEnterEvent>(OnHover);
        }

        #endregion

    }
}
