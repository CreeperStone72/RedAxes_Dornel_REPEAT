using System;
using DG.Tweening;
using Norsevar.Combat;
using Norsevar.Upgrade_System;
using TMPro;
using UnityEngine;

namespace Norsevar.Interaction
{

    public class ItemBehaviour : MonoBehaviour
    {

        #region Delegates and Events

        public static event Action<Item> OnItemPickUp;

        #endregion

        #region Private Fields

        private Item _item;
        private Transform _meshTransform;
        private bool _isFree;

        private ItemRarity _rarity;
        private PlayerController _playerController;
        private RectTransform _uiPanel;

        #endregion

        #region Serialized Fields

        [SerializeField] private GameEvent onFreeItemPickup;
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;

        [SerializeField] private GameObject commonVFX;
        [SerializeField] private GameObject rareVFX;
        [SerializeField] private GameObject legendaryVFX;

        [SerializeField] private GameObject uiPanel;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        #endregion

        #region Properties

        private int Cost => _isFree ? 0 : _item.Cost;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _meshTransform = meshFilter.gameObject.transform;
            _playerController = NorseGame.Instance.Get<PlayerController>();
            _uiPanel = uiPanel.GetComponent<RectTransform>();
        }

        #endregion

        #region Private Methods

        private void ConsumeBuy()
        {
            _playerController.ApplyUpgrade(new Upgrade(_item.UpgradeData));
        }

        private void Initialize()
        {
            meshFilter.transform.localScale = Vector3.one * _item.Scale;
            meshFilter.mesh = _item.Mesh;
            meshRenderer.material = _item.Material;

            _rarity = _item.ItemRarity;

            nameText.SetText(_item.UpgradeData.Name);
            descriptionText.SetText(_item.UpgradeData.Description);

            ShowRarityVFX();
        }

        private void ResetPosition()
        {
            Vector3 meshTransformPosition = _meshTransform.localPosition;
            meshTransformPosition.y = 0;
            _meshTransform.localPosition = meshTransformPosition;
        }

        private void ShowRarityVFX()
        {
            switch (_rarity)
            {
                case ItemRarity.Common:
                    commonVFX.SetActive(true);
                    break;
                case ItemRarity.Rare:
                    rareVFX.SetActive(true);
                    break;
                case ItemRarity.Legendary:
                    legendaryVFX.SetActive(true);
                    break;
            }
        }

        #endregion

        #region Public Methods

        public void AnimateItem()
        {
            _uiPanel.DOAnchorPos(new Vector2(0, 0), 1f);
            Vector3 endValue = _meshTransform.rotation.eulerAngles + new Vector3(0, 180, 0);
            _meshTransform.DORotate(endValue, 1f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
            float endValuePosition = _meshTransform.transform.position.y + .5f;
            _meshTransform.DOMoveY(endValuePosition, .5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutSine);
        }

        public void BuyItem()
        {
            if (_isFree)
            {
                NorseGame.Instance.RaiseEvent(ENorseGameEvent.Interaction_GetUpgrade, transform.position);
                onFreeItemPickup.Raise();
            }
            else
                NorseGame.Instance.RaiseEvent(ENorseGameEvent.Interaction_BuyItem, transform.position);


            ConsumeBuy();
            OnItemPickUp?.Invoke(_item);
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Interaction_PickupUpgrade, NorseGame.Instance.Get<PlayerController>().gameObject);
            Destroy(gameObject);
        }

        public void SetFree()
        {
            _isFree = true;
        }

        public void SetItem(Item item)
        {
            _item = item;
            Initialize();
        }

        public void StopAnimation()
        {
            _meshTransform.DOKill();
            _uiPanel.DOAnchorPos(new Vector2(0, 300), 1f);
            ResetPosition();
        }

        #endregion

    }

}
