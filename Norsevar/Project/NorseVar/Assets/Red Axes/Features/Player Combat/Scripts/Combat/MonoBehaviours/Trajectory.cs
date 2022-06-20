using UnityEngine;

namespace Norsevar.Combat
{
    public class Trajectory : MonoBehaviour
    {

        #region Serialized Fields

        [SerializeField] private LineRenderer lineRenderer;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            lineRenderer.positionCount = 2;
            Deactivate();
        }

        #endregion

        #region Public Methods

        public void Activate()
        {
            lineRenderer.enabled = true;
        }

        public Vector3 CalculatePositionInTime(float maxDistance, float currentTime, float maxTime)
        {
            Vector3 startPos = transform.position;
            lineRenderer.SetPosition(0, startPos);

            Vector3 currentPos = Vector3.Lerp(startPos, startPos + transform.forward * maxDistance, currentTime / maxTime);
            lineRenderer.SetPosition(1, currentPos);

            return currentPos;
        }

        public void Deactivate()
        {
            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.zero);
            lineRenderer.enabled = false;
        }

        #endregion

    }
}
