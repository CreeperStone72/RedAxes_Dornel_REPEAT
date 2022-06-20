using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.Room_Generation
{
    [Serializable]
    public enum WeightFunction
    {
        [InspectorName("Constant weight")]
        Constant,
        [InspectorName("Linear weight growth")]
        LinearGrowth
    }

    [Serializable]
    public class WeightInformation
    {

        #region Private Fields

        [Header("Constraints")]
        private int _roomsSinceLastAppearance;

        #endregion

        #region Serialized Fields

        [Space]
        [SerializeField] public WeightFunction function;
        [LabelText("Weight")] public int defaultWeight = 1;

        [Space]
        [HideIf("IsConstant")]
        public float growthFactor;

        [SerializeField] [HideIf("IsConstant")]
        public bool resetOnAppearance;

        [SerializeField] [VectorLabels("start", "end")]
        public Vector2Int range;

        #endregion

        #region Private Methods

        private void AssertRange()
        {
            // (0) Cannot be zero
            if (!(0 < GetA())) SetA(1);
            // (1) Direct Order
            if (!(GetA() <= GetB())) SetB(GetA());
        }

        private bool IsConstant()
        {
            return function == WeightFunction.Constant;
        }

        #endregion

        #region Public Methods

        public void AssertConstant()
        {
            // W cannot be negative
            if (!(0 <= defaultWeight)) defaultWeight = 0;
        }

        public void AssertLinear()
        {
            // g cannot be negative or equal to zero (otherwise it's a constant function)
            if (!(0 < growthFactor)) growthFactor = 0.01f;
            // W cannot be negative
            if (!(0 <= defaultWeight)) defaultWeight = 0;
        }

        public int GetA()
        {
            return range.x;
        }

        public int GetB()
        {
            return range.y;
        }

        public int GetWeight(int depth)
        {
            if (depth < GetA()) return 0;
            if (depth > GetB()) return 0;
            return function switch
            {
                // f(x) = W
                WeightFunction.Constant => defaultWeight,
                // f(x) = g * x + W || a(x) * x + W, with a(x) the number of rooms since this type last appeared
                WeightFunction.LinearGrowth =>
                    (int)(growthFactor * (resetOnAppearance ? _roomsSinceLastAppearance : depth) + defaultWeight),
                _ => 0
            };
        }

        public void HasAppeared()
        {
            _roomsSinceLastAppearance = 0;
        }

        public void HasNotAppeared()
        {
            _roomsSinceLastAppearance++;
        }

        public void Permanent()
        {
            SetA(0);
            SetB(int.MaxValue);
        }

        public void SemiPermanent()
        {
            SetB(int.MaxValue);
        }

        public void SetA(int a)
        {
            range.x = a;
        }

        public void SetB(int b)
        {
            range.y = b;
        }

        public void StartingRoom()
        {
            SetA(0);
            SetB(0);
        }

        public void ValidateData()
        {
            AssertRange();
            AssertConstant();
            AssertLinear();
        }

        #endregion

    }
}
