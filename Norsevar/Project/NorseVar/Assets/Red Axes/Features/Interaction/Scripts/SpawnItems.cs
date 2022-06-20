using System.Collections;
using Norsevar.VFX;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.Interaction
{

    public class SpawnItems : MonoBehaviour
    {

        #region Serialized Fields

        [SerializeField] [Required] private ItemPool itemPool;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] [Required] private GameObject spawnPrefab;
        [SerializeField] protected SpawnCondition spawnCondition;

        [SerializeField] private bool isFree;

        #endregion

        #region Unity Methods

        protected void Start()
        {
            AttemptSpawnItems(SpawnCondition.OnStart);
        }

        #endregion

        #region Private Methods

        private void ApplyFreeStatus(ItemBehaviour pItemBehaviour)
        {
            if (isFree)
                pItemBehaviour.SetFree();
        }

        private IEnumerator CreateItems()
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                yield return new WaitForSeconds(0.5f);
                InstantiateEffects.InstantiatePoof(spawnPrefab, spawnPoint.position, spawnPoint, Vector3.one * .5f, .5f, OnSpawn);
            }
        }

        private void OnSpawn(GameObject obj)
        {
            ItemBehaviour itemBehaviour = obj.GetComponent<ItemBehaviour>();
            itemBehaviour.SetItem(itemPool.GetRandomItem());
            ApplyFreeStatus(itemBehaviour);
        }

        #endregion

        #region Protected Methods

        protected virtual void AttemptSpawnItems(SpawnCondition condition)
        {
            if(spawnCondition == condition)
                StartCoroutine(CreateItems());
        }

        #endregion

        #region Public Methods

        public void DestroyAll()
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint.childCount <= 0) continue;
                foreach (Transform child in spawnPoint) Destroy(child.gameObject);
            }
        }

        public void SpawnItem()
        {
            AttemptSpawnItems(SpawnCondition.OnEvent);
        }

        #endregion

    }

}
