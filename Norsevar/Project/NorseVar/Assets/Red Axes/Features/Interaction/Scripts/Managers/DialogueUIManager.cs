using System;
using System.Collections;
using Norsevar.Interaction.DialogueSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Norsevar.Interaction
{

    [Serializable]
    public class DialogueUIManager
    {

        #region Private Fields

        private Label _sentence;
        private Label _trader;
        private VisualElement _responseArea;
        private UIManager _uiManager;
        private ItemPool _itemPool;
        private WaitForSeconds _letterTime;
        private WaitForSeconds _sentenceWaitTime;

        #endregion

        #region Serialized Fields

        [SerializeField] private float letterSpeed = .1f;
        [SerializeField] private float sentenceSpeed = 1.5f;
        [SerializeField] private VisualTreeAsset responseButton;

        #endregion

        #region Private Methods

        private void CreateResponses(DSDialogueSo conversation)
        {
            if (conversation.DialogueType is DSDialogueType.Single or DSDialogueType.Action)
            {
                _uiManager.StartCoroutine(DelayedNextDialogueCoroutine(conversation.GetNextDialogue()));
                return;
            }

            foreach (DSDialogueChoiceData response in conversation.Choices)
            {
                TemplateContainer child = responseButton.CloneTree();
                _responseArea.Add(child);
                Button button = child.Q<Button>();
                button.text = response.Text;
                button.clickable.clicked += () => NextConversation(response.NextDialogue);
            }
        }

        private IEnumerator DelayedNextDialogueCoroutine(DSDialogueSo dialogue)
        {
            yield return _sentenceWaitTime;
            NextConversation(dialogue);
        }

        private void NextConversation(DSDialogueSo dialogue)
        {
            if (dialogue == null)
            {
                _uiManager.DisableUI();
                return;
            }

            if (dialogue.DialogueType == DSDialogueType.Action)
            {
                switch (dialogue.Action)
                {
                    case EAction.Shop:
                        _uiManager.EnableShopUI(_itemPool);
                        return;
                    case EAction.Leave:
                        _uiManager.DisableUI();
                        return;
                }
            }

            _responseArea.Clear();
            _uiManager.StartCoroutine(Talk(dialogue));
        }

        private IEnumerator Talk(DSDialogueSo conversation)
        {
            _sentence.text = string.Empty;

            foreach (char letter in conversation.Text)
            {
                _sentence.text += letter;
                yield return _letterTime;
            }

            CreateResponses(conversation);
        }

        #endregion

        #region Public Methods

        public void Initialize(UIManager uiManager, VisualElement dialogue)
        {
            _uiManager = uiManager;
            _sentence = dialogue.Q<Label>("Trader-Sentence-Label");
            _trader = dialogue.Q<Label>("Trader-Info-Name-Label");
            _responseArea = dialogue.Q<VisualElement>("Decision");

            _letterTime = new WaitForSeconds(letterSpeed);
            _sentenceWaitTime = new WaitForSeconds(sentenceSpeed);
        }

        public void StartDialogue(MerchantInfo merchantInfo)
        {
            _trader.text = merchantInfo.GetTraderName();
            _itemPool = merchantInfo.ItemPool;
            NextConversation(merchantInfo.GetDialogue());
        }

        #endregion

    }

}
