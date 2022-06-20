using DG.Tweening;
using Norsevar.Upgrade_System;
using UnityEngine;

namespace Norsevar.Interaction
{
    [RequireComponent(typeof(SphereCollider))]
    public class HealthPickup : MonoBehaviour, IConsumable
    {
        [SerializeField] private Transform healthObject;
        [SerializeField] private float value = 20f;

        private SphereCollider _collider;

        private void Awake()
        {
            _collider = GetComponent<SphereCollider>();
            _collider.enabled = false;
            this.ExecuteInSeconds(() => _collider.enabled = true, .5f);
        }

        private void Start()
        {
            healthObject.DOMoveY(healthObject.transform.position.y + .5f, 1f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }

        public void Consume(IConsumer consumer)
        {
            consumer?.ApplyHealthPickup(value);
            _collider.enabled = false;
            healthObject.DOScale(Vector3.zero, .5f).OnComplete(() => Destroy(gameObject));
        }
    }
}