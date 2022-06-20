using System.Collections;
using JetBrains.Annotations;
using Norsevar.AI;
using Norsevar.Combat;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.Spawner
{

    public class SpawnBehaviour : MonoBehaviour, ISpawn
    {

        #region Private Fields

        private Transform _target;
        private PackBehaviour _packBehaviour;
        private bool _isRunning;

        #endregion

        #region Serialized Fields

        [InfoBox("Game object needs a Pack Behaviour Script", InfoMessageType.Error, "HasPackBehaviour")] [Required] [SerializeField]
        private Spawner spawner;

        [SerializeField] private int numberOfNewEnemies;
        [SerializeField] private float timeBetweenSpawns;
        [SerializeField] private int maxAllowed;
        [SerializeField] private bool spawnOnStart;

        #endregion

        #region Unity Methods

        private void Start()
        {
            if (spawnOnStart)
                Spawn();
        }

        private void OnDrawGizmosSelected()
        {
            if (spawner is null) return;
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + new Vector3(0, 3, 0), new Vector3(spawner.Width, 6, spawner.Depth));
        }

        #endregion

        #region Private Methods

        private void AddToWolfPack(int i, GameObject instantiate)
        {
            if (_packBehaviour == null)
                return;

            instantiate.GetComponent<IPack>().SetPack(_packBehaviour);
            instantiate.GetComponent<IPack>().SetID(i);
            _packBehaviour.AddWolf(i, instantiate);
            _packBehaviour.SetTarget(_target);
        }

        [UsedImplicitly]
        private bool HasPackBehaviour()
        {
            return spawner != null &&
                   (spawner.Enemy != null ? spawner.Enemy.name : null) == "Wolf" &&
                   GetComponent<PackBehaviour>() is null;
        }

        private void SpawnEnemies()
        {
            for (int i = 0; i < spawner.NumberOfEnemies; i++)
            {
                GameObject instantiate = SpawnEnemy(i);
                AddToWolfPack(i, instantiate);
            }
        }

        private GameObject SpawnEnemy(int i)
        {
            GameObject instantiate = Instantiate(
                spawner.Enemy,
                transform.position.AddRandomPositionVector(spawner.Width, spawner.Depth),
                QuaternionExtension.GetRandomRotation(),
                transform);
            instantiate.name = $"(Instantiated) - {spawner.Enemy.name} ({i})";

            instantiate.GetComponent<AIDataManager>().Id = i;
            instantiate.GetComponent<IHunter>().SetTarget(_target);

            return instantiate;
        }

        private IEnumerator SpawnPeriodically()
        {
            WaitForSeconds waitForSeconds = new(timeBetweenSpawns);
            _isRunning = true;
            for (int i = 0; i < numberOfNewEnemies; i++)
            {
                yield return waitForSeconds;
                int childCount = transform.childCount;
                while (childCount >= maxAllowed)
                {
                    yield return waitForSeconds;
                    childCount = transform.childCount;
                }

                GameObject spawnEnemy = SpawnEnemy(childCount);
                AddToWolfPack(childCount, spawnEnemy);
            }

            _isRunning = false;
        }

        #endregion

        #region Public Methods

        public void Despawn()
        {
            //todo
        }

        public bool IsAnyEnemyAlive()
        {
            return transform.childCount != 0;
        }

        public bool IsStillSpawning()
        {
            return _isRunning;
        }

        [Button]
        public void Spawn()
        {
            _target = NorseGame.Instance.Get<PlayerController>().transform;

            _packBehaviour = GetComponent<PackBehaviour>();

            if (maxAllowed == 0)
            {
                _isRunning = false;
                return;
            }

            SpawnEnemies();
            StartCoroutine(SpawnPeriodically());
        }

        #endregion

    }

}
