using Norsevar.Combat;
using UnityEngine;
using UnityEngine.Serialization;

namespace Norsevar.AI
{

    public class ProjectileLaunch : MonoBehaviour
    {

        #region Private Fields

        private Rigidbody _rigid;
        private Quaternion _initialRotation;
        private Transform _target;
        private float _launchAngle;

        #endregion

        #region Serialized Fields

        [FormerlySerializedAs("_poolGO")] [SerializeField]
        private GameObject poolGo;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody>();
            _initialRotation = transform.rotation;
        }

        private void Update()
        {
            transform.rotation = Quaternion.LookRotation(_rigid.velocity) * _initialRotation;
        }


        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                return;

            if (other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                Instantiate(poolGo, transform.position, Quaternion.identity);

            if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                other.gameObject.GetComponent<IDamageable>().ReceiveDamage(
                    new DamageInfo { SourceGameObject = gameObject, DamageType = EDamageType.Poison, DamageValue = 10 });
            }

            Destroy(gameObject);
        }

        #endregion

        #region Private Methods

        private void Launch()
        {
            Vector3 position = transform.position;
            Vector3 projectileXZPos = new(position.x, 0.0f, position.z);

            Vector3 targetPosition = _target.position;
            Vector3 targetXZPos = new(targetPosition.x, 0.0f, targetPosition.z);

            transform.LookAt(targetXZPos);

            float r = Vector3.Distance(projectileXZPos, targetXZPos);
            float g = Physics.gravity.y;
            float tanAlpha = Mathf.Tan(_launchAngle * Mathf.Deg2Rad);
            const float targetHeight = 2;
            float h = targetPosition.y + targetHeight - position.y;

            float vz = Mathf.Sqrt(g * r * r / (2.0f * (h - r * tanAlpha)));
            float vy = tanAlpha * vz;

            Vector3 localVelocity = new(0f, vy, vz);
            Vector3 globalVelocity = transform.TransformDirection(localVelocity);

            if (float.IsNaN(globalVelocity.x))
                return;

            _rigid.velocity = globalVelocity;
        }

        #endregion

        #region Public Methods

        public void SetTargetPos(Transform target, float angle)
        {
            _launchAngle = angle;
            _target = target;
            Launch();
        }

        #endregion

    }

}
