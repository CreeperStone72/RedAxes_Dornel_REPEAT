using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Norsevar.Menu
{

    public class AnimateDeathUI : MonoBehaviour
    {

        #region Private Fields

        private Label _label;
        private VisualElement _root;

        #endregion

        #region Serialized Fields

        [SerializeField] private UIDocument uiDocument;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            uiDocument ??= GetComponent<UIDocument>();
            _root = uiDocument.rootVisualElement;

            _label = _root.Q<Label>();
            _root.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        private void Start()
        {
            Animate();
        }

        #endregion

        #region Public Methods

        public void Animate()
        {
            _root.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            ((ITransitionAnimations)_label).Scale(1, 3);
        }

        #endregion

    }

}
