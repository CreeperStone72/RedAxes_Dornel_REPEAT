using System;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.Events;
using Action = BehaviorDesigner.Runtime.Tasks.Action;

namespace Norsevar.AI
{
    [TaskCategory("Norsevar/Event")]
    public class UnityEventBtAction : Action
    {

        #region Public Methods

        public override TaskStatus OnUpdate()
        {
            Event.Value.Invoke();
            return TaskStatus.Success;
        }

        #endregion

        public SharedUnityEvent Event;
    }

    [Serializable]
    public class SharedUnityEvent : SharedVariable<UnityEvent>
    {

        #region Public Methods

        public static implicit operator SharedUnityEvent(UnityEvent value)
        {
            return new SharedUnityEvent { mValue = value };
        }

        #endregion

    }

}
