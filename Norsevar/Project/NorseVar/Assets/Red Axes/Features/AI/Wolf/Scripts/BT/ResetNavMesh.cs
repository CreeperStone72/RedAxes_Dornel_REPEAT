using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

namespace Norsevar.AI
{
    [TaskCategory("Norsevar/Movement")]
    public class ResetNavMesh : NavMeshAgentBt
    {

        #region Private Fields

        private Rigidbody _rigidbody;

        #endregion

        #region Protected Methods

        protected override float GetSpeedModifier()
        {
            return 0;
        }

        #endregion

        #region Public Methods

        public override void OnStart()
        {
            base.OnStart();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override TaskStatus OnUpdate()
        {
            Reset();

            switch (_rigidbody.velocity.magnitude)
            {
                case 0:
                    _rigidbody.AddForce(direction.Value, ForceMode.Force);
                    return TaskStatus.Running;
                case > .3f:
                    return TaskStatus.Running;
            }

            _rigidbody.velocity = Vector3.zero;
            _rigidbody.isKinematic = true;
            return TaskStatus.Success;
        }

        #endregion

        public SharedVector3 direction;
    }
}
