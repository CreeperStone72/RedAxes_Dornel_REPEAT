using System;
using UnityEngine;
using UnityEngine.Events;

namespace Norsevar
{
    public class TimeTickSystem : Singleton<TimeTickSystem>
    {

        #region Constants and Statics

        private const float MAX_TICK_INTERVAL = 0.1f;

        #endregion

        #region Private Fields

        private readonly UnityEvent _onTickResolution1 = new();
        private readonly UnityEvent _onTickResolution2 = new();
        private readonly UnityEvent _onTickResolution4 = new();
        private readonly UnityEvent _onTickResolution8 = new();

        private float _tickTimer;
        private float _tickTimerOne;
        private float _tickTimerTwo;
        private float _tickTimerFour;
        private float _tickTimerEight;

        #endregion

        #region Properties

        private uint Tick { get; set; }

        #endregion

        #region Unity Methods

        private void Update()
        {
            _tickTimer += Time.deltaTime;
            if (!(_tickTimer >= MAX_TICK_INTERVAL))
                return;
            _tickTimer -= MAX_TICK_INTERVAL;

            Tick++;

            _onTickResolution1?.Invoke();

            if (Tick % (int)TickRateMultiplierType.Two != 0)
                return;
            _onTickResolution2?.Invoke();

            if (Tick % (int)TickRateMultiplierType.Four != 0)
                return;
            _onTickResolution4?.Invoke();

            if (Tick % (int)TickRateMultiplierType.Eight == 0)
                _onTickResolution8?.Invoke();
        }

        #endregion

        #region Protected Methods

        protected override void OnAwake()
        {
        }

        #endregion

        #region Public Methods

        public void RegisterListener(TickRateMultiplierType pTickRateType, UnityAction pListener)
        {
            switch (pTickRateType)
            {
                case TickRateMultiplierType.One:
                    _onTickResolution1.AddListener(pListener);
                    break;

                case TickRateMultiplierType.Two:
                    _onTickResolution2.AddListener(pListener);
                    break;

                case TickRateMultiplierType.Four:
                    _onTickResolution4.AddListener(pListener);
                    break;

                case TickRateMultiplierType.Eight:
                    _onTickResolution8.AddListener(pListener);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pTickRateType), pTickRateType, null);
            }
        }

        public void UnregisterListener(TickRateMultiplierType pTickRateType, UnityAction pListener)
        {
            switch (pTickRateType)
            {
                case TickRateMultiplierType.One:
                    _onTickResolution1.RemoveListener(pListener);
                    break;

                case TickRateMultiplierType.Two:
                    _onTickResolution2.RemoveListener(pListener);
                    break;

                case TickRateMultiplierType.Four:
                    _onTickResolution4.RemoveListener(pListener);
                    break;

                case TickRateMultiplierType.Eight:
                    _onTickResolution8.RemoveListener(pListener);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pTickRateType), pTickRateType, null);
            }
        }

        #endregion

        public enum TickRateMultiplierType : sbyte
        {
            One = 1,
            Two = 2,
            Four = 4,
            Eight = 8
        }
    }
}
