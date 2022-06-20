using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Norsevar.AI.BT
{

    [TaskCategory("Norsevar/Movement")]
    public class LookAt : Action
    {

        #region Public Methods

        public override void OnReset()
        {
            target = null;
        }

        public override TaskStatus OnUpdate()
        {
            if (target.Value != null)
                transform.LookAt(transform.position + (transform.position - target.Value.position).normalized);

            return TaskStatus.Success;
        }

        #endregion

        [Tooltip("The GameObject to look at.")]
        public SharedTransform target;
    }

}
