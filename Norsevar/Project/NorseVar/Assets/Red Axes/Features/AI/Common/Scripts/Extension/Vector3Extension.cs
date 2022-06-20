using UnityEngine;

namespace Norsevar.AI
{

    public static class Vector3Extension
    {

        #region Public Methods

        public static Vector3 AddRandomPositionVector(this Vector3 pPosition, float pWidth, float pDepth, float pHeight = 0)
        {
            float x = Random.Range(-pWidth / 2, pWidth / 2 + 1);
            float y;
            float z = Random.Range(-pDepth / 2, pDepth / 2 + 1);

            if (pHeight == 0)
                y = 0;
            else
                y = Random.Range(-pHeight / 2, pHeight / 2 + 1);

            return pPosition + new Vector3(x, y, z);
        }

        #endregion

    }

}
