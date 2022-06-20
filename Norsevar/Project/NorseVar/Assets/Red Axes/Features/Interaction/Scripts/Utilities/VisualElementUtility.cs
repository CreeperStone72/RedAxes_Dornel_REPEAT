using UnityEngine.UIElements;

namespace Norsevar.Interaction.Utilities
{

    public static class VisualElementUtility
    {

        #region Private Methods

        private static void SetDisplay(this VisualElement pVisualElement, DisplayStyle pDisplayStyle)
        {
            StyleEnum<DisplayStyle> styleDisplay = pVisualElement.style.display;
            styleDisplay.value = pDisplayStyle;
            pVisualElement.style.display = styleDisplay;
        }

        #endregion

        #region Public Methods

        public static void SetActive(this VisualElement pVisualElement, bool pActive)
        {
            pVisualElement.visible = pActive;
            pVisualElement.SetDisplay(pActive ? DisplayStyle.Flex : DisplayStyle.None);
        }

        #endregion

    }

}
