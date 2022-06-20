using System.Collections;
using UnityEngine;

namespace Norsevar
{
    public class SlowMoManager : Singleton<SlowMoManager>
    {

        #region Private Methods

        private IEnumerator SlowTimeRoutine(float percent, float time)
        {
            StartSlowMo(percent);
            yield return new WaitForSecondsRealtime(time);
            StopSlowMo();
        }

        #endregion

        #region Protected Methods

        protected override void OnAwake()
        {
        }

        #endregion

        #region Public Methods

        public void SlowTime(TimeScaleDataSO data)
        {
            if (data.OnlyResumeTime) StopSlowMo();
            else if (data.OnlySlowTime) StartSlowMo(data.Percent);
            else
                SlowTime(data.Percent, data.Time);
        }

        public void SlowTime(float percent, float time)
        {
            StartCoroutine(SlowTimeRoutine(percent, time));
        }

        public void StartSlowMo(float percent)
        {
            Time.timeScale = percent;
        }

        public void StopSlowMo()
        {
            Time.timeScale = 1;
        }

        #endregion

    }
}
