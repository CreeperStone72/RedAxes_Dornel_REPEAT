using DG.Tweening;
using Norsevar.Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace Norsevar.Spawner
{

    public class SpawnUpgrade : MonoBehaviour, ISpawn
    {

        #region Private Fields

        private bool _spawned;

        #endregion

        #region Serialized Fields

        [SerializeField] private SpawnItems itemSpawner;
        [SerializeField] private GameObject upgradeObject;
        [SerializeField] private GameObject mainStatue;
        [SerializeField] private Transform startPos;
        [SerializeField] private Transform endPos;
        [SerializeField] private UnityEvent onSpawned;
        [SerializeField] private bool spawnOnStart;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _spawned = upgradeObject.activeSelf;
        }

        private void Start()
        {
            if (spawnOnStart)
                Spawn();
        }

        #endregion

        #region Public Methods

        public void Despawn()
        {
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Upgrade_UpgradeSpawn, upgradeObject.transform.position);
            mainStatue.transform.DOScale(Vector3.one * 3, 2f).SetEase(Ease.OutBack);
            upgradeObject.SetActive(false);
            _spawned = false;
            itemSpawner.DestroyAll();
        }

        public void Spawn()
        {
            if (_spawned)
            {
                Despawn();
                return;
            }

            _spawned = true;

            //Set Active
            upgradeObject.SetActive(true);

            //Set positions
            upgradeObject.transform.position = startPos.position;
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Upgrade_UpgradeSpawn, upgradeObject.transform.position);

            //Setup tweens
            Sequence sequence = DOTween.Sequence();
            Tween t1 = upgradeObject.transform.DOMove(endPos.position, 2f).SetEase(Ease.InBack);
            mainStatue.transform.DOScale(Vector3.one * 5, 2f).SetEase(Ease.InElastic);
            sequence.Append(t1);
            sequence.onComplete += () => onSpawned.Invoke();
        }

        #endregion

    }

}
