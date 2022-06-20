using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Norsevar
{
    public class ObjectPooler : MonoBehaviour
    {

        #region Private Fields

        private Dictionary<GameObject, Tuple<GameObject, List<GameObject>, List<bool>>> _poolsDict;

        #endregion

        #region Serialized Fields

        public Transform safePos;

        public List<PoolInfo> PoolInfos = new();

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _poolsDict = new Dictionary<GameObject, Tuple<GameObject, List<GameObject>, List<bool>>>();
            foreach (PoolInfo poolInfo in PoolInfos)
                PoolPrefab(poolInfo.prefab, poolInfo.instances);

            transform.position = safePos.position;
        }

        private void Start()
        {
            foreach (GameObject o in _poolsDict.SelectMany(kvp => kvp.Value.Item2))
                o.SetActive(false);
        }

        private void OnEnable()
        {
            NorseGame.Instance.Register(this);
        }

        private void OnDisable()
        {
            NorseGame.Instance.Unregister<ObjectPooler>();
        }

        private void OnDestroy()
        {
            foreach (KeyValuePair<GameObject, Tuple<GameObject, List<GameObject>, List<bool>>> kvp in _poolsDict)
                Destroy(kvp.Value.Item1);

            _poolsDict.Clear();
        }

        #endregion

        #region Private Methods

        private void DePool(GameObject prefab, int index)
        {
            _poolsDict[prefab].Item2[index].SetActive(false);
            _poolsDict[prefab].Item2[index].transform.parent = _poolsDict[prefab].Item1.transform;
            _poolsDict[prefab].Item2[index].transform.localScale = Vector3.one;
            _poolsDict[prefab].Item2[index].transform.localPosition = Vector3.zero;
            _poolsDict[prefab].Item2[index].transform.rotation = Quaternion.identity;
            _poolsDict[prefab].Item3[index] = false;
        }

        private void Pool(GameObject prefab, int index)
        {
            _poolsDict[prefab].Item3[index] = true;
        }

        private void PoolPrefab(GameObject prefab, int instances)
        {
            if (!_poolsDict.ContainsKey(prefab))
            {
                GameObject parent = new(prefab.name + " Pool")
                {
                    transform =
                    {
                        parent = transform
                    }
                };
                List<GameObject> list = new();
                List<bool> boolList = new();
                _poolsDict.Add(prefab, new Tuple<GameObject, List<GameObject>, List<bool>>(parent, list, boolList));
                for (int i = 0; i < instances; i++)
                {
                    list.Add(Instantiate(prefab, parent.transform));
                    boolList.Add(false);
                }
            }
            else
            {
                for (int i = 0; i < instances; i++)
                {
                    GameObject parent = _poolsDict[prefab].Item1;
                    _poolsDict[prefab].Item2.Add(Instantiate(prefab, parent.transform));
                    _poolsDict[prefab].Item3.Add(false);
                }
            }
        }

        private IEnumerator UnPoolInSeconds(float seconds, GameObject prefab, int index)
        {
            yield return new WaitForSeconds(seconds);
            DePool(prefab, index);
        }

        #endregion

        #region Public Methods

        public void DeletePrefab(GameObject prefab, int amt = 0)
        {
            if (amt == 0)
                _poolsDict.Remove(prefab);
            else
            {
                for (int i = 0; i < amt; i++)
                    _poolsDict[prefab].Item2.RemoveRange(_poolsDict[prefab].Item2.Count - amt, amt);
            }
        }

        public GameObject PoolVFX(GameObject prefab, float time)
        {
            (_, List<GameObject> item2, List<bool> item3) = _poolsDict[prefab];
            int i = 0;
            while (item3[i]) i++;
            Pool(prefab, i);
            StartCoroutine(UnPoolInSeconds(time, prefab, i));
            return item2[i];
        }

        #endregion

        [Serializable]
        public struct PoolInfo
        {

            #region Serialized Fields

            public GameObject prefab;
            public int instances;

            #endregion

        }
    }
}
