using UnityEngine;

namespace Norsevar.AI
{

    public class GameObjectRayProvider : MonoBehaviour, IRayProvider
    {

        #region Public Methods

        public Ray CreateRay(Transform rayOrigin, Transform target = null)
        {
            return new Ray(rayOrigin.position, target is null ? rayOrigin.forward : rayOrigin.position.GetDirection(target.position));
        }

        #endregion

    }

}
