using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Norsevar
{
    public class UnityEventEmitterBehavior : MonoBehaviour
    {

        #region Serialized Fields

        [FormerlySerializedAs("_eventCondition")] [SerializeField]
        private GameEvent eventCondition;

        [FormerlySerializedAs("_event")] [SerializeField]
        private UnityEvent @event;

        #endregion

        #region Unity Methods

        private void Start()
        {
            if (eventCondition == GameEvent.ObjectStart) @event.Invoke();
        }

        private void OnEnable()
        {
            if (eventCondition == GameEvent.ObjectEnable) @event.Invoke();
        }

        private void OnDisable()
        {
            if (eventCondition == GameEvent.ObjectDisable) @event.Invoke();
        }

        private void OnDestroy()
        {
            if (eventCondition == GameEvent.ObjectDestroy) @event.Invoke();
        }

        private void OnCollisionEnter()
        {
            if (eventCondition == GameEvent.CollisionEnter) @event.Invoke();
        }

        private void OnCollisionExit()
        {
            if (eventCondition == GameEvent.CollisionExit) @event.Invoke();
        }

        private void OnMouseDown()
        {
            if (eventCondition == GameEvent.MouseDown) @event.Invoke();
        }

        private void OnMouseEnter()
        {
            if (eventCondition == GameEvent.MouseEnter) @event.Invoke();
        }

        private void OnMouseExit()
        {
            if (eventCondition == GameEvent.MouseExit) @event.Invoke();
        }

        private void OnMouseUp()
        {
            if (eventCondition == GameEvent.MouseUp) @event.Invoke();
        }


        private void OnTriggerEnter(Collider other)
        {
            if (eventCondition == GameEvent.TriggerEnter) @event.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (eventCondition == GameEvent.TriggerExit) @event.Invoke();
        }

        #endregion

        [Flags]
        private enum GameEvent
        {
            None,
            ObjectStart = 1 << 0,
            ObjectDestroy = 1 << 1,
            TriggerEnter = 1 << 2,
            TriggerExit = 1 << 3,
            CollisionEnter = 1 << 4,
            CollisionExit = 1 << 5,
            ObjectEnable = 1 << 6,
            ObjectDisable = 1 << 7,
            MouseEnter = 1 << 8,
            MouseExit = 1 << 9,
            MouseDown = 1 << 10,
            MouseUp = 1 << 1
        }
    }
}
