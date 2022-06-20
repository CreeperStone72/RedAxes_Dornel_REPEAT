using Norsevar.Combat;
using UnityEngine;

namespace Norsevar.AI
{
    public class ChargeAttack : MonoBehaviour
    {

        #region Private Fields

        private BtManager _btManager;
        private AnimationBehaviour _componentInChildren;
        private DamageInfo _damageInfo;

        private bool _canFire;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _btManager = GetComponent<BtManager>();
            _componentInChildren = GetComponentInChildren<AnimationBehaviour>();
            _damageInfo = new DamageInfo { SourceGameObject = gameObject, DamageType = EDamageType.Physical, DamageValue = 20 };
        }

        private void OnTriggerEnter(Collider other)
        {
            /*if (!_btManager.GetIsAttacking() || !_btManager.GetCanAttack())
                return;

            if (!other.CompareTag("Player"))
                return;

            _componentInChildren.PlayDamaged();
            _btManager.Kickback(Vector3.zero, null);
            other.GetComponent<IDamageable>().ReceiveDamage(_damageInfo);

            _btManager.SetCanAttack(false);*/
        }

        #endregion

    }
}
