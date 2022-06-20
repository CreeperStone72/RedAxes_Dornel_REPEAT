using Norsevar.Interaction.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Norsevar.Interaction
{

    public class UIManager : MonoBehaviour
    {

        #region Private Fields

        private VisualElement _shop;
        private VisualElement _dialogue;

        #endregion

        #region Serialized Fields

        [SerializeField] private DialogueUIManager dialogueUIManager;
        [SerializeField] private ShopUIManager shopUIManager;
        [SerializeField] private GameEvent unPauseGame;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            _dialogue = root.Q<VisualElement>("Dialogue");
            _shop = root.Q<VisualElement>("Shop");

            dialogueUIManager.Initialize(this, _dialogue);
            shopUIManager.Initialize(this, _shop);

            DisableUI();
        }

        #endregion

        #region Private Methods

        private void EnableDialogueUI()
        {
            _dialogue.SetActive(true);
            _shop.SetActive(false);
        }

        #endregion

        #region Public Methods

        public void DisableUI()
        {
            _dialogue.SetActive(false);
            _shop.SetActive(false);

            unPauseGame.Raise();
        }

        public void EnableShopUI(ItemPool pItemPoolAsset)
        {
            _dialogue.SetActive(false);
            _shop.SetActive(true);

            shopUIManager.PopulateList(pItemPoolAsset);
        }

        public void StartDialogue(MerchantInfo dialogue)
        {
            EnableDialogueUI();
            dialogueUIManager.StartDialogue(dialogue);
        }

        #endregion

    }

}
