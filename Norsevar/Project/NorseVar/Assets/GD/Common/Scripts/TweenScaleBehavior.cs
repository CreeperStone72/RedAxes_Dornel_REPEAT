using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Norsevar
{
    public class TweenScaleBehavior : MonoBehaviour
    {
        [SerializeField] private Vector3 min;
        [SerializeField] private Vector3 max;
        [SerializeField] private float interval;

        private bool isMin = true;
        private TweenerCore<Vector3, Vector3, VectorOptions> tween;

        private void OnEnable()
        {
            if (tween == null)
                TweenScale();
            else tween.Play();
        }

        private void TweenScale()
        {
            tween = transform.DOScale(isMin ? max : min, interval);
            tween.OnComplete(() => TweenScale());
            isMin = !isMin;
        }

        private void OnDisable()
        {
            tween.Pause();
        }
    }
}
