using System.Collections;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Norsevar.Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace Norsevar.Spawner
{

    public class SpawnShop : MonoBehaviour, ISpawn
    {

        #region Private Fields

        private bool _spawned;

        #endregion

        #region Serialized Fields

        [SerializeField] private GameObject shopObject;
        [SerializeField] private GameObject chestLid;
        [SerializeField] private GameObject merchant;
        [SerializeField] private Transform startPos;
        [SerializeField] private Transform endPos;
        [SerializeField] private ParticleSystem poofVFX;
        [SerializeField] private UnityEvent onSpawned;
        [SerializeField] private SpawnItems itemSpawner;
        [SerializeField] private bool spawnOnStart;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _spawned = shopObject.activeSelf;
        }

        private void Start()
        {
            if (spawnOnStart)
                Spawn();
        }

        #endregion

        #region Private Methods

        private Tween AnimateChest()
        {
            TweenerCore<Quaternion, Vector3, QuaternionOptions> t = chestLid.transform.DOLocalRotate(new Vector3(-135, 0, 0), 1.5f)
                .SetEase(Ease.OutBounce);
            t.onComplete += () =>
            {
                StartCoroutine(SpawnMerchant());
            };
            return t;
        }

        private IEnumerator SpawnMerchant()
        {
            poofVFX.Play();
            yield return new WaitForSeconds(0.1f);
            merchant.SetActive(true);
        }

        #endregion

        #region Public Methods

        public void Despawn()
        {
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Merchant_MerchantDespawn, shopObject.transform.position);
            chestLid.transform.localRotation = Quaternion.identity;
            shopObject.SetActive(false);
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
            shopObject.SetActive(true);
            merchant.SetActive(false);

            //Set positions
            shopObject.transform.position = startPos.position;
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Merchant_MerchantSpawn, shopObject.transform.position);

            //Setup tweens
            Sequence sequence = DOTween.Sequence();
            Tween t1 = shopObject.transform.DOMove(endPos.position, 0.5f).SetEase(Ease.OutBack);
            Tween t2 = AnimateChest();
            sequence.Append(t1);
            sequence.Append(t2);
            sequence.onComplete += () => onSpawned.Invoke();
        }

        #endregion

    }

}
