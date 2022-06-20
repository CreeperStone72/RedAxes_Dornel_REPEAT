using System.Collections.Generic;
using System.Linq;

namespace Norsevar.Room_Generation
{

    public static class WeightManager
    {

        #region Private Methods

        private static bool IndexInRange(IReadOnlyCollection<RoomType> types, int i)
        {
            return 0 <= i && i < types.Count;
        }

        private static float NormalisedWeight(RoomType[] types, int i, int depth)
        {
            return types[i].GetWeight(depth) / TotalWeight(types, depth);
        }

        private static float RoomTypeWeight(RoomType[] types, int i, int depth)
        {
            return IndexInRange(types, i) ? NormalisedWeight(types, i, depth) : .0f;
        }

        private static float TotalWeight(IEnumerable<RoomType> types, int depth)
        {

            float totalWeight = types.Sum(type => type.GetWeight(depth));
            return totalWeight;
        }

        #endregion

        #region Public Methods

        public static float[] StaggeredWeights(RoomType[] roomTypes, int depth)
        {
            var staggeredWeights = new float[roomTypes.Length];

            for (var (index, totalWeight) = (0, .0f); index < staggeredWeights.Length; index++)
            {
                totalWeight += RoomTypeWeight(roomTypes, index, depth);
                staggeredWeights[index] = totalWeight;
            }

            return staggeredWeights;
        }

        public static void UpdatePresence(IList<RoomType> roomTypes, int index)
        {
            for (var i = 0; i < roomTypes.Count; i++)
            {
                if (i == index) roomTypes[i].WeightInformation.HasAppeared();
                else roomTypes[i].WeightInformation.HasNotAppeared();
            }
        }

        #endregion

    }
}
