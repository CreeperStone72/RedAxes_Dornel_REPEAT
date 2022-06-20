using UnityEngine;

namespace Norsevar.AI
{

    [RequireComponent(typeof(AIDataManager))]
    public abstract class AttackBehaviour : MonoBehaviour, IStrengthen
    {

        #region Private Fields

        private AIStats _aiStats;
        private int _bonus;

        #endregion

        #region Protected Fields

        protected AttackType attackType;

        #endregion

        #region Unity Methods

        protected virtual void Awake()
        {
            _bonus = 0;
            attackType = GetComponent<AIDataManager>().AttackType;
            _aiStats = GetComponent<AIDataManager>().AIStats;
        }

        #endregion

        #region Public Methods

        public void AddUpgrade(int pAmount)
        {
            _bonus += pAmount;
        }

        public abstract void Attack();

        public int GetAttackDamage()
        {
            return (attackType.BaseDamage + _bonus) * _aiStats.Strength;
        }

        #endregion

    }

}
