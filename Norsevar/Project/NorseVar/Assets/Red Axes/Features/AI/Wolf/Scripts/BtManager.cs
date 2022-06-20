using BehaviorDesigner.Runtime;
using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Events;

namespace Norsevar.AI
{
    public abstract class BtManager : MonoBehaviour, IKickbackAble, IHunter, IPack
    {
        #region Private Fields

        protected BehaviorTree behaviorTree;
        private SharedBool _isHit;
        private SharedBool _isHunting;
        private SharedBool _isAttacking;
        private SharedBool _canAttack;
        private SharedInt _id;
        private SharedTransform _target;
        private SharedVector3 _direction;
        private SharedPackBehaviour _packBehaviour;

        #endregion

        #region Serialized Fields

        public MMFeedbacks hitFeedback;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            behaviorTree = GetComponent<BehaviorTree>();
            _isHit = (SharedBool) behaviorTree.GetVariable("IsHit");
            _isHunting = (SharedBool) behaviorTree.GetVariable("IsHunting");
            _isAttacking = (SharedBool) behaviorTree.GetVariable("IsAttacking");
            _canAttack = (SharedBool) behaviorTree.GetVariable("CanAttack");
            _id = (SharedInt) behaviorTree.GetVariable("Id");
            _target = (SharedTransform) behaviorTree.GetVariable("Target");
            _direction = (SharedVector3) behaviorTree.GetVariable("KnockbackForce");
            _packBehaviour = (SharedPackBehaviour) behaviorTree.GetVariable("Pack");


            OnAwake();
        }

        #endregion

        #region Protected Methods

        protected abstract void OnAwake();

        protected void SetVariable(string blackboardProperty, UnityEvent feedback)
        {
            behaviorTree.GetVariable(blackboardProperty).SetValue(feedback);
        }

        protected void SetVariable(string blackboardProperty, float value)
        {
            behaviorTree.GetVariable(blackboardProperty).SetValue(value);
        }

        #endregion

        #region Public Methods

        public bool GetCanAttack()
        {
            return _canAttack.Value;
        }

        public bool GetIsAttacking()
        {
            return _isAttacking.Value;
        }

        public bool GetIsHunting()
        {
            return _isHunting.Value;
        }

        public virtual void Kickback(Vector3 dirToTarget, float? damageInfo)
        {
            if (!(damageInfo > 0))
                return;

            _isHit.Value = true;
            hitFeedback?.PlayFeedbacks(transform.position, (int)damageInfo);
            _direction.Value = dirToTarget;
        }

        public void SetCanAttack(bool value)
        {
            _canAttack.Value = value;
        }

        public void SetID(int id)
        {
            _id.Value = id;
        }

        public void SetPack(PackBehaviour packBehaviour)
        {
            _packBehaviour.Value = packBehaviour;
        }

        public void SetTarget(Transform target)
        {
            _target.Value = target;
        }

        #endregion
    }
}
