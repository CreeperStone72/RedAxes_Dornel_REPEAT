using UnityEngine;

namespace Norsevar
{

    [CreateAssetMenu(fileName = "New Game Scene", menuName = "Norsevar/Game/Scene")]
    public class GameScene : ScriptableObject
    {

        #region Serialized Fields

        [SerializeField] private string scene;

        #endregion

        #region Properties

        public string Name => scene;

        #endregion

    }

}
