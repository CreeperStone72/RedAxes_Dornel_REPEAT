using UnityEngine;

namespace Norsevar.AI
{

    public class AIRaySelector : MonoBehaviour, ISelector
    {

        #region Private Fields

        private RaycastHit _raycastHit;
        private Transform _transformSelection;

        #endregion

        #region Serialized Fields

        [SerializeField] private float radius;

        #endregion

        #region Public Methods

        public void Check(Ray ray, float maxDistance, LayerMask layerMask)
        {
            _transformSelection = null;

            RaycastHit[] results = new RaycastHit[1];
            int size = Physics.SphereCastNonAlloc(
                ray.origin,
                radius,
                ray.direction,
                results,
                maxDistance,
                layerMask,
                QueryTriggerInteraction.Collide);

            if (size == 0) return;

            _transformSelection = results[0].transform;
            _raycastHit = results[0];
        }

        public RaycastHit GetHitInfo()
        {
            return _raycastHit;
        }

        public Transform GetSelection()
        {
            return _transformSelection;
        }

        #endregion

    }

}
