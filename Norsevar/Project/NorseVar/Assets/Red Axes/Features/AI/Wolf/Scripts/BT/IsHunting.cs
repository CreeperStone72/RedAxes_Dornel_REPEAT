using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Norsevar.AI
{

    [TaskCategory("Norsevar/Wolf")]
    public class IsHunting : Conditional
    {

        #region Private Methods

        private bool WithinDistance(Transform targetTransform)
        {
            Vector3 direction = targetTransform.position - transform.position;
            return spottingDistance.Value >= direction.magnitude;
        }

        private bool WolvesHunting()
        {
            return packBehaviour.Value.IsAWolfHunting();
        }

        #endregion

        #region Public Methods

        public override TaskStatus OnUpdate()
        {
            if (target.Value is null)
                return TaskStatus.Running;
            return WithinDistance(target.Value) || WolvesHunting() ? TaskStatus.Success : TaskStatus.Failure;
        }

        #endregion

        public SharedFloat spottingDistance;
        public SharedPackBehaviour packBehaviour;
        public SharedTransform target;
    }

}
