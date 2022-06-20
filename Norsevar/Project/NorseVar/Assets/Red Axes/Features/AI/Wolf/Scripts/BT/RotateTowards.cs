using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Norsevar.AI
{

    [TaskCategory("Norsevar/Movement")]
    public class RotateTowards : NavMeshAgentBt
    {

        #region Private Methods

        private Vector3 GetTargetDirection()
        {
            return target.Value.position - transform.position;
        }

        private void Rotate()
        {
            Rotate(GetTargetDirection());
        }

        #endregion

        #region Protected Methods

        protected override float GetSpeedModifier()
        {
            return 0;
        }

        #endregion

        #region Public Methods

        public override TaskStatus OnUpdate()
        {
            Vector3 forward = transform.forward;
            Vector3 forward2d = new(forward.x, 0, forward.z);
            Vector3 direction = new(GetTargetDirection().x, 0, GetTargetDirection().z);
            if (Vector3.Angle(forward2d, direction) < rotationEpsilon.Value) return TaskStatus.Success;
            Rotate();
            return TaskStatus.Running;
        }

        #endregion

        public SharedTransform target;
        public SharedFloat rotationEpsilon = 5;
    }

}
