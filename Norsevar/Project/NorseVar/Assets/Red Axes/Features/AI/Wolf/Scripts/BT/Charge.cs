using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using DG.Tweening;
using Norsevar.Combat;
using UnityEngine;

namespace Norsevar.AI
{
    [TaskCategory("Norsevar/Movement")]
    public class Charge : NavMeshAgentBt
    {

        #region Private Fields

        private Vector3 _currentDestination;
        private TaskStatus _taskStatus;
        private Vector3 _oldColliderSize;

        #endregion

        #region Private Methods

        private Vector3 GetChargePosition()
        {
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Enemies_ChargeWolf_End, transform.position);
            return transform.position + transform.forward * chargeDistance.Value;
        }

        #endregion

        #region Protected Methods

        protected override float GetSpeedModifier()
        {
            return speed.Value;
        }

        #endregion

        #region Public Methods

        public override void OnStart()
        {
            base.OnStart();
            _currentDestination = GetChargePosition();
            DisableNavMesh();
            _oldColliderSize = collider.size;
            collider.size = new Vector3(colliderXSize.Value, _oldColliderSize.y, _oldColliderSize.z);
            _taskStatus = TaskStatus.Running;

            if (Physics.BoxCast(
                collider.center + transform.position,
                collider.size / 2f,
                transform.forward,
                out RaycastHit hitInfo,
                Quaternion.identity,
                chargeDistance.Value,
                blockingLayerMask.Value,
                QueryTriggerInteraction.Collide))
            {
                Vector3 forward = transform.forward;
                _currentDestination = hitInfo.transform.position - new Vector3(forward.x, 0, forward.z) * 1.5f;
                if (Physics.Raycast(_currentDestination, Vector3.down, out hitInfo, 5, terrainLayerMask.Value))
                    _currentDestination.y = hitInfo.point.y;
            }

            if (Physics.BoxCast(
                collider.center + transform.position,
                collider.size / 2f,
                transform.forward,
                out hitInfo,
                Quaternion.identity,
                chargeDistance.Value,
                playerLayerMask.Value,
                QueryTriggerInteraction.Collide))
            {
                hitInfo.transform.GetComponent<IDamageable>().ReceiveDamage(
                    new DamageInfo { DamageType = EDamageType.Physical, DamageValue = 20, CanBeBlocked = false });
                NorseGame.Instance.RaiseEvent(ENorseGameEvent.Enemies_ChargeWolf_Impact, transform.position);
            }

            transform.DOMove(_currentDestination, speed.Value).SetEase(Ease.OutExpo).OnComplete(
                () =>
                {
                    collider.size = _oldColliderSize;
                    EnableNavMesh();
                    _taskStatus = TaskStatus.Success;
                });
        }

        public override TaskStatus OnUpdate()
        {
            return _taskStatus;
        }

        #endregion

        public SharedFloat chargeDistance;
        public SharedFloat speed;
        public SharedFloat colliderXSize;
        public SharedLayerMask playerLayerMask;
        public SharedLayerMask blockingLayerMask;
        public SharedLayerMask terrainLayerMask;
    }


}
