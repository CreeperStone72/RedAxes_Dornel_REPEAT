using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Norsevar.AI.BT
{

    [TaskCategory("Norsevar/Attack")]
    public class Spit : Action
    {

        #region Public Methods

        public override void OnReset()
        {
            origin = null;
            target = null;
            projectile = null;
            angle = 0;
        }

        public override TaskStatus OnUpdate()
        {
            Vector3 forward = origin.Value.forward;
            GameObject instantiate = Object.Instantiate(projectile.Value, origin.Value.position, Quaternion.Euler(forward));
            instantiate.GetComponent<ProjectileLaunch>().SetTargetPos(target.Value, angle.Value);

            return TaskStatus.Success;
        }

        #endregion

        public SharedFloat angle;

        public SharedTransform origin;
        public SharedGameObject projectile;
        public SharedTransform target;
    }

}
