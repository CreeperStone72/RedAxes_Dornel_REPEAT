using UnityEngine;

namespace Norsevar.Combat
{

    [CreateAssetMenu(fileName = "PlayerDataCollection", menuName = "Norsevar/Player/Data/Data Collection")]
    public class PlayerDataCollection : ScriptableGameObject
    {

        #region Serialized Fields

        [SerializeField] private PlayerMovementData movementData;

        #endregion

        #region Properties

        public PlayerMovementData MovementData => movementData;

        #endregion

    }

}
