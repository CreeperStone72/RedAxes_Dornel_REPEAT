using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.AI
{

    public class RangeAttackBehaviour : AttackBehaviour
    {

        #region Private Fields

        private RangedCombatAttackType _attackType;
        [ShowInInspector] private Transform _target;

        #endregion

        #region Serialized Fields

        [SerializeField] private Transform origin;

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            _attackType = attackType as RangedCombatAttackType;
        }

        private void Start()
        {
            _target = GetComponent<SnakeStateBehaviour>().GetTarget();
        }

        #endregion

        #region Public Methods

        public override void Attack()
        {
            Vector3 forward = origin.forward;
            GameObject instantiate = Instantiate(_attackType.Projectile, origin.position, Quaternion.Euler(forward), transform.parent);
            instantiate.GetComponent<ProjectileLaunch>().SetTargetPos(_target, _attackType.ThrowAngle);
            instantiate.name = $"(Instantiated) - {_attackType.Projectile.name}";
        }

        #endregion

    }

}
