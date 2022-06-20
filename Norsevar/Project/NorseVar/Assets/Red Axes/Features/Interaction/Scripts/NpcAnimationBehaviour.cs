using Sirenix.OdinInspector;
using UnityEngine;

namespace Norsevar.Interaction
{

    public class NpcAnimationBehaviour : MonoBehaviour
    {

        #region Constants and Statics

        private static readonly int Wave = Animator.StringToHash("Wave");

        #endregion

        #region Serialized Fields

        [SerializeField] [Required] private Animator animator;
        [SerializeField] private string visibleTag;

        #endregion

        #region Unity Methods

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(visibleTag))
                return;

            animator.SetTrigger(Wave);
            Destroy(gameObject);
        }

        #endregion

    }

}
