using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Norsevar.Interaction
{

    [Serializable]
    public class ShopUIManager
    {

        #region Private Fields

        private UIManager _uiManager;
        private ListView _listView;
        private VisualElement _itemVisual;
        private Button _close;

        #endregion

        #region Serialized Fields

        [SerializeField] private VisualTreeAsset listItem;
        [SerializeField] private VisualTreeAsset itemShop;

        #endregion

        #region Private Methods

        private static void BindItem(ItemPool pItemPoolAsset, VisualElement pElement, int pIndex)
        {
            pElement.Q<Label>().text = pItemPoolAsset.Items[pIndex].Name;
            pElement.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(pItemPoolAsset.Items[pIndex].Icon);
        }

        private static void BuyItem(Item pItem)
        {
            Debug.Log($"Bought a {pItem.Name} for {pItem.Cost}$");
        }

        private void CloseInformationPanel()
        {
            _listView.selectedIndex = -1;
            _itemVisual.Clear();
        }

        private void CloseShop()
        {
            _uiManager.DisableUI();
        }

        private VisualElement MakeItem()
        {
            return listItem.CloneTree();
        }

        private void PopulateInformationPage()
        {
            _itemVisual.Clear();

            TemplateContainer templateContainer = itemShop.CloneTree();

            if (_listView.selectedItem is not Item item) return;
            templateContainer.Q<Label>("Name").text = item.Name;
            templateContainer.Q<VisualElement>("Icon").style.backgroundImage = new StyleBackground(item.Icon);
            templateContainer.Q<Label>("Price").text = $"Price: {item.Cost}";

            templateContainer.style.flexGrow = 1;

            templateContainer.Q<Button>("Buy").clickable.clicked += () => BuyItem(item);
            templateContainer.Q<Button>("Close").clickable.clicked += CloseInformationPanel;

            _itemVisual.Add(templateContainer);
        }

        #endregion

        #region Public Methods

        public void Initialize(UIManager pUIManager, VisualElement pShop)
        {
            _uiManager = pUIManager;
            _listView = pShop.Q<ListView>("Item-List");
            _itemVisual = pShop.Q<VisualElement>("Right-Side");
            _close = pShop.Q<Button>("Close-Button");

            _close.clicked += CloseShop;
        }

        public void PopulateList(ItemPool pItemPoolAsset)
        {
            const int itemHeight = 100;

            _listView.itemsSource = pItemPoolAsset.Items;
            _listView.bindItem = (pElement, pIndex) => BindItem(pItemPoolAsset, pElement, pIndex);
            _listView.makeItem = MakeItem;
            _listView.fixedItemHeight = itemHeight;
            _listView.onSelectionChange += _ => PopulateInformationPage();
        }

        #endregion

    }

}
