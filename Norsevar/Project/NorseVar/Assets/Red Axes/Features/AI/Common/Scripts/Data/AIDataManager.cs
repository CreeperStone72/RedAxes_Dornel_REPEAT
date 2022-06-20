using Norsevar.MusicAndSFX;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.AI
{

    public class AIDataManager : MonoBehaviour
    {

        #region Serialized Fields

        [SerializeField] [InlineEditor(InlineEditorModes.FullEditor)]
        private Enemy enemy;

        [SerializeField] private EnemyEvent onDeath;

        #endregion

        #region Properties

        public AIStates AIStates => enemy.States;

        public AttackType AttackType => enemy.AttackType;

        public AIStats AIStats => enemy.Stats;

        public int Id { get; set; }

        public AIHealth AIHealth => enemy.Health;
        public EnemyEvent OnDeath => onDeath;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            NorseGame.Instance.RaiseEvent(
                IsWolf() ? ENorseGameEvent.Enemies_Wolf_Presence : ENorseGameEvent.Enemies_Snake_Presence,
                transform.position);
            FMODGlobalParameterChangeScript.AddEnemy();
        }

        #endregion

        #region Public Methods

        public bool IsWolf()
        {
            return enemy.Name == "Wolf";
        }

        #endregion

    }

}
