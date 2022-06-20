using System.Linq;
using UnityEngine;

namespace Norsevar.Room_Generation
{
    public class LevelTree
    {

        #region Constants and Statics

        private static Vector2 _dimensions = Vector2.one;

        #endregion

        #region Private Fields

        private RoomNode _root;

        #endregion

        #region Constructors

        public LevelTree(RoomType roomType)
        {
            _root = new RoomNode(0, 0, roomType);
        }

        #endregion

        #region Private Methods

        private static void AttachChildren(RoomNode currentNode, float space, IRoomType generator)
        {
            var size = currentNode.Children.Length;

            for (var i = 0; i < size; i++)
            {
                RoomNode newNode = new(
                    currentNode.Depth + 1,
                    i,
                    RoomTypeManager.GetRandomRoomType(generator.RoomTypes, currentNode.Depth + 1));
                currentNode.AttachChild(newNode, _dimensions, space);
            }
        }

        #endregion

        #region Public Methods

        public static void BuildDimensions(Bounds bounds, Vector3 scale)
        {
            // size in pixels
            var width = scale.x * bounds.size.x;
            var height = scale.z * bounds.size.z;

            _dimensions = new Vector2(width, height);
        }

        public void DrawChildren(GameObject roomModel, Transform parent)
        {
            foreach (var child in _root.Children)
                child.Instantiate(roomModel, parent);
        }

        public void DrawRoot(GameObject roomModel, Transform parent)
        {
            _root.Instantiate(roomModel, parent);
        }

        public RoomType[] GetChildren()
        {
            RoomType[] roomTypes = _root.Children.Select(child => child.RoomType).ToArray();
            return roomTypes;
        }

        public int GetChildrenCount()
        {
            return _root.Children.Length;
        }

        public RoomType GetCurrent()
        {
            return _root.RoomType;
        }

        public Vector3 GetOrigin()
        {
            return _root.Origin;
        }

        public void Move(int child)
        {
            if (_root.IsOutOfBounds(child)) return;
            _root = _root.Children[child];
        }

        public void Populate(float space, IRoomType roomTypes)
        {
            AttachChildren(_root, space, roomTypes);
        }

        #endregion

    }
}
