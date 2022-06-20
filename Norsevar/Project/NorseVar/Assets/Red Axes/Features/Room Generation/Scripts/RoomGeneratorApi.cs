using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.Room_Generation
{
    public class RoomGeneratorApi : Singleton<RoomGeneratorApi>, IRoomType
    {

        #region Constants and Statics

        private const int MAX_DEPTH = 15;

        #endregion

        #region Private Fields

        private LevelTree _levelTree;
        private int _currentDepth;

        #endregion

        #region Serialized Fields

        [SerializeField] [Header("Tree model parameters")] [Range(1, MAX_DEPTH)]
        private int depth;
        [SerializeField]
        private int scale;

        [SerializeField] [Header("Room types")]
        private RoomType[] roomTypes;

        #endregion

        #region Properties

        public RoomType[] RoomTypes => roomTypes;

        #endregion

        #region Private Methods

        private void Populate(int child)
        {
            _levelTree.Move(child);
            _levelTree.Populate(scale, this);
            _currentDepth++;
        }

        #endregion

        #region Protected Methods

        protected override void OnAwake()
        {
            LevelTree.BuildDimensions(new Bounds(Vector3.zero, Vector3.one), Vector3.one);

            _currentDepth = 1;
            _levelTree = new LevelTree(RoomTypeManager.GetRoomType(RoomTypes, "Start"));
            _levelTree.Populate(scale, this);
        }

        #endregion

        #region Public Methods

        public IEnumerable<RoomType> GetChildren()
        {
            return _levelTree.GetChildren();
        }

        [Button]
        public bool OnClickUpdate(int selectedChild)
        {
            if (_currentDepth >= depth) return false;
            Populate(selectedChild);
            Debug.Log(_levelTree.GetChildrenCount().ToString());
            foreach (RoomType roomType in _levelTree.GetChildren())
                Debug.Log(roomType.RoomName);
            return true;
        }

        #endregion

    }
}
