using System.Collections.Generic;
using Norsevar.Stat_System;
using Norsevar.Status_Effect_System;
using UnityEngine;

namespace Norsevar.Combat
{
    public class ChargeAttack : Attack
    {

        #region Constants and Statics

        private static readonly int ChargeAttackAnimProp = Animator.StringToHash("ChargeAttack");

        #endregion

        #region Private Fields

        private readonly ChargingAttackData _chargingAttackData;
        private readonly GameObject _weaponGameObject;

        private bool _isCharging;
        private float _currentChargeTime;

        #endregion

        #region Constructors

        public ChargeAttack(
            AttackData                         data,
            GameObject                         instigator,
            WeaponAttackModificationCollection attackModifications,
            GameObject                         weaponGameObject) : base(data, instigator, attackModifications)
        {
            _chargingAttackData = data as ChargingAttackData;
            _weaponGameObject = weaponGameObject;
        }

        #endregion

        #region Public Methods

        public override List<GameObject> AttackHit(Transform attacker, Vector3 attackPosition, Stat damageStat, Stat critStat)
        {
            if (_currentChargeTime < 1f)
                return null;

            Analytics.AddChargeAttack();

            var forward = attacker.forward;
            var spawnPos = attacker.position + forward * 2f;
            spawnPos.y += 1f;
            ChargeAttackProjectile chargeAttackProjectile =
                Object.Instantiate(_chargingAttackData.ThrowPrefab, spawnPos, Quaternion.identity).GetComponent<ChargeAttackProjectile>();
            chargeAttackProjectile.Init(Data, CreateDamageInfo(damageStat, critStat), attacker, forward);
            // Vector3 targetPos = new(_chargeValue.x, spawnPos.y, _chargeValue.z);

            // axe.transform.DOMove(targetPos, .3f).OnComplete(() => { Object.Destroy(axe); });
            return null;
        }

        public void ChargeEnd(Animator animator)
        {
            _isCharging = false;

            animator.SetBool(ChargeAttackAnimProp, false);
        }

        public void ChargeStart(Animator animator)
        {
            _currentChargeTime = 0;
            _isCharging = true;
            animator.SetBool(ChargeAttackAnimProp, true);
        }

        public override EWeaponAttackType GetAttackType()
        {
            return EWeaponAttackType.ChargeAttack;
        }

        public override void Update()
        {
            if (!_isCharging)
                return;

            if (_currentChargeTime < _chargingAttackData.ChargeTime)
                _currentChargeTime += Time.deltaTime;
        }

        #endregion

    }
}
