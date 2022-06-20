using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Norsevar.Spawner
{

    public class EnemyManager : MonoBehaviour
    {

        #region Private Fields

        private bool _isLevelCleared = true;

        #endregion


        #region Serialized Fields

        [SerializeField] private SpawnBehaviour[] spawners;
        [SerializeField] private TimeTickSystem.TickRateMultiplierType tickRateMultiplierType = TimeTickSystem.TickRateMultiplierType.Eight;
        [SerializeField] private UnityEvent levelCleared;
        public bool IsLevelCleared
        {
            set => _isLevelCleared = value;
        }

        #endregion

        #region Unity Methods

        private void OnEnable()
        {
            TimeTickSystem.Instance.RegisterListener(tickRateMultiplierType, HandleTick);
        }

        private void OnDisable()
        {
            TimeTickSystem.Instance.UnregisterListener(tickRateMultiplierType, HandleTick);
        }

        #endregion

        #region Private Methods

        private void HandleTick()
        {
            if (_isLevelCleared)
                return;
            if (spawners.Any(spawner => spawner.IsStillSpawning()))
                return;
            if (spawners.Any(spawner => spawner.IsAnyEnemyAlive()))
                return;

            IsLevelCleared = true;
            levelCleared?.Invoke();
        }

        #endregion
    }

}
