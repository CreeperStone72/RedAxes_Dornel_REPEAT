using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Norsevar.AI
{

    [InfoBox("Please make sure to add Pack Behaviour to the Enemy Event Listener.")]
    public class PackBehaviour : MonoBehaviour
    {

        #region Private Fields

        private Dictionary<int, BtManager> _wolves;
        private Transform _target;

        private int _leaderId;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _leaderId = 0;
            _wolves = new Dictionary<int, BtManager>();
        }

        #endregion

        #region Private Methods

        private int GetWolvesCount()
        {
            return _wolves.Count;
        }

        #endregion

        #region Public Methods

        public void AddWolf(int pID, GameObject pInstance)
        {
            if (_wolves.ContainsKey(pID))
            {
                pID = 0;
                while (_wolves.ContainsKey(pID))
                    pID++;
            }
            _wolves.Add(pID, pInstance.GetComponent<BtManager>());
        }

        public Vector3 GetPositionAroundTarget(float id, float radiusFromTarget)
        {
            Vector3 targetPosition = Vector3.zero;
            Vector3 position = _target.transform.position;

            targetPosition.x = position.x + radiusFromTarget * math.sin(2 * math.PI * id / GetWolvesCount());
            targetPosition.y = position.y;
            targetPosition.z = position.z + radiusFromTarget * math.cos(2 * math.PI * id / GetWolvesCount());

            return targetPosition;
        }

        public void HandleWolfDeath(EnemyData pEnemyData)
        {
            int id = pEnemyData.id;
            if (!_wolves.ContainsKey(id)) return;
            _wolves.Remove(id);

            if (_leaderId == id) _leaderId = -1;
        }

        public bool IsAWolfHunting()
        {
            return _wolves.Any(pPair => pPair.Value.GetIsHunting());
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        #endregion

    }

}
