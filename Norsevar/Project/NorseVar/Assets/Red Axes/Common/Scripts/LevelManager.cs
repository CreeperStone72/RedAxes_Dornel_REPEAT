using DG.Tweening;
using Norsevar.Combat;
using Norsevar.Spawner;
using UnityEngine;
using UnityEngine.UI;

namespace Norsevar
{

    public class LevelManager : MonoBehaviour
    {

        #region Private Fields

        private GameObject _player;

        private GameObject _camera;

        private ISpawn _currentSpawner;

        #endregion

        #region Serialized Fields

        [SerializeField] private Image image;
        [SerializeField] private GameObject buttons;

        [SerializeField] private Vector3 playerStartPos;

        #endregion

        #region Private Methods

        private void OnFadeInComplete()
        {
            _player.SetActive(false);
            buttons.SetActive(true);
            _player.transform.position = playerStartPos;
            _currentSpawner.Despawn();
        }

        #endregion

        #region Public Methods

        public void ReloadScene()
        {
            _player ??= NorseGame.Instance.Get<PlayerController>().gameObject;

            image.DOFade(1, 2).OnComplete(OnFadeInComplete);
        }

        public void Spawn(GameObject spawner)
        {
            _player ??= NorseGame.Instance.Get<PlayerController>().gameObject;
            _player.SetActive(true);
            _currentSpawner = spawner.GetComponent<ISpawn>();
            buttons.SetActive(false);

            image.DOFade(0, 2).OnComplete(() => { _currentSpawner.Spawn(); });
        }

        #endregion

    }

}
