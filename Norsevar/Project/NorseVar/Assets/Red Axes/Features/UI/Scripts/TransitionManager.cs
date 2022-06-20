using DG.Tweening;
using Norsevar.Room_Generation;
using UnityEngine;
using UnityEngine.UI;

namespace Norsevar.UI
{
    public class TransitionManager : MonoBehaviour
    {

        #region Serialized Fields

        [SerializeField] [Header("Fade")]
        private Image image;
        [SerializeField] private int duration = 3;

        [SerializeField] [Header("Extra")]
        private GameObject buttonSelection;
        [SerializeField] private GameLayout gameLayout;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            buttonSelection.SetActive(false);
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }


        private void Start()
        {
            image.DOFade(1, duration).OnComplete(OnEndOfTransition);
        }

        private void OnEnable()
        {
            ButtonsManager.OnButtonClicked += HandleButtonClicked;
        }

        private void OnDisable()
        {
            ButtonsManager.OnButtonClicked -= HandleButtonClicked;
        }

        #endregion

        #region Private Methods

        private void HandleButtonClicked(int index, int type)
        {
            buttonSelection.SetActive(false);
            image.DOFade(0, duration).OnComplete(() => OnEndOfExitTransition(index, type));
        }

        private void OnEndOfExitTransition(int index, int type)
        {
            RoomGeneratorApi.Instance.OnClickUpdate(index);
            gameLayout.LoadNextRoom(type);
        }

        private void OnEndOfTransition()
        {
            buttonSelection.SetActive(true);
        }

        #endregion

    }
}
