using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.Room_Generation
{
    [Serializable]
    public enum Persistence { StartingRoom, Segment, SemiPermanent, Permanent }

    [Serializable] [CreateAssetMenu]
    public class RoomType : ScriptableObject
    {

        #region Serialized Fields

        [SerializeField] [LabelText("Name")] private string roomName;
        [SerializeField] private Sprite sprite;
        [SerializeField] private int identifier;
        [SerializeField] private Persistence persistence;
        [SerializeField] [HideLabel] [HideIf("IsStartingRoom")]
        private WeightInformation weightInformation;

        #endregion

        #region Properties

        public string RoomName
        {
            get => roomName;
            set => roomName = value;
        }
        public Sprite RoomSprite => sprite;
        public int Identifier => identifier;
        public Persistence RoomPersistence
        {
            get => persistence;
            set => persistence = value;
        }
        public WeightInformation WeightInformation => weightInformation;

        #endregion

        #region Private Methods

        private bool IsStartingRoom()
        {
            return persistence == Persistence.StartingRoom;
        }

        #endregion

        #region Public Methods

        public float GetWeight(int depth)
        {
            return WeightInformation.GetWeight(depth);
        }

        public void ValidateData()
        {
            WeightInformation.ValidateData();

            switch (RoomPersistence)
            {
                case Persistence.StartingRoom:
                    WeightInformation.StartingRoom();
                    break;
                case Persistence.SemiPermanent:
                    WeightInformation.SemiPermanent();
                    break;
                case Persistence.Permanent:
                    WeightInformation.Permanent();
                    break;
                case Persistence.Segment: return;
                default:                  return;
            }
        }

        #endregion

    }
}
