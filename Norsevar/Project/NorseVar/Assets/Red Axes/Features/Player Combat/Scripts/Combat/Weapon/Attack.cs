using System.Collections.Generic;
using Norsevar.AI;
using Norsevar.Stat_System;
using Norsevar.Status_Effect_System;
using Norsevar.UI;
using UnityEngine;

namespace Norsevar.Combat
{
    public class Attack
    {

        #region Private Fields

        private readonly WeaponAttackModificationCollection _attackModifications;
        private readonly Collider[] _attackHitColliders;

        #endregion

        #region Protected Fields

        protected readonly GameObject Instigator;

        #endregion

        #region Constructors

        public Attack(AttackData data, GameObject instigator, WeaponAttackModificationCollection attackModifications)
        {
            Data = data;
            Instigator = instigator;
            _attackModifications = attackModifications;
            _attackHitColliders = new Collider[20];
        }

        #endregion

        #region Properties

        public AttackData Data { get; }

        #endregion

        #region Private Methods

        private (float, bool) CalculateDamageValue(Stat damageStat, Stat critStat)
        {
            bool isCrit = GetCrit(critStat);

            float damageValue = Data.BaseDamage;

            if (_attackModifications != null)
                damageValue *= _attackModifications.GetDamageMultiplierOfAttack(GetAttackType());

            damageValue *= damageStat.Value;

            if (isCrit)
                damageValue *= 2;

            return (damageValue, isCrit);
        }

        private static bool GetCrit(Stat critStat)
        {
            float baseValue = critStat.Value;
            float chance = Random.Range(0f, 1f);
            return chance <= baseValue;
        }

        private void HandleHit(
            Component               collider,
            Vector3                 dirToTarget,
            ICollection<GameObject> hits,
            RaycastHit              hit,
            Stat                    damageStat,
            Stat                    critStat)
        {
            DamageInfo damageInfo = CreateDamageInfo(damageStat, critStat);

            IFeedback componentInChildren = collider.transform.root.GetComponentInChildren<IFeedback>();
            componentInChildren?.SetIsCrit(damageInfo.IsCrit);

            IDamageable b = collider.GetComponentInParent<IDamageable>();
            float? damageDealt = b?.ReceiveDamage(damageInfo);

            IKickbackAble a = collider.GetComponentInParent<IKickbackAble>();
            a?.Kickback(dirToTarget * Data.KnockbackForce, damageDealt);


            hits.Add(collider.gameObject);
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Player_Hit, hit.point);
            if (collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                NorseGame.Instance.RaiseEvent(ENorseGameEvent.Player_HitEnemy, hit.point);
        }

        #endregion

        #region Protected Methods

        protected DamageInfo CreateDamageInfo(Stat damageStat, Stat critStat)
        {
            (float damageValue, bool isCrit) = CalculateDamageValue(damageStat, critStat);
            DamageInfo damageInfo = new()
            {
                SourceGameObject = Instigator,
                DamageType = EDamageType.Physical,
                EffectsToApply = _attackModifications?.GetStatusEffectsOfAttack(GetAttackType()),
                DamageValue = damageValue,
                IsCrit = isCrit
            };


            if (isCrit)
                Analytics.AddCrit();

            Analytics.UpdateMaxDamage(damageInfo.DamageValue);

            return damageInfo;
        }

        #endregion

        #region Public Methods

        public virtual void AttackEnd(Animator animator)
        {
        }

        public virtual List<GameObject> AttackHit(Transform attacker, Vector3 attackPosition, Stat damageStat, Stat critStat)
        {
            List<GameObject> hits = new();
            Vector3 position = attacker.position;
            int numberOfHits = Physics.OverlapSphereNonAlloc(position, Data.FovRadius, _attackHitColliders, Data.HittableLayers);

            for (int i = 0; i < numberOfHits; i++)
            {
                Collider collider = _attackHitColliders[i];
                Vector3 targetPos = collider.transform.position;
                targetPos.y = position.y + .5f;
                Vector3 dirToTarget = (targetPos - position).normalized;
                float distToTarget = Vector3.Distance(position, targetPos);

                // float dot = Vector3.Dot(dirToTarget, transform.forward);
                if (!(Vector3.Angle(attacker.forward, dirToTarget) < Data.FovAngle / 2) && distToTarget > 1f)
                    continue;

                if (!Physics.Raycast(new Ray(position, dirToTarget), out RaycastHit hit, distToTarget))
                    continue;

                HandleHit(collider, dirToTarget, hits, hit, damageStat, critStat);
            }

            if (Data.name.Contains("Ground"))
                NorseGame.Instance.RaiseEvent(ENorseGameEvent.Player_Attack_GroundSlam, new Vector3(attackPosition.x, attackPosition.y, attackPosition.z));
            else
                NorseGame.Instance.RaiseEvent(ENorseGameEvent.Player_Attack_AxeSwing, attackPosition);

            return hits;
        }

        public virtual EWeaponAttackType GetAttackType()
        {
            return EWeaponAttackType.None;
        }

        public virtual void Update()
        {
        }

        #endregion

    }
}
