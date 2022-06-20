using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar
{
    public class SpriteRendererFader : MonoBehaviour
    {

        #region Private Fields

        private SpriteRenderer _sr;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
        }

        #endregion

        #region Public Methods

        public void Fade(float target, float duration)
        {
            DOTween.ToAlpha(() => _sr.color, c => _sr.color = c, target, duration);
        }

        [Button]
        public void FadeIn()
        {
            DOTween.ToAlpha(() => _sr.color, c => _sr.color = c, 1, 1);
        }

        [Button]
        public void FadeOut()
        {
            DOTween.ToAlpha(() => _sr.color, c => _sr.color = c, 0, 1);
        }

        #endregion

    }
}
