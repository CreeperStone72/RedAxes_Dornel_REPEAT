using System;
using System.Collections;
using UnityEngine;

namespace Norsevar.VFX
{

    public class InstantiateEffects : MonoBehaviour
    {

        #region Constants and Statics

        private static InstantiateEffects _instance;

        #endregion

        #region Serialized Fields

        [SerializeField] private GameObject poofVFX;

        #endregion

        #region Unity Methods
        
        private void Awake()
        {
            if (_instance != null) Destroy(gameObject);
            else
            {
                _instance = this;
                DontDestroyOnLoad(transform.root);
            }
        }
        
        #endregion

        #region Private Methods

        private static IEnumerator WaitForPoof( 
            GameObject         prefab,
            Vector3            position,
            Transform          parent,
            float              waitTime,
            Action<GameObject> onSpawn)
        {
            yield return new WaitForSeconds(waitTime);
            GameObject go = Instantiate(prefab, position, Quaternion.identity, parent);
            onSpawn?.Invoke(go);
        }

        #endregion

        #region Public Methods

        public static void InstantiatePoof(
            GameObject         prefab,
            Vector3            position,
            Transform          parent,
            Vector3            poofScale,
            float              waitTime,
            Action<GameObject> onSpawn = null)
        {
            GameObject poof = NorseGame.Instance.Get<ObjectPooler>().PoolVFX(_instance.poofVFX, waitTime + 2);
            poof.SetActive(true);
            poof.transform.localPosition = position;
            poof.transform.localScale = poofScale;
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.World_SpawnPoof, position);
            _instance.StartCoroutine(WaitForPoof( prefab, position, parent, waitTime, onSpawn));
        }

        #endregion

    }

}
