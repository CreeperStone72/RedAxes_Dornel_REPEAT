using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Norsevar.AI
{
    [TaskCategory("Norsevar/Wolf")]
    public class Attack : NavMeshAgentBt
    {

        #region Constants and Statics

        private const float PRE_ATTACK_TIME = 2.1f;

        #endregion

        #region Private Fields

        private AttackBehaviour _attackBehaviour;
        private WolfFeedback _wolfFeedback;
        private float _attackInterval;
        private bool _preAttackStarted;

        #endregion

        #region Private Methods

        private TaskStatus AttackTarget()
        {
            if (HandlePreAttackTime())
                PreAttack();

            if (HandleAttackTime())
                return TaskStatus.Running;

            _wolfFeedback.Bite(2);
            _attackInterval = Random.Range(attackData.MinWait, attackData.MaxWait);
            animationBehaviour.Value.PlayAttack(() => _attackBehaviour.Attack());
            _preAttackStarted = false;

            return TaskStatus.Success;
        }

        private bool HandleAttackTime()
        {
            if (!(_attackInterval > 0))
                return false;

            _attackInterval -= Time.deltaTime;
            return true;
        }

        private bool HandlePreAttackTime()
        {
            return _attackInterval <= PRE_ATTACK_TIME;
        }

        private void PreAttack()
        {
            if (_preAttackStarted)
                return;
            _preAttackStarted = true;
            _wolfFeedback.PrepareBite();
        }

        #endregion

        #region Protected Methods

        protected override float GetSpeedModifier()
        {
            return attackData.SpeedMultiplier;
        }

        #endregion

        #region Public Methods

        public override void OnAwake()
        {
            base.OnAwake();
            _attackBehaviour = GetComponent<AttackBehaviour>();
            _wolfFeedback = GetComponent<WolfFeedback>();
        }

        public override void OnStart()
        {
            base.OnStart();
            _attackInterval = Random.Range(attackData.MinWait, attackData.MaxWait);
        }

        public override TaskStatus OnUpdate()
        {
            return AttackTarget();
        }

        #endregion

        public AttackData attackData;
    }

}
