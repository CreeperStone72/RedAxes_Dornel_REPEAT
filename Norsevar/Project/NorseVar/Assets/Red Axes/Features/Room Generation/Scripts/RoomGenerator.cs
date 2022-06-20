using System.Collections;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Norsevar.Room_Generation
{

    public class RoomGenerator : MonoBehaviour, IRoomType
    {

        #region Private Fields

        [Header("Private attributes")]
        private LevelTree _levelTree;
        private int _currentDepth;

        #endregion

        #region Serialized Fields

        [Header("Detail options")]
        public bool enableMinimap;
        public bool enableButtonPanel;

        [Header("Tree model parameters")]
        [SerializeField] private int depth;
        [SerializeField] private int scale;
        [SerializeField] private GameObject roomModel;

        [Header("UI parameters")]
        [SerializeField] private Camera minimapCamera;

        [SerializeField] private float timeToZoom = 0.7f;
        [SerializeField] private float zoomFOV = 60f;
        [SerializeField] private float defaultFOV = 90f;

        [SerializeField] private float timeToMove = 0.5f;

        [Header("Button panel parameters")]
        [SerializeField] private GameObject buttonModel;
        [SerializeField] private HorizontalLayoutGroup buttonPanel;

        [Header("Room types")]
        [SerializeField] private RoomType[] roomTypes;

        #endregion

        #region Properties

        // Tree model parameters
        public int Depth
        {
            get => depth;
            set => depth = value;
        }
        public int Scale
        {
            get => scale;
            set => scale = value;
        }
        public GameObject RoomModel
        {
            get => roomModel;
            set => roomModel = value;
        }

        // UI parameters
        public Camera MinimapCamera
        {
            get => minimapCamera;
            set => minimapCamera = value;
        }
        public float TimeToZoom
        {
            get => timeToZoom;
            set => timeToZoom = value;
        }
        public float ZoomFOV
        {
            get => zoomFOV;
            set => zoomFOV = value;
        }
        public float DefaultFOV
        {
            get => defaultFOV;
            set => defaultFOV = value;
        }
        public float TimeToMove
        {
            get => timeToMove;
            set => timeToMove = value;
        }

        // Button panel parameters
        public GameObject ButtonModel
        {
            get => buttonModel;
            set => buttonModel = value;
        }
        public HorizontalLayoutGroup ButtonPanel
        {
            get => buttonPanel;
            set => buttonPanel = value;
        }

        public RoomType[] RoomTypes => roomTypes;
        private bool EnablePanel => enableMinimap && enableButtonPanel;

        #endregion

        #region Unity Methods

        private void Start()
        {
            LevelTree.BuildDimensions(GetBounds(), roomModel.transform.localScale);

            _currentDepth = 1;
            _levelTree = new LevelTree(RoomTypeManager.GetRoomType(roomTypes, "Start"));
            _levelTree.Populate(scale, this);

            DrawRoot();
            DrawTree();

            if (EnablePanel) UpdateUI(_levelTree);
        }

        public void OnValidate()
        {
            roomTypes.ForEach(roomType => roomType.ValidateData());
        }

        #endregion

        #region Private Methods

        private void DrawRoot()
        {
            _levelTree.DrawRoot(roomModel, transform);
        }

        private void DrawTree()
        {
            _levelTree.DrawChildren(roomModel, transform);
        }

        private Bounds GetBounds()
        {
            return roomModel.GetComponent<MeshFilter>().sharedMesh.bounds;
        }

        private GameObject GetButton()
        {
            return Instantiate(buttonModel, GetPanelTransform());
        }

        private Transform GetPanelTransform()
        {
            return buttonPanel.transform;
        }

        private IEnumerator MoveCamera()
        {
            yield return StartCoroutine(Zoom(false));
            yield return StartCoroutine(SnapToRoot(_levelTree));
            yield return StartCoroutine(Zoom(true));
        }

        private void OnClickUpdate(int selectedChild)
        {
            if (_currentDepth >= depth)
                return;
            Populate(selectedChild);
            if (enableMinimap) StartCoroutine(MoveCamera());
            if (EnablePanel) UpdateUI(_levelTree);
        }

        private void Populate(int child)
        {
            _levelTree.Move(child);
            _levelTree.Populate(scale, this);
            DrawTree();
            _currentDepth++;
        }

        private IEnumerator SnapToRoot(LevelTree levelTree)
        {
            var timeElapsed = 0.0f;
            Vector3 origin = minimapCamera.transform.position, target = levelTree.GetOrigin();
            target.y = origin.y;

            while (timeElapsed < timeToMove)
            {
                minimapCamera.transform.position = Vector3.Lerp(origin, target, timeElapsed / timeToMove);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            minimapCamera.transform.position = target;
        }

        private void UpdateUI(LevelTree levelTree)
        {
            foreach (Transform oldChild in GetPanelTransform())
                Destroy(oldChild.gameObject);

            for (var child = 0; child < levelTree.GetChildrenCount(); child++)
            {
                var selectedChild = child;

                var buttonGo = GetButton();
                var childButton = buttonGo.GetComponent<Button>();

                //childButton.GetComponent<TMP_Text>().text = $"Child {child + 1};
                childButton.onClick.AddListener(() => OnClickUpdate(selectedChild));
            }
        }

        private IEnumerator Zoom(bool zoomingIn)
        {
            var timeElapsed = 0.0f;
            float currentFOV = minimapCamera.fieldOfView, targetFOV = zoomingIn ? zoomFOV : defaultFOV;

            while (timeElapsed < timeToZoom)
            {
                minimapCamera.fieldOfView = Mathf.Lerp(currentFOV, targetFOV, timeElapsed / timeToZoom);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            minimapCamera.fieldOfView = targetFOV;
        }

        #endregion

    }
}
