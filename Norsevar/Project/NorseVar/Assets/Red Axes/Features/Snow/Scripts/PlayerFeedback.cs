using MoreMountains.Tools;
using UnityEngine;

namespace Norsevar.Snow
{
    public class PlayerFeedback : MonoBehaviour
    {

        #region Constants and Statics

        private static readonly int Velocity = Animator.StringToHash("Velocity");

        #endregion

        #region Private Fields

        private ParticleSystem _snowCollisionPS;
        private ParticleSystem _breathPS;

        #endregion

        #region Serialized Fields

        [SerializeField] private Animator animator;
        [SerializeField] private GameObject vfxParent;
        [SerializeField] private GameObject snowCollisionPsgo;
        [SerializeField] private GameObject breathPsgo;
        [SerializeField] private GameObject leftFoot;
        [SerializeField] private GameObject rightFoot;
        [SerializeField] private GameObject mouth;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _snowCollisionPS = Instantiate(snowCollisionPsgo, transform).GetComponent<ParticleSystem>();
            _breathPS = Instantiate(breathPsgo, vfxParent.transform).GetComponentInChildren<ParticleSystem>();
            MMFollowTarget mmfollow = _breathPS.transform.parent.GetComponent<MMFollowTarget>();
            mmfollow.Target = mouth.transform;
            _snowCollisionPS.transform.position += Vector3.up * 1000;
            _snowCollisionPS.Play();
        }

        #endregion

        #region Public Methods

        public void OnBreathe(AnimationEvent info)
        {
            _breathPS.Play();
        }

        public void OnFootTouchSnow(AnimationEvent info)
        {
            if (!(animator.GetFloat(Velocity) > 0.1f))
                return;
            _snowCollisionPS.transform.position =
                info.stringParameter == "LeftFoot" ? leftFoot.transform.position : rightFoot.transform.position;
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Player_Movement_FootstepSnow, animator.transform.position);
            _snowCollisionPS.Play();
        }

        #endregion

    }
}
