using UnityEngine;

namespace Norsevar.Combat
{
    [RequireComponent(typeof(CharacterController))]
    public class ForceReceiver : MonoBehaviour
    {

        #region Constants and Statics

        private const float MASS = 3.0f;

        #endregion

        #region Private Fields

        private Vector3 _force = Vector3.zero;
        private CharacterController _character;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _character = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (_force.magnitude > 0.2f)
                _character.Move(_force * Time.deltaTime);

            _force = Vector3.Lerp(_force, Vector3.zero, 5 * Time.deltaTime);
        }

        #endregion

        #region Public Methods

        public void AddForce(Vector3 dir, float force)
        {
            dir.Normalize();

            if (dir.y < 0)
                dir.y = -dir.y;

            _force += dir.normalized * force / MASS;
        }

        #endregion

    }
}
