using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar
{
    [CreateAssetMenu(fileName = "TimeScaleData", menuName = "Norsevar/Data/TimeScale")]
    public class TimeScaleDataSO : ScriptableObject
    {

        #region Serialized Fields

        [SerializeField] [HideIf("onlyResumeTime")]
        private bool onlySlowTime;
        [SerializeField] [HideIf("onlySlowTime")]
        private bool onlyResumeTime;
        [SerializeField] [Range(0, 1)] [HideIf("onlyResumeTime")]
        private float percent = 1;
        [SerializeField] [Range(0.01f, 100)] [HideIf("@this.onlyResumeTime || this.onlySlowTime")]
        private float time = 1;

        #endregion

        #region Properties

        public bool OnlySlowTime => onlySlowTime;
        public bool OnlyResumeTime => onlyResumeTime;
        public float Percent => percent;
        public float Time => time;

        #endregion

    }
}
