using System.Collections.Generic;
using System.Linq;
using Norsevar.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace Norsevar
{
    public class NorseInteractableTrigger : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField]
        private List<string> tags;

        [SerializeField] private UnityEvent onTriggerEnter;
        [SerializeField] private UnityEvent onTriggerExit;
        [SerializeField] private UnityEvent onKeyPressed;

        private bool attackDeactivated;
        
        #endregion

        #region Unity Methods

        private void OnTriggerExit(Collider other)
        {
            if (!tags.Any(other.CompareTag)) return;
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Prompt_InteractStop);
            PlayerInputs.Instance.PopAttack();
            NorseGame.Instance.Get<PlayerController>().AttackEnabled = true; 
            attackDeactivated = false;
            onTriggerExit.Invoke();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!tags.Any(other.CompareTag)) return;
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Prompt_Interact);
            PlayerInputs.Instance.AddAttack(OnKeyPressed, true);
            NorseGame.Instance.Get<PlayerController>().AttackEnabled = false;
            attackDeactivated = true;
            onTriggerEnter.Invoke();
        }

        #endregion

        #region Private Methods

        private void OnKeyPressed()
        {
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Interaction_PointOfInterest);
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Prompt_InteractStop);
            onKeyPressed.Invoke();
        }

        #endregion


        private void OnDisable()
        {
            if (attackDeactivated)
            {
                PlayerInputs.Instance.PopAttack();
                NorseGame.Instance.Get<PlayerController>().AttackEnabled = true;
                attackDeactivated = false;
            }
        }
    }
}
