using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Norsevar.Menu
{

    public class SettingsManager
    {

        #region Private Fields

        private readonly UIManager _uiManager;
        private readonly Button _closeButton;
        private readonly VisualElement _gif;

        private readonly Gif[] _gifs;
        private Coroutine _coroutine;

        #endregion

        #region Constructors

        public SettingsManager(UIManager uiManager, UIDocument uiDocument, Gif[] texture2Ds)
        {
            VisualElement root = uiDocument.rootVisualElement;
            _gifs = texture2Ds;

            _gif = root.Q<VisualElement>("gif");

            Button wasdButton = root.Q<Button>("wasd");
            wasdButton.clicked += () => SetGif();
            Button spaceButton = root.Q<Button>("space");
            spaceButton.clicked += () => SetGif(1);
            Button shiftButton = root.Q<Button>("shift");
            shiftButton.clicked += () => SetGif(2);
            Button pressButton = root.Q<Button>("press");
            pressButton.clicked += () => SetGif(3);
            Button holdButton = root.Q<Button>("hold");
            holdButton.clicked += () => SetGif(4);

            _closeButton = root.Q<Button>("close-button");
            _closeButton.clicked += CloseSettings;

            _uiManager = uiManager;

            SetGif();
        }

        #endregion

        #region Private Methods

        private void CloseSettings()
        {
            MenuManager.ButtonClickSfx();
            _uiManager.DisableSettings();
        }

        private IEnumerator RunGif(int id)
        {
            int count = 0;
            Gif gif = _gifs[id];
            WaitForSecondsRealtime waitForSecondsRealtime = new(gif.GetFPS());
            StyleBackground styleBackgroundImage = _gif.style.backgroundImage;
            Background background = styleBackgroundImage.value;

            while (true)
            {
                background.texture = gif[count++];
                count %= gif.Size();

                styleBackgroundImage.value = background;
                _gif.style.backgroundImage = styleBackgroundImage;

                yield return waitForSecondsRealtime;
            }
        }

        private void SetGif(int id = 0)
        {
            if (_coroutine != null)
                _uiManager.StopCoroutine(_coroutine);
            _coroutine = _uiManager.StartCoroutine(RunGif(id));
        }

        #endregion

        #region Public Methods

        public void Destroy()
        {
            _closeButton.clicked -= CloseSettings;
        }

        #endregion

    }


}
