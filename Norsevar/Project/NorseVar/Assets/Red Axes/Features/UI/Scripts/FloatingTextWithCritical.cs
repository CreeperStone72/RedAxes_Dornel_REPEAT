using MoreMountains.Feedbacks;
using UnityEngine;

namespace Norsevar.UI
{
    public class FloatingTextWithCritical : MMFeedbackFloatingText, IFeedback
    {

        #region Private Fields

        private bool _isCrit;

        #endregion

        #region Protected Methods

        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            Channel = _isCrit ? 1 : 0;
            base.CustomPlayFeedback(position, feedbacksIntensity);
        }

        #endregion

        #region Public Methods

        public void SetIsCrit(bool isCrit)
        {
            _isCrit = isCrit;
        }

        #endregion

    }
}
