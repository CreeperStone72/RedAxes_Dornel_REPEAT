using BehaviorDesigner.Runtime.Tasks;

namespace Norsevar.AI
{
    [TaskCategory("Norsevar/Wolf")]
    public class AttackCharger : Action
    {

        #region Private Fields

        private AttackBehaviour _attackBehaviour;

        #endregion

        #region Private Methods

        private void AttackTarget()
        {
            _attackBehaviour.Attack();
        }

        #endregion

        #region Public Methods

        public override void OnAwake()
        {
            base.OnAwake();
            _attackBehaviour = GetComponent<AttackBehaviour>();
        }

        public override TaskStatus OnUpdate()
        {
            AttackTarget();
            return TaskStatus.Success;
        }

        #endregion

    }
}
