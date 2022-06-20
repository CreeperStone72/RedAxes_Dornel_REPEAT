using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Norsevar.UI
{
    public class UIUpgradeManager : MonoBehaviour
    {

        #region Private Fields

        private float _showPosition;
        private float _hidePosition;
        private RectTransform _rectTransform;
        private float _moveDistance;
        private bool _notPlaying = true;

        #endregion

        #region Serialized Fields

        [SerializeField] private Image upgradeIcon;
        [SerializeField] private Sprite[] sprites;
        [SerializeField] private TextMeshProUGUI upgradeName;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Start()
        {
            SetUIPosition();
        }

        private void OnEnable()
        {
            UIHoverManager.OnHoverUI += HandleOnHoverUI;
        }


        private void OnDisable()
        {
            UIHoverManager.OnHoverUI -= HandleOnHoverUI;
        }

        #endregion

        #region Private Methods

        private void HandleOnHoverUI(string[] s)
        {
            if (s.Contains(gameObject.name))
                ShowWindow();
            else
                HideWindow();
        }

        private void HideWindow()
        {
            if (!_notPlaying)
                return;

            _notPlaying = false;
            _rectTransform.DOMoveX(_hidePosition, 1).SetEase(Ease.OutBounce).OnComplete(() => _notPlaying = true);
        }

        private void SetUIPosition()
        {
            _rectTransform.ForceUpdateRectTransforms();
            _moveDistance = _rectTransform.rect.width * 4 / 5f;
            _showPosition = 0;

            _hidePosition = 0 - _moveDistance;
        }

        private void ShowWindow()
        {
            if (!_notPlaying)
                return;

            _notPlaying = false;
            _rectTransform.DOMoveX(_showPosition, 1).SetEase(Ease.OutBounce).OnComplete(() => _notPlaying = true);
        }

        #endregion

        #region Public Methods

        [Button]
        public void InitUpgrade(int sprite, string s)
        {
            upgradeIcon.sprite = sprites[sprite];
            upgradeName.SetText(s);
        }

        #endregion

    }
}
