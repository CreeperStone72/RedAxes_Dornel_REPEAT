using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Norsevar.Menu
{

    public class AnimateMenuUI : MonoBehaviour
    {

        #region Private Fields

        private VisualElement _visualElement;
        private VisualElement _root;
        private int _index;
        private int _currentGif;

        #endregion

        #region Serialized Fields

        [SerializeField] private Gif gif;

        [SerializeField] private UIDocument uiDocument;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            uiDocument ??= GetComponent<UIDocument>();
            _root = uiDocument.rootVisualElement;

            _visualElement = _root.Q<VisualElement>("background-image");
        }


        private void OnEnable()
        {
            StartCoroutine(Animate());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        #endregion

        #region Private Methods

        private IEnumerator Animate()
        {
            int count = 0;
            WaitForSecondsRealtime waitForSecondsRealtime = new(gif.GetFPS());
            StyleBackground styleBackgroundImage = _visualElement.style.backgroundImage;
            Background background = styleBackgroundImage.value;

            while (true)
            {
                background.texture = gif[count++];
                count %= gif.Size();

                styleBackgroundImage.value = background;
                _visualElement.style.backgroundImage = styleBackgroundImage;

                yield return waitForSecondsRealtime;
            }
        }

        #endregion

    }

}
