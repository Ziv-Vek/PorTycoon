using System;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwDateTimer : ISwStartStopTimer
    {
        #region --- Events ---

        public event Action OnFinishEvent;
        public event Action OnStoppedEvent;
        public event OnTickDelegate OnTickEvent;

        #endregion


        #region --- Members ---

        private readonly Action<DateTime> _onUpdate;
        private readonly DateTime _endDate;
        private readonly float _checkEndDatePassEverySeconds;
        private Coroutine _coroutine;
        private DateTime _startDate;

        #endregion


        #region --- Properties ---

        public bool DidFinish
        {
            get { return _coroutine == null; }
        }

        public bool IsPaused
        {
            get { return false; }
        }

        public bool IsReset
        {
            get { return _coroutine == null; }
        }

        public bool IsEnabled
        {
            get { return _coroutine != null; }
        }

        public float Duration
        {
            get { return (_endDate - DateTime.UtcNow).Seconds; }
        }

        public float Elapsed
        {
            get { return (DateTime.UtcNow - _startDate).Seconds; }
        }

        #endregion


        #region --- Construction ---

        public SwDateTimer(DateTime utcEndDate, float checkEndDatePassEverySeconds)
        {
            _endDate = utcEndDate;
            _checkEndDatePassEverySeconds = checkEndDatePassEverySeconds;
        }

        #endregion


        #region --- Public Methods ---

        public ISwStartStopTimer StartTimer()
        {
            _startDate = DateTime.UtcNow.Date;
            _coroutine = SwInfra.CoroutineService.RunActionEndlessly(CheckAndUpdateIfEndDatePassed, _checkEndDatePassEverySeconds, () => false);

            return this;
        }

        public ISwStartStopTimer StopTimer()
        {
            SwInfra.CoroutineService.StopCoroutine(_coroutine);
            _coroutine = null;

            return this;
        }

        #endregion


        #region --- Private Methods ---

        private void CheckAndUpdateIfEndDatePassed()
        {
            if (DateTime.UtcNow.Date >= _endDate)
            {
                StopTimer();
                OnFinishEvent?.Invoke();
            }

            OnTickEvent?.Invoke(Elapsed, Duration);
        }

        #endregion
    }
}