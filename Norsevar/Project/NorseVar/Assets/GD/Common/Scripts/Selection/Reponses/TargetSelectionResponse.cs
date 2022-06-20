using UnityEngine;

namespace GD.Selection
{

    public class TargetSelectionResponse : MonoBehaviour, ISelectionResponse
    {
        #region Private Fields

        private readonly float scaleFactor = 5;
        private readonly int rayCastDepth = 10;

        private GameObject currentTargetInstance;

        #endregion

        #region Serialized Fields

        [SerializeField]
        [Tooltip( "Set prefab object used for target highlighting" )]
        private GameObject targetSelectionPrefab;

        [SerializeField]
        private LayerMask targetGroundLayerMask;

        [SerializeField]
        [Tooltip( "Vertical offset on target highlight above ground layer" )]
        private float targetOffset;

        #endregion

        #region Unity Methods

        public void Awake()
        {
            currentTargetInstance = Instantiate( targetSelectionPrefab, Vector3.zero, Quaternion.Euler( 0, 0, 0 ) );
            currentTargetInstance.SetActive( false );

            //demo in class - remove
            //Vector3 source = new Vector3(1, 1, 1);
            //Vector3 target = new Vector3(5, 5, 5);
            //Vector3 normalizedDirection = source.GetDirection(target);

            //Vector3 x = new Vector3(1, 5, 10);
            //x.ToGround(0);
        }

        #endregion

        #region Public Methods

        public void OnDeselect( Transform selection ) { currentTargetInstance.SetActive( false ); }

        public void OnSelect( Transform selection )
        {
            if ( currentTargetInstance != null )
            {
                RaycastHit hitInfo;
                if ( Physics.Raycast( selection.position, -selection.up, out hitInfo, rayCastDepth,
                    targetGroundLayerMask ) )
                {
                    currentTargetInstance.transform.position =
                        selection.position - new Vector3( 0, hitInfo.distance - targetOffset, 0 );
                    float mag = selection.GetComponent<Collider>().bounds.size.magnitude / scaleFactor;
                    currentTargetInstance.transform.localScale = new Vector3( mag, mag, mag );
                    currentTargetInstance.SetActive( true );
                }
            }
        }

        #endregion
    }

}