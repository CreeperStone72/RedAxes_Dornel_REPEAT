using System.Collections;
using Norsevar.Combat;
using Norsevar.Stat_System;
using Norsevar.VFX;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Norsevar.Interaction
{
    public class HealthPickupSpawner : MonoBehaviour
    {
        [SerializeField] [Required] private GameObject healthPrefab;

        private Resource _playerHealth;

        private void Start()
        {
            StartCoroutine(GetHealthResource());
        }

        private IEnumerator GetHealthResource()
        {
            PlayerController playerController = NorseGame.Instance.Get<PlayerController>();
            while (playerController is null)
            {
                yield return null;
                playerController = NorseGame.Instance.Get<PlayerController>();
            }

            _playerHealth = playerController[EStatType.Health] as Resource;
        }

        public void SpawnHealthPickup(EnemyData enemyData)
        {
            float healthPercent = _playerHealth.CurrentValue / _playerHealth.Value;
            float dropChance = 1.2f - healthPercent;
            
            if(Random.Range(0f, 1f) < dropChance)
                InstantiateEffects.InstantiatePoof(healthPrefab, enemyData.position + new Vector3(0, 0.5f, 0), null, Vector3.one * .5f, .5f);
        }
    }
}
