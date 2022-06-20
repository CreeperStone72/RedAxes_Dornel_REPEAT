using System;
using MoreMountains.Feedbacks;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Norsevar.AI
{

    public class SnakeStateBehaviour : StateBehaviour<ESnakeStates>
    {

        #region Private Fields

        private MoveSnakeState _move;
        private AttackSnakeState _attack;
        private HitSnakeState _hit;
        private SearchSnakeState _search;

        private SnakeStates _states;

        #endregion

        #region Serialized Fields

        [SerializeField] private MMFeedbacks hitFeedback;

        #endregion

        #region Unity Methods

        protected override void Awake()
        {
            base.Awake();
            _states = aiStates as SnakeStates;

            StateMachine = new StateMachine<ESnakeStates>();
            _move = new MoveSnakeState(this);
            _attack = new AttackSnakeState(this);
            _hit = new HitSnakeState(this);
            _search = new SearchSnakeState(this);

            StateMachine.AddAnyTransition(_hit, IsHit);

            StateMachine.AddTransition(_search, _attack, TargetIsInAggressionRange);

            StateMachine.AddTransition(_move, _attack, TargetIsNotClose);

            StateMachine.AddTransition(_attack, _search, TargetIsNotInAggressionRange);
            StateMachine.AddTransition(_attack, _move, TargetIsClose);

            StateMachine.SetState(_search);
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_states is null) return;
            Handles.color = Color.red;
            Vector3 position = transform.position;
            Handles.DrawWireDisc(position, Vector3.up, _states.AggressionRange);
            Handles.color = Color.blue;
            Handles.DrawWireDisc(position, Vector3.up, _states.FleeDistance);
            Handles.color = Color.cyan;
            Handles.DrawWireDisc(position, Vector3.up, _states.StopFleeDistance);
        }
#endif

        #endregion

        #region Private Methods

        private bool TargetIsClose()
        {
            return _states.FleeDistance > GetDistanceToTarget();
        }

        private bool TargetIsNotClose()
        {
            return _states.StopFleeDistance < GetDistanceToTarget();
        }

        #endregion

        #region Protected Methods

        protected override IState<ESnakeStates> GetState(ESnakeStates pEnum)
        {
            return pEnum switch
            {
                ESnakeStates.Move   => _move,
                ESnakeStates.Attack => _attack,
                ESnakeStates.Hit    => _hit,
                ESnakeStates.Search => _search,
                _                   => throw new ArgumentOutOfRangeException(nameof(pEnum), pEnum, null)
            };
        }

        protected override void UpdateAnimator()
        {
            animationBehaviour.PlaySnakeMove(StateMachine.State is ESnakeStates.Move or ESnakeStates.Search, TargetIsInAggressionRange());
        }

        #endregion

        #region Public Methods

        public float GetSearchDistance()
        {
            return _states.SearchDistance;
        }

        public override float GetSpeedMultiplier()
        {
            return _states.SlitherSpeed;
        }

        public float GetTimeBetweenAttacks()
        {
            return _states.TimeBetweenAttacks;
        }

        public override void Kickback(Vector3 dirToTarget, float? damageInfo)
        {
            base.Kickback(dirToTarget, damageInfo);
            _hit.SetDirection(dirToTarget);
            _hit.SetPreviousState(StateMachine.State);
            if (damageInfo is not null)
                hitFeedback?.PlayFeedbacks(transform.position, (int)damageInfo);
        }

        public virtual void PlayAttackAnimation()
        {
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Enemies_Snake_Attack, transform.position);
            animationBehaviour.PlayAttack(Attack);
        }

        public bool TargetIsInAttackRange()
        {
            return TargetIsInAggressionRange();
        }

        #endregion

    }

}
