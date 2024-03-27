using System;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwStopWatch : SwTimer
    {
        #region --- Members ---

        private bool _shouldInvokeTick;
        private float _lastTick;
        private float _tickIntervalSeconds;

        #endregion


        #region --- Properties ---

        protected override bool ShouldInvokeTick
        {
            get { return _shouldInvokeTick; }
        }

        #endregion


        #region --- Public Methods ---

        public static SwStopWatch Create(GameObject gameObject, string name = "", bool pauseWhenUnityOutOfFocus = false, float tickIntervalSeconds = 0)
        {
            var instance = gameObject.AddComponent<SwStopWatch>();
            instance.Name = string.IsNullOrEmpty(name) ? instance.Name : name;
            instance.Duration = Mathf.Infinity;
            instance.PauseWhenUnityOutOfFocus = pauseWhenUnityOutOfFocus;
            instance._tickIntervalSeconds = tickIntervalSeconds;

            return instance;
        }

        #endregion


        #region --- Private Methods ---

        protected override void BeforeInvokeTick ()
        {
            // Elapsed progress can be describes as chunks of size _timeInterval
            // We want to know when Elapsed is progressing towards a new chunk and then set _shouldInvokeTick to true so OnTickEvent will be invoked.
            // _lastTick equals to the last elapsed time that the progress of Elapsed passed another time chunk _tickInterval.
            // As long as `Elapsed - _lastTick < _tickInterval` we know that we are in the middle a time chunk.
            // So when `Elapsed - _lastTick >= _tickInterval` it's time to reset _lastTick and set _shouldInvokeTick to true.
            if (Elapsed - _lastTick >= _tickIntervalSeconds)
            {
                _lastTick = Elapsed;
                _shouldInvokeTick = true;
            }
            else
            {
                _shouldInvokeTick = false;
            }
        }

        #endregion
    }
}