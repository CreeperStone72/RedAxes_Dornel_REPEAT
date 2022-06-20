using UnityEngine;

namespace Norsevar.AI
{

    public static class QuaternionExtension
    {

        #region Public Methods

        public static Quaternion GetRandomRotation()
        {
            return Quaternion.Euler(0, Random.Range(0, 360), 0);
        }

        #endregion

    }

}
