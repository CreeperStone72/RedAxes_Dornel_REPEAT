using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Norsevar.Animals
{
    public class CrowBehaviour : MonoBehaviour
    {

        #region Private Fields

        private bool _flying;
        private Animator _animator;

        #endregion

        #region Serialized Fields

        public List<Transform> destinations;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        #endregion

        #region Public Methods

        public void FlyOutOfScreen()
        {
            NorseGame.Instance.RaiseEvent(ENorseGameEvent.Enviroment_BirdFlyAway, transform.position);
            _flying = true;
            _animator.SetBool("Flying", _flying);
            var dest = destinations[Random.Range(0, destinations.Count)].position;
            transform.LookAt(dest);
            transform.Rotate(new Vector3(0, 90, 0));
            float duration = Vector3.Distance(transform.position, dest) / 3f;
            Destroy(gameObject, duration + 0.5f);
            transform.DOMove(dest, duration);
        }

        #endregion

    }
}
