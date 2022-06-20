using UnityEngine;

namespace Norsevar.Combat
{
    [CreateAssetMenu(fileName = "PlayerMovementData", menuName = "Norsevar/Player/Data/MovementData")]
    public class PlayerMovementData : ScriptableGameObject
    {

        #region Serialized Fields

        [Header("Input")]
        [SerializeField] private float smoothInputSpeed = .2f;
        
        [Header("Mouse Marker")] 
        [SerializeField] private GameObject mouseRaycastPlanePrefab;
        [SerializeField] private LayerMask mousePositionLayerMask;
        
        [SerializeField] private GameObject mousePositionMarkerPrefab;
        [SerializeField] private Vector3 mouseMarkerOffset;
        [SerializeField] private float mouseMarkerMaxDistance;

        [Header("Dash")]
        [SerializeField] private float dashSpeed;
        [SerializeField] private float dashTime;

        #endregion

        #region Properties

        public float SmoothInputSpeed => smoothInputSpeed;
        public float DashSpeed => dashSpeed;
        public float DashTime => dashTime;
        public GameObject MouseRaycastPlanePrefab => mouseRaycastPlanePrefab;
        public LayerMask MousePositionLayerMask => mousePositionLayerMask;
        public GameObject MousePositionMarkerPrefab => mousePositionMarkerPrefab;
        public Vector3 MouseMarkerOffset => mouseMarkerOffset;
        public float MouseMarkerMaxDistance => mouseMarkerMaxDistance;

        #endregion

    }
}
