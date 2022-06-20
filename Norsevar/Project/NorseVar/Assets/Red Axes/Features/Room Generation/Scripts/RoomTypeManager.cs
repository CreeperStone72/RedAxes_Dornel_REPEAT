namespace Norsevar.Room_Generation
{

    public static class RoomTypeManager
    {

        #region Private Methods

        private static float Random()
        {
            return UnityEngine.Random.value;
        }

        #endregion

        #region Public Methods

        public static RoomType GetRandomRoomType(RoomType[] roomTypes, int depth)
        {
            float[] weights = WeightManager.StaggeredWeights(roomTypes, depth);
            float threshold = Random();
            int index = 0;

            // Debug.Log($"Depth : {depth}, Threshold : {threshold}, Weights : [{string.Join(", ", weights)}]");

            while (weights[index] < threshold && index < weights.Length)
                index++;

            WeightManager.UpdatePresence(roomTypes, index);

            return roomTypes[index];
        }

        public static RoomType GetRoomType(RoomType[] roomTypes, string typeName)
        {
            foreach (RoomType roomType in roomTypes)
            {
                if (roomType.name.Equals(typeName))
                    return roomType;
            }
            return roomTypes[0];
        }

        #endregion

    }
}
