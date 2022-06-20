using UnityEngine;

namespace Norsevar.AI
{

    public interface IRayProvider
    {

        #region Public Methods

        Ray CreateRay(Transform pRayOrigin, Transform pTarget = null);

        #endregion

    }

}
