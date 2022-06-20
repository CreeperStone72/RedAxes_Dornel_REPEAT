using UnityEngine;

namespace Norsevar
{
    public class ExitTrigger : MonoBehaviour
    {

        #region Serialized Fields

        [SerializeField] private GameLayout gameLayout;

        #endregion

        private void OnTriggerEnter(Collider other)
        {
            gameLayout.ClearedLevel();
        }

    }
}
