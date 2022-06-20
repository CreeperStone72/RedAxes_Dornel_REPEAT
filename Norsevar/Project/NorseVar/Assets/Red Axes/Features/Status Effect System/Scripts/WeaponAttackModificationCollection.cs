using System;
using System.Collections.Generic;
using Norsevar.Combat;

namespace Norsevar.Status_Effect_System
{
    [Serializable]
    public class WeaponAttackModification
    {

        #region Serialized Fields

        public List<BaseEffectData> attackStatusEffects;
        public float damageMultiplier;

        #endregion

        // public WeaponAttackModification(WeaponAttackModifier modifier)
        // {
        //     attackStatusEffects = new List<BaseEffectData>(modifier.statusEffectsToAdd);
        //     damageMultiplier = 1 + modifier.damageMultiplierToAdd;
        // }
    }

    // [Serializable]
    // public struct WeaponAttackModifier
    // {
    //     public List<BaseEffectData> statusEffectsToAdd;
    //     public float damageMultiplierToAdd;
    //     [HideInInspector] public object source;
    //
    //     public WeaponAttackModifier(List<BaseEffectData> statusEffectsToAdd, float damageMultiplierToAdd, object source)
    //     {
    //         this.statusEffectsToAdd = statusEffectsToAdd;
    //         this.damageMultiplierToAdd = damageMultiplierToAdd;
    //         this.source = source;
    //     }
    // }

    public class WeaponAttackModificationCollection
    {

        #region Private Fields

        private readonly Dictionary<EWeaponAttackType, WeaponAttackModification> _attackModifications;

        #endregion

        #region Constructors

        public WeaponAttackModificationCollection()
        {
            _attackModifications = new Dictionary<EWeaponAttackType, WeaponAttackModification>();
        }

        #endregion

        #region Public Methods

        public void AddDamageMultiplierToAttack(EWeaponAttackType attackType, float damageMultiplierValueToAdd)
        {
            if (!_attackModifications.ContainsKey(attackType))
            {
                _attackModifications.Add(
                    attackType,
                    new WeaponAttackModification
                    {
                        attackStatusEffects = new List<BaseEffectData>(),
                        damageMultiplier = 1 + damageMultiplierValueToAdd
                    });
            }
            else
                _attackModifications[attackType].damageMultiplier += damageMultiplierValueToAdd;
        }

        public void AddStatusEffectToAttack(EWeaponAttackType attackType, BaseEffectData effect)
        {
            if (!_attackModifications.ContainsKey(attackType))
            {
                _attackModifications.Add(
                    attackType,
                    new WeaponAttackModification { attackStatusEffects = new List<BaseEffectData> { effect } });
            }
            else
                _attackModifications[attackType].attackStatusEffects.Add(effect);
        }

        public float GetDamageMultiplierOfAttack(EWeaponAttackType attackType)
        {
            if (!_attackModifications.ContainsKey(attackType))
                return 1;

            return _attackModifications[attackType].damageMultiplier;
        }

        public List<BaseEffectData> GetStatusEffectsOfAttack(EWeaponAttackType attackType)
        {
            if (!_attackModifications.ContainsKey(attackType))
                return null;

            return _attackModifications[attackType].attackStatusEffects;
        }

        public void RemoveStatusEffectFromAttack(EWeaponAttackType attackType, BaseEffectData effect)
        {
            if (!_attackModifications.ContainsKey(attackType))
                return;

            _attackModifications[attackType].attackStatusEffects.Remove(effect);
        }

        public void SubtractDamageMultiplierFromAttack(EWeaponAttackType attackType, float damageMultiplierValueToSubtract)
        {
            if (!_attackModifications.ContainsKey(attackType))
                return;

            _attackModifications[attackType].damageMultiplier -= damageMultiplierValueToSubtract;
        }

        #endregion

    }
}
