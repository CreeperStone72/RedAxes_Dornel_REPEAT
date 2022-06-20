using UnityEngine;

namespace Norsevar.Spawner
{

    public class Spawner : ScriptableGameObject
    {

        #region Serialized Fields

        [SerializeField] [Range(5, 20)]
        private float width;

        [SerializeField] [Range(5, 20)]
        private float depth;

        [SerializeField] [Range(0, 10)]
        private int numberOfEnemies;

        [SerializeField]
        private GameObject enemy;

        #endregion

        #region Properties

        public float Width => width;

        public float Depth => depth;

        public int NumberOfEnemies => numberOfEnemies;

        public GameObject Enemy => enemy;

        #endregion

    }

}
