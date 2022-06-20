using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEditor;
using UnityEngine;

namespace Norsevar.AI.BT
{

    [TaskCategory("Norsevar")]
    public class WithinDistance : Conditional
    {

        #region Private Fields

        // distance * distance, optimization so we don't have to take the square root
        private float _sqrMagnitude;

        #endregion

        #region Public Methods

        // Draw the seeing radius
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (Owner == null || magnitude == null) return;
            Color oldColor = Handles.color;
            Handles.color = Color.yellow;
            Transform transform1 = Owner.transform;
            Handles.DrawWireDisc(transform1.position, transform1.up, magnitude.Value);
            Handles.color = oldColor;
#endif
        }

        public override void OnReset()
        {
            targetObject = null;
            magnitude = 5;
        }

        public override void OnStart()
        {
            _sqrMagnitude = magnitude.Value * magnitude.Value;
        }

        public override TaskStatus OnUpdate()
        {
            if (transform == null || targetObject.Value == null)
                return TaskStatus.Failure;

            if (targetObject.Value == null || targetObject.Value == transform) return TaskStatus.Failure;
            Vector3 direction = targetObject.Value.transform.position - transform.position;

            return !(Vector3.SqrMagnitude(direction) <= _sqrMagnitude) ? TaskStatus.Failure : TaskStatus.Success;

        }

        #endregion

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The object that we are searching for")]
        public SharedTransform targetObject;

        [BehaviorDesigner.Runtime.Tasks.Tooltip("The distance that the object needs to be within")]
        public SharedFloat magnitude = 5;
    }

}
