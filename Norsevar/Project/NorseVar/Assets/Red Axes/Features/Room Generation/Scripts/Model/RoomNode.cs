using UnityEngine;
using Random = System.Random;

namespace Norsevar.Room_Generation
{
    public class RoomNode
    {

        #region Constants and Statics

        private const int MAXCHILDREN = 3;

        #endregion

        #region Private Fields

        private int _childrenCount;

        private int _siblingIndex;

        #endregion

        #region Constructors

        public RoomNode(int depth, int siblingIndex, RoomType roomType) : this(depth, Vector3.zero, siblingIndex, roomType)
        {
        }

        private RoomNode(int depth, Vector3 origin, int siblingIndex, RoomType roomType)
        {
            _childrenCount = 0;
            Depth = depth;
            Children = new RoomNode[RandInt()];
            RoomType = roomType;
            Origin = origin;
            _siblingIndex = siblingIndex;
        }

        #endregion

        #region Properties

        public RoomType RoomType { get; }

        #endregion

        #region Private Methods

        private Vector3 GetOffsetOrigin(RoomNode parent, Vector2 dimensions, float space)
        {
            var width = dimensions.x;
            var height = dimensions.y;

            var xChild = parent.Origin.x + width + space;

            var coordsChild = new Vector3(xChild, 0, parent.Origin.z);

            switch (parent.Children.Length)
            {
                case 1:
                    break;
                case 2:
                    var sign = _siblingIndex == 0 ? -1 : +1;
                    coordsChild.z += sign * (height + space) / 2;
                    break;
                case 3:
                    coordsChild.z += (_siblingIndex - 1) * (height + space);
                    break;
                default:
                    return parent.Origin;
            }

            return coordsChild;
        }

        private static int RandInt()
        {
            var random = new Random();
            return random.Next(MAXCHILDREN) + 1;
        }

        #endregion

        #region Public Methods

        public void AttachChild(RoomNode child, Vector2 dimensions, float space)
        {
            if (IsOutOfBounds(_childrenCount)) return;

            child._siblingIndex = _childrenCount;
            child.Origin = child.GetOffsetOrigin(this, dimensions, space);

            Children[_childrenCount] = child;
            _childrenCount++;
        }

        public void Instantiate(GameObject model, Transform parent)
        {
            var room = Object.Instantiate(model, Origin, Quaternion.identity, parent);
            room.name = $"({Depth};{_siblingIndex})";
        }

        public bool IsOutOfBounds(int index)
        {
            return index >= Children.Length;
        }

        public override string ToString()
        {
            return $"({Depth}, {_siblingIndex}) - {Origin}";
        }

        #endregion

        public readonly int Depth;

        public readonly RoomNode[] Children;

        public Vector3 Origin;
    }
}
