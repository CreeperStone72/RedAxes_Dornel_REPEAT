using System.Collections.Generic;
using Norsevar.Stat_System;
using UnityEngine;

namespace Norsevar.Combat
{

    [CreateAssetMenu(fileName = nameof(WeaponData), menuName = "Norsevar/Player/Combat/Weapon")]
    public class WeaponData : ScriptableGameObject
    {

        #region Serialized Fields

        [SerializeField] private List<AttackData> basicAttacks;
        [SerializeField] private AttackData chargeAttack;
        [SerializeField] private AttackData specialAttack;
        [SerializeField] private AttackData dashAttack;
        [SerializeField] private Mesh weaponMesh;
        [SerializeField] private StatDictionary statModifiers;

        #endregion

        #region Properties

        public AttackData ChargeAttack => chargeAttack;
        public AttackData SpecialAttack => specialAttack;
        public List<AttackData> BasicAttacks => basicAttacks;
        public AttackData DashAttack => dashAttack;
        public Mesh WeaponMesh => weaponMesh;
        public StatDictionary StatModifiers => statModifiers;

        #endregion

        #region Public Methods

        public void ApplyStatModifiers(StatController statController)
        {
            foreach ((EStatType eStatType, StatModifier statModifier) in StatModifiers)
            {
                statModifier.Source = this;
                statController.AddModifierToStat(eStatType, statModifier);
            }
        }

        public AttackData GetBasicAttack(int index)
        {
            if (index < 0 || index >= BasicAttacks.Count)
                return null;

            return BasicAttacks[index];
        }

        public void RemoveStatModifiers(StatController statController)
        {
            statController.RemoveAllModifiersOfSource(this);
        }

        #endregion

    }

}
