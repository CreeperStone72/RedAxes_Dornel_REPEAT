using UnityEngine;

namespace DataTypes
{
    public class Square
    {
        private readonly int _a, _b, _c, _d;
        private readonly Vector3 _vA, _vB, _vC, _vD;

        public Square(int a, int b, int c, int d, Vector3[] vertices)
        {
            _a = a;
            _b = b;
            _c = c;
            _d = d;
            _vA = vertices[_a];
            _vB = vertices[_b];
            _vC = vertices[_c];
            _vD = vertices[_d];
        }

        public int[] GetTriangleIndices() { return new []{ _a, _c, _b, _b, _c, _d }; }

        public override string ToString() { return $"({_a}, {_vA}) - ({_b}, {_vB}) - ({_c}, {_vC}) - ({_d}, {_vD})"; }
    }
}