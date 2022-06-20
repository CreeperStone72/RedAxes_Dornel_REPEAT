using System;
using System.Collections.Generic;
using Norsevar.Stat_System;
using Norsevar.Status_Effect_System;
using Norsevar.Upgrade_System;
using UnityEngine;

namespace Norsevar.Combat
{
    public class Weapon
    {

        #region Delegates and Events

        public event Action OnAttackEnd;

        public event Action<Attack, List<GameObject>> OnWeaponHit;

        #endregion

        #region Private Fields

        private readonly GameObject _ownerGameObject;
        private readonly GameObject _weaponGameObject;
        private readonly Animator _ownerAnimator;
        private readonly StatController _ownerStats;
        private readonly WeaponAttackModificationCollection _modificationCollection;

        private List<BasicAttack> _basicAttacks;
        private AttackCombo _combo;
        private SpecialAttack _specialAttack;
        private ChargeAttack _chargeAttack;
        private DashAttack _dashAttack;

        #endregion

        #region Constructors

        public Weapon(
            WeaponData                         data,
            GameObject                         owner,
            GameObject                         weapon,
            Animator                           ownerAnimator,
            StatController                     ownerStats             = null,
            WeaponAttackModificationCollection modificationCollection = null)
        {
            Data = data;
            _ownerGameObject = owner;
            _weaponGameObject = weapon;
            _ownerAnimator = ownerAnimator;
            _ownerStats = ownerStats;
            _modificationCollection = modificationCollection;

            StatModifiers = data.StatModifiers == null ? new Dictionary<EStatType, StatModifier>() :
                new Dictionary<EStatType, StatModifier>(data.StatModifiers);

            InitializeAttacks();
            ApplyStatModifiers();
        }

        #endregion

        #region Properties

        public Dictionary<EStatType, StatModifier> StatModifiers { get; }
        public WeaponData Data { get; }
        public Attack CurrentAttack { get; private set; }

        #endregion

        #region Private Methods

        private void ApplyStatModifiers()
        {
            if (_ownerStats == null)
                return;

            foreach ((EStatType eStatType, StatModifier statModifier) in StatModifiers)
            {
                statModifier.Source = this;
                _ownerStats.AddModifierToStat(eStatType, statModifier);
            }
        }

        private void AttackEnd()
        {
            CurrentAttack?.AttackEnd(_ownerAnimator);
            CurrentAttack = null;
            _combo?.Reset(_ownerAnimator);
            OnAttackEnd?.Invoke();
        }

        private void InitializeAttacks()
        {
            _basicAttacks = new List<BasicAttack>();
            Data.BasicAttacks.ForEach(data => _basicAttacks.Add(new BasicAttack(data, _ownerGameObject, _modificationCollection)));
            _combo = new AttackCombo(_basicAttacks);

            _specialAttack = new SpecialAttack(Data.SpecialAttack, _ownerGameObject, _modificationCollection);
            _chargeAttack = new ChargeAttack(Data.ChargeAttack, _ownerGameObject, _modificationCollection, _weaponGameObject);
            _dashAttack = new DashAttack(Data.DashAttack, _ownerGameObject, _modificationCollection);
        }

        #endregion

        #region Public Methods

        public void ApplyUpgrade(WeaponUpgradeData upgradeData)
        {
            foreach (WeaponUpgradeData.AttackStatusEffect attackStatusEffect in upgradeData.StatusEffects)
                _modificationCollection.AddStatusEffectToAttack(attackStatusEffect.attackType, attackStatusEffect.effectData);

            foreach ((EWeaponAttackType eWeaponAttackType, float value) in upgradeData.WeaponAttackDamageMultiplierModifiers)
                _modificationCollection.AddDamageMultiplierToAttack(eWeaponAttackType, value);
        }

        public void Attack()
        {
            CurrentAttack = _combo.Attack(CurrentAttack, _ownerAnimator);
        }

        public void ChargeAttackChargeEnd()
        {
            _chargeAttack?.ChargeEnd(_ownerAnimator);
            // CurrentAttack = null;
        }

        public bool ChargeAttackChargeStart()
        {
            if (CurrentAttack != null)
                return false;

            CurrentAttack = _chargeAttack;
            _chargeAttack?.ChargeStart(_ownerAnimator);
            return true;
        }

        public void HandleAnimationEvent(EAnimationEventType animationEventType)
        {
            switch (animationEventType)
            {
                case EAnimationEventType.AttackHit:
                    if (CurrentAttack != null)
                    {
                        Transform transform = _ownerGameObject.transform;
                        List<GameObject> hits = CurrentAttack.AttackHit(
                            transform,
                            transform.position,
                            _ownerStats[EStatType.DamageMultiplier],
                            _ownerStats[EStatType.CriticalStrikeChance]);
                        OnWeaponHit?.Invoke(CurrentAttack, hits);
                    }
                    break;
                case EAnimationEventType.ComboDelayStart:
                    _combo.ComboTimeStart();
                    break;
                case EAnimationEventType.ComboDelayEnd:
                    CurrentAttack = _combo.ComboCheck(_ownerAnimator);
                    if (CurrentAttack == null) AttackEnd();
                    break;
                case EAnimationEventType.AttackEnd:
                    AttackEnd();
                    break;
            }
        }

        public void PerformDashAttack()
        {
            if (CurrentAttack != null)
                return;

            CurrentAttack = _dashAttack;
            _dashAttack.Perform(_ownerAnimator);
        }

        public void PerformSpecialAttack()
        {
            if (CurrentAttack != null)
                return;

            CurrentAttack = _specialAttack;
            _specialAttack.Perform(_ownerAnimator);
        }

        public void RemoveStatModifiers()
        {
            _ownerStats.RemoveAllModifiersOfSource(this);
        }

        public void Update()
        {
            CurrentAttack?.Update();
        }

        #endregion

    }
}
