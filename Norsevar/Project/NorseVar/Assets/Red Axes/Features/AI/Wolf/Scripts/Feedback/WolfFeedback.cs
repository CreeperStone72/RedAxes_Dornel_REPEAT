using Norsevar.VFX;
using UnityEngine;

namespace Norsevar.AI
{
    public class WolfFeedback : MonoBehaviour
    {

        #region Serialized Fields

        [SerializeField] private WolfTeeth wolfTeeth;

        #endregion

        #region Unity Methods

        private void Start()
        {
            wolfTeeth.Hide();
        }

        #endregion

        #region Public Methods

        public void Bite(float disappearAfterSeconds)
        {
            wolfTeeth.Bite();
            this.ExecuteInSeconds(() => wolfTeeth.Hide(), disappearAfterSeconds);
        }

        public void PrepareBite()
        {
            wolfTeeth.Reset();
            wolfTeeth.FadeIn();
            wolfTeeth.BeforeBite();
        }

        #endregion

    }
}
