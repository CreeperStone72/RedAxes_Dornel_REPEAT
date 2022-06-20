using UnityEngine;

namespace Norsevar.AI
{

    [CreateAssetMenu(fileName = "New Snake States", menuName = "Norsevar/AI/Snake/State")]
    public class SnakeStates : AIStates
    {

        #region Serialized Fields

        [SerializeField] private float fleeDistance;
        [SerializeField] private float stopFleeDistance;
        [SerializeField] private float slitherSpeed;
        [SerializeField] private float timeBetweenAttacks;
        [SerializeField] private float searchDistance;

        #endregion

        #region Properties

        public float FleeDistance => fleeDistance;

        public float StopFleeDistance => stopFleeDistance;

        public float SlitherSpeed => slitherSpeed;

        public float TimeBetweenAttacks => timeBetweenAttacks;
        public float SearchDistance => searchDistance;

        #endregion

    }

}
