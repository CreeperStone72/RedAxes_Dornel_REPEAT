using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Norsevar.Interaction
{

    public class TriggerArea : MonoBehaviour
    {

        #region Serialized Fields

        [HideLabel] [SerializeField] [EnumToggleButtons]
        private Trigger trigger;

        [SerializeField] [HideIf("NoCollision")]
        private List<string> tags;

        [SerializeField] private UnityEvent action;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            InvokeEvent(Trigger.OnAwake);
        }

        private void Start()
        {
            InvokeEvent(Trigger.OnStart);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!tags.Any(other.CompareTag)) return;
            if (trigger == Trigger.OnKeyPressed) PlayerInputs.Instance.AddAttack(OnKeyPressed, true);
            InvokeEvent(Trigger.OnTriggerEnter);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!tags.Any(other.CompareTag)) return;
            if (trigger == Trigger.OnKeyPressed)
                PlayerInputs.Instance.PopAttack();
            InvokeEvent(Trigger.OnTriggerExit);
        }

        #endregion

        #region Private Methods

        private void InvokeEvent(Trigger pTrigger)
        {
            if (trigger != pTrigger) return;
            action.Invoke();
        }

        private bool NoCollision()
        {
            return trigger is Trigger.OnAwake or Trigger.OnStart;
        }

        private void OnKeyPressed()
        {
            InvokeEvent(Trigger.OnKeyPressed);
        }

        #endregion

    }

}
