using System;
using System.Collections;
using JetBrains.Annotations;
using Norsevar.MusicAndSFX;
using UnityEngine;

namespace Norsevar.AI
{
    public class AnimationBehaviour : MonoBehaviour
    {

        #region Constants and Statics

        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Damaged = Animator.StringToHash("Damaged");
        private static readonly int Turn = Animator.StringToHash("Turn");
        private static readonly int Forward = Animator.StringToHash("Forward");
        private static readonly int Die = Animator.StringToHash("Die");
        private static readonly int Move = Animator.StringToHash("Move");
        private static readonly int Hostile = Animator.StringToHash("Hostile");

        #endregion

        #region Private Fields

        private bool _isDying;
        private bool _isRunning;

        private Animator _animator;
        private Action _attack;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        #endregion

        #region Private Methods

        private bool IsAnimationStillRunning()
        {
            return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
        }

        private void ToggleBool(int id)
        {
            _animator.SetBool(id, !_animator.GetBool(id));
        }

        private IEnumerator WaitForAnimationCoroutine(string animName, Action action, int id)
        {
            yield return WaitForAnimationCoroutine(animName, action);
            ToggleBool(id);
        }

        private IEnumerator WaitForAnimationCoroutine(string animName, Action action)
        {
            while (!(IsAnimationByName(animName) && IsAnimationStillRunning()))
                yield return null;

            action?.Invoke();
        }

        #endregion

        #region Public Methods

        [UsedImplicitly]
        public void AnimationEnded()
        {
            _isRunning = false;
        }

        public bool GetAnimationRunning()
        {
            return _isRunning;
        }

        public bool IsAnimationByName(string animName)
        {
            return _animator.GetCurrentAnimatorStateInfo(0).IsName(animName);
        }

        public void PlayAttack(Action action)
        {
            ToggleBool(Attack);
            NorseGame.Instance.RaiseEvent(
                _animator.name.ToLower().Contains("wolf") ? ENorseGameEvent.Enemies_Wolf_Attack : ENorseGameEvent.Enemies_Snake_Attack,
                transform.position);
            StartCoroutine(WaitForAnimationCoroutine("Attack", action, Attack));
        }

        public void PlayDamaged()
        {
            if (_isDying) return;
            _animator.SetTrigger(Damaged);
            _isRunning = true;
        }

        public void PlayDeath(Action action)
        {
            if (_isDying) return;

            _isDying = true;
            _animator.SetTrigger(Die);
            _isRunning = true;
            FMODGlobalParameterChangeScript.RemoveEnemy();
            StartCoroutine(WaitForAnimationCoroutine("Die", action));
        }

        public virtual void PlayMove(float forward, float turn, float speedMultiplier)
        {
            _animator.SetFloat(Turn, turn, 0.1f, Time.deltaTime);
            _animator.SetFloat(Forward, forward * speedMultiplier, 0.1f, Time.deltaTime);
        }

        public void PlaySnakeMove(bool isMoving, bool isHostile)
        {
            _animator.SetBool(Move, isMoving);
            _animator.SetBool(Hostile, isHostile);
        }

        #endregion

    }

}
