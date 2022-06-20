using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Norsevar.VFX
{
    public class WolfTeeth : MonoBehaviour
    {

        #region Private Fields

        private SpriteRendererFader teethUpperFader;
        private SpriteRendererFader teethLowerFader;

        #endregion

        #region Serialized Fields

        [SerializeField] private GameObject teethUpper;
        [SerializeField] private GameObject teethLower;

        [SerializeField] private Transform teethUpperDefaultPos;
        [SerializeField] private Transform teethUpperPreparePos;
        [SerializeField] private Transform teethUpperBitePos;

        [SerializeField] private Transform teethLowerDefaultPos;
        [SerializeField] private Transform teethLowerPreparePos;
        [SerializeField] private Transform teethLowerBitePos;

        [FormerlySerializedAs("_particleSystem")] [SerializeField]
        private ParticleSystem system;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            teethLowerFader = teethLower.GetComponent<SpriteRendererFader>();
            teethUpperFader = teethUpper.GetComponent<SpriteRendererFader>();
        }

        [Button]
        public void Reset(float duration = 0)
        {
            if (duration == 0)
            {
                teethUpper.transform.localPosition = teethUpperDefaultPos.localPosition;
                teethLower.transform.localPosition = teethLowerDefaultPos.localPosition;
            }
            else
            {
                teethUpper.transform.DOLocalMove(teethUpperDefaultPos.localPosition, duration).SetEase(Ease.InOutBack);
                teethLower.transform.DOLocalMove(teethLowerDefaultPos.localPosition, duration).SetEase(Ease.InOutBack);
            }
        }

        #endregion

        #region Private Methods

        private IEnumerator PlayBitePS()
        {
            yield return new WaitForSeconds(0.5f);
            system.Play();
        }

        #endregion

        #region Public Methods

        [Button]
        public void BeforeBite()
        {
            teethUpper.transform.DOLocalMove(teethUpperPreparePos.localPosition, 2).SetEase(Ease.InOutBack);
            teethLower.transform.DOLocalMove(teethLowerPreparePos.localPosition, 2).SetEase(Ease.InOutBack);
        }

        [Button]
        public void Bite()
        {
            teethUpper.transform.DOLocalMove(teethUpperBitePos.localPosition, 0.5f).SetEase(Ease.InOutBack);
            teethLower.transform.DOLocalMove(teethLowerBitePos.localPosition, 0.5f).SetEase(Ease.InOutBack);
            StartCoroutine(PlayBitePS());
        }

        [Button]
        public void FadeIn()
        {
            teethLowerFader.FadeIn();
            teethUpperFader.FadeIn();
        }

        [Button]
        public void FadeOut()
        {
            teethLowerFader.FadeOut();
            teethUpperFader.FadeOut();
        }

        [Button]
        public void Hide()
        {
            teethLowerFader.Fade(0, 0.1f);
            teethUpperFader.Fade(0, 0.1f);
        }

        #endregion

    }
}
