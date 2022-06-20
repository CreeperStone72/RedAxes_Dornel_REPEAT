using UnityEngine;

namespace Norsevar.AI
{

    public interface ISelector
    {

        #region Public Methods

        void Check(Ray ray, float maxDistance, LayerMask layerMask);

        RaycastHit GetHitInfo();

        Transform GetSelection();

        #endregion

    }

}
