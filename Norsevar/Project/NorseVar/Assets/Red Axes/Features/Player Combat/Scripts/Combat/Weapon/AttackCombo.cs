using System.Collections.Generic;
using UnityEngine;

namespace Norsevar.Combat
{
    public class AttackCombo
    {

        #region Constants and Statics

        private static readonly int AttackHash = Animator.StringToHash("Attack");
        private static readonly int ComboCount = Animator.StringToHash("ComboCount");
        private static readonly int ComboSuccess = Animator.StringToHash("ComboSuccess");

        #endregion

        #region Private Fields

        private readonly List<BasicAttack> _attacks;

        private int _currentAttackIndex;
        private bool _comboSuccess, _isWithinComboDelay;

        #endregion

        #region Constructors

        public AttackCombo(List<BasicAttack> attacks)
        {
            _attacks = attacks;
            _attacks[^1].IsFinalComboAttack = true;
        }

        #endregion

        #region Private Methods

        private BasicAttack GetNext()
        {
            if (_attacks.Count == 0)
                return null;

            ++_currentAttackIndex;

            if (_currentAttackIndex < 0 || _currentAttackIndex >= _attacks.Count)
                return null;

            return _attacks[_currentAttackIndex];
        }

        #endregion

        #region Public Methods

        public Attack Attack(Attack currentAttack, Animator animator)
        {
            switch (currentAttack)
            {
                case null:
                {
                    Analytics.AddAttack();
                    _currentAttackIndex = -1;
                    BasicAttack next = GetNext();
                    animator.SetTrigger(AttackHash);
                    animator.SetInteger(ComboCount, _currentAttackIndex);
                    return next;
                }
                case BasicAttack attack when _attacks.Contains(attack):
                    _comboSuccess = _isWithinComboDelay;
                    break;
            }

            return currentAttack;
        }

        public Attack ComboCheck(Animator animator)
        {
            BasicAttack nextComboAttack = GetNext();

            if (_comboSuccess && nextComboAttack != null)
                animator.SetTrigger(ComboSuccess);
            else
            {
                _currentAttackIndex = -1;
                nextComboAttack = null;
            }

            animator.SetInteger(ComboCount, _currentAttackIndex);
            _isWithinComboDelay = _comboSuccess = false;
            return nextComboAttack;
        }

        public void ComboTimeStart()
        {
            _isWithinComboDelay = true;
        }

        public void Reset(Animator animator)
        {
            _currentAttackIndex = -1;
            _isWithinComboDelay = _comboSuccess = false;
            animator.SetInteger(ComboCount, -1);
        }

        #endregion

    }
}
