using Norsevar.Stat_System;
using Norsevar.Status_Effect_System;
using UnityEngine;

namespace Norsevar.Combat
{
    [RequireComponent(typeof(HealthBase))]
    public class CombatEntityController : MonoBehaviour, IEffectable
    {

        #region Protected Fields

        protected HealthBase healthBehaviour;
        protected StatController statController;

        #endregion

        #region Serialized Fields

        [Header("Stats")] [SerializeField]
        private StatCollectionData statCollectionData;

        #endregion

        #region Public Properties

        public Stat this[EStatType index] => statController[index];

        #endregion

        #region Protected Methods

        protected virtual void Init()
        {
            statController = new StatController(statCollectionData);
            healthBehaviour = GetComponent<HealthBase>();
            healthBehaviour.SetStats(statController.Stats);

        }

        protected virtual void OnDie()
        {
            // Debug.Log($"Entity {gameObject.name} died!");
        }

        protected virtual void OnHit(float f)
        {
            // Debug.Log($"Entity {gameObject.name} was hit!");
        }

        #endregion

        #region Public Methods

        //Method is called by the StatusEffectController add modifiers to stats (slow for example)
        public virtual void EffectApplyStatModifier(EStatType stat, StatModifier modifier)
        {
            statController.AddModifierToStat(stat, modifier);
        }

        //Method is called by the StatusEffectController to apply damage by damage effects (poison for example)
        public virtual void EffectDamage(DamageInfo damageInfo)
        {
            healthBehaviour.ReceiveDamage(damageInfo);
        }

        //Method is called by the StatusEffectController to remove stat modifiers when the effect ends.
        public virtual void EffectRemoveStatModifiers(object source)
        {
            statController.RemoveAllModifiersOfSource(source);
        }

        #endregion
        
    }
}
