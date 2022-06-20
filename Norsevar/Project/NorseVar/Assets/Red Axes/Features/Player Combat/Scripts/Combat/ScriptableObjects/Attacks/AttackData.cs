using System.Collections.Generic;
using Norsevar.Status_Effect_System;
using UnityEngine;

namespace Norsevar.Combat
{
    [CreateAssetMenu(fileName = nameof(AttackData), menuName = "Norsevar/Player/Combat/Attacks/Normal Attack")]
    public class AttackData : ScriptableGameObject
    {

        #region Serialized Fields
        
        [Header("Attack Config")]
        [SerializeField] [Range(0, 10)] private float fovRadius = 2f;
        [SerializeField] [Range(0, 360)] private float fovAngle = 100f;
        [SerializeField] private float baseDamage;
        [SerializeField] private LayerMask hittableLayers;
        [SerializeField] private float knockbackForce;
        [SerializeField] private float attackForwardMoveForce;
        
        [Header("Status Effects to apply to Enemies by default")]
        [SerializeField] private List<BaseEffectData> statusEffects;
        [SerializeField] private StatDictionary statModifiers;

        #endregion

        #region Properties

        public float FovRadius => fovRadius;
        public float FovAngle => fovAngle;
        public LayerMask HittableLayers => hittableLayers;
        public float KnockbackForce => knockbackForce;
        public float AttackForwardMoveForce => attackForwardMoveForce;
        public float BaseDamage => baseDamage;
        public List<BaseEffectData> StatusEffects => statusEffects;
        public StatDictionary StatModifiers => statModifiers;

        #endregion
    }
}
