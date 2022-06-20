using BehaviorDesigner.Runtime.Tasks;

namespace Norsevar.AI
{

    [TaskCategory("Norsevar/Wolf")]
    public class IsAnimationRunning : Action
    {

        #region Private Methods

        private bool IsDamagedRunning(out TaskStatus pTaskStatus)
        {
            if (animationBehaviour.Value.IsAnimationByName("Damaged"))
            {
                if (IsRunning(out pTaskStatus))
                    return true;
            }

            pTaskStatus = TaskStatus.Failure;
            return false;
        }

        private bool IsDieRunning(out TaskStatus pTaskStatus)
        {
            if (animationBehaviour.Value.IsAnimationByName("Die"))
            {
                if (IsRunning(out pTaskStatus))
                    return true;
            }

            pTaskStatus = TaskStatus.Failure;
            return false;
        }

        private bool IsRunning(out TaskStatus pTaskStatus)
        {
            if (animationBehaviour.Value.GetAnimationRunning())
            {
                pTaskStatus = TaskStatus.Running;
                return true;
            }

            pTaskStatus = TaskStatus.Failure;
            return false;
        }

        #endregion

        #region Public Methods

        public override TaskStatus OnUpdate()
        {
            if (IsDamagedRunning(out TaskStatus taskStatus))
                return taskStatus;

            return IsDieRunning(out taskStatus) ? taskStatus : TaskStatus.Success;
        }

        #endregion

        public SharedAnimationBehaviour animationBehaviour;
    }

}
