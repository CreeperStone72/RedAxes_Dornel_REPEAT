using Norsevar.AI;
using UnityEngine;

namespace Norsevar.Combat
{
    [RequireComponent(typeof(Collider), typeof(Rigidbody))]
    public class ChargeAttackProjectile : MonoBehaviour
    {

        #region Private Fields

        private Transform _attacker;
        private Vector3 _launchDirection;
        private AttackData _attackData;
        private DamageInfo _damageInfo;
        private Rigidbody _rb;

        #endregion

        #region Serialized Fields

        [SerializeField] private float speed = 15f;
        [SerializeField] private GameObject hitVFX;
        [SerializeField] private GameObject flashVFX;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (_attacker)
                _rb.velocity = transform.forward * speed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            Impact(collision.collider);
        }

        private void OnTriggerEnter(Collider other)
        {
            Impact(other);
        }

        #endregion

        #region Private Methods

        private void Impact(Collider other)
        {

            if (other.gameObject.CompareTag("Player"))
                return;

            if (hitVFX != null)
            {
                var hitInstance = Instantiate(hitVFX, transform.position, Quaternion.identity);
                hitInstance.transform.rotation = gameObject.transform.rotation * Quaternion.Euler(0, 180f, 0);
            }

            Collider[] hits = Physics.OverlapSphere(transform.position, _attackData.FovRadius, _attackData.HittableLayers);

            foreach (var hit in hits)
            {
                if (!hit.TryGetComponent<IDamageable>(out var entity))
                    continue;

                DamageInfo damageInfo = new(_damageInfo);

                if (hit.gameObject != other.gameObject)
                {
                    float distance = Vector3.Distance(hit.transform.position, transform.position);
                    float damageMultiplier = distance / _attackData.FovRadius;
                    damageInfo.DamageValue *= damageMultiplier;
                }

                float value = entity.ReceiveDamage(damageInfo);
                IKickbackAble a = hit.GetComponent<IKickbackAble>();
                a?.Kickback(Vector3.zero, value);
            }

            Destroy(gameObject);
        }

        #endregion

        #region Public Methods

        public void Init(AttackData attackData, DamageInfo damageToDeal, Transform attacker, Vector3 launchDir)
        {
            _attacker = attacker;
            _launchDirection = launchDir;
            _attackData = attackData;
            _damageInfo = damageToDeal;

            transform.forward = launchDir;

            var flashInstance = Instantiate(flashVFX, transform.position, Quaternion.identity);
            flashInstance.transform.forward = gameObject.transform.forward;
        }

        #endregion

    }
}
