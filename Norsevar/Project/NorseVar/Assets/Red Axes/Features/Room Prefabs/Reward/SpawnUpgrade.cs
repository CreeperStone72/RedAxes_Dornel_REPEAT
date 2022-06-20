using DG.Tweening;
using Norsevar.Interaction;
using Norsevar.MusicAndSFX;
using Norsevar.Spawner;
using UnityEngine;
using UnityEngine.Events;

namespace Norsevar.Room_Prefabs
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

        #endregion

        #region Properties

        public static SpawnUpgrade Instance { get; private set; }

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _spawned = upgradeObject.activeSelf;
            Instance = this;
        }

        #endregion

        #region Public Methods

        public void Despawn()
        {
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Upgrade_UpgradeSpawn, upgradeObject.transform.position);
            FMODGlobalParameterChangeScript.ActivateRewardMusic(MusicEnums.NeitherSOrH);
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
            FMODGlobalParameterChangeScript.ActivateRewardMusic(MusicEnums.Upgrade);

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
