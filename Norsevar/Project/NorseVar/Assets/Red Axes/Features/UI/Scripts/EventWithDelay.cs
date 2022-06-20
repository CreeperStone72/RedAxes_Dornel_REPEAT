using UnityEngine;
using UnityEngine.Events;

namespace Norsevar.UI
{
    public class EventWithDelay : MonoBehaviour
    {
        [SerializeField] private int seconds = 8;
        [SerializeField] private UnityEvent @event;

        private void Start()
        {
            this.ExecuteInSeconds(() => @event?.Invoke(), seconds);
        }

    }
}
