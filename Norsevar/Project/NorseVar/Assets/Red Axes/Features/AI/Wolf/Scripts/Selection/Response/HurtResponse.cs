using GD.Selection;
using Norsevar.Combat;
using UnityEngine;

namespace Norsevar.AI
{

    public class HurtResponse : ISelectionResponse
    {

        #region Private Fields

        private readonly AttackBehaviour _attackBehaviour;
        private DamageInfo _damageInfo;

        #endregion

        #region Constructors

        public HurtResponse(AttackBehaviour pAttackBehaviour)
        {
            _attackBehaviour = pAttackBehaviour;
        }

        #endregion

        #region Public Methods

        public void OnDeselect(Transform pTransform)
        {
        }

        public void OnSelect(Transform pTransform)
        {
            _damageInfo = new DamageInfo
            {
                DamageType = EDamageType.Physical,
                DamageValue = _attackBehaviour.GetAttackDamage()
            };

            pTransform.GetComponent<IDamageable>().ReceiveDamage(_damageInfo);
        }

        #endregion

    }

}
