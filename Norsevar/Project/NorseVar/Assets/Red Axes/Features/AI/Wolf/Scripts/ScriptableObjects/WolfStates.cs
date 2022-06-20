using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.AI
{

    [CreateAssetMenu(fileName = "New Wolf States", menuName = "Norsevar/AI/Wolf/States", order = 1)]
    public class WolfStates : AIStates
    {

        #region Serialized Fields

        [SerializeField] [TabGroup("Idle")] private IdleData idleData;

        [SerializeField] [TabGroup("Hunt")] private BaseData huntData;

        [SerializeField] [TabGroup("Surround")]
        private ApproachData surroundData;

        [SerializeField] [TabGroup("Attack")]
        private AttackData attackData;

        #endregion

        #region Properties

        public AttackData AttackData => attackData;

        public ApproachData SurroundData => surroundData;

        public BaseData HuntData => huntData;

        public IdleData IdleData => idleData;

        #endregion

    }

}
