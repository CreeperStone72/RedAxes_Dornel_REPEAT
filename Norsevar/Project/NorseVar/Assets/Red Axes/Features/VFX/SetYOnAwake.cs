using UnityEngine;
using UnityEngine.Serialization;

namespace Norsevar.VFX
{
    public class SetYOnAwake : MonoBehaviour
    {

        #region Serialized Fields

        [FormerlySerializedAs("_y")] [SerializeField]
        private float y;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }

        #endregion

    }
}
