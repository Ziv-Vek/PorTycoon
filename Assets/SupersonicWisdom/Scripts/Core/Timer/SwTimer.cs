using System;
using JetBrains.Annotations;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwTimer : MonoBehaviour, ISwTimer
    {
        #region --- Events ---

        public event Action OnFinishEvent;
        public event Action OnStoppedEvent;
        public event OnTickDelegate OnTickEvent;

        #endregion


        #region --- Members ---

        private bool _isNextUpdateAfterAppResume;
        private bool _isPaused;
        private bool _wasPaused;

        #endregion


        #region --- Properties ---

        public bool IsDisabled
        {
            get { return Duration < 0; }
        }
        
        public bool IsReset
        {
            get { return Duration > 0 && !IsEnabled && Elapsed == 0; }
        }

        public bool DidFinish { get; private set; }

        public bool IsPaused
        {
            get
            {
                return _isPaused;
            }
            private set
            {
                if (_isPaused && !value)
                {
                    _wasPaused = true;
                }
                
                _isPaused = value;
            }
        }

        public bool PauseWhenUnityOutOfFocus { get; set; }
        ///  Duration in seconds
        public float Duration { get; set; }
        public float Elapsed { get; set; }

        public string Name { get; set; } = "Anonymous Timer";

        protected virtual bool ShouldInvokeTick
        {
            get { return true; }
        }

        [PublicAPI]
        public bool IsEnabled { get; set; }

        private float DeltaTime
        {
            get
            {
                if (PauseWhenUnityOutOfFocus)
                {
                    // On this case unity has just resumed so for timers with PauseWhenUnityOutOfFocus == true
                    // The delta time should be very close to zero regardless of Time.unscaledDeltaTime value
                    return _isNextUpdateAfterAppResume || _wasPaused ? Mathf.Epsilon : Time.unscaledDeltaTime;
                }

                // For timers with PauseWhenUnityOutOfFocus == false
                // The first update of Elapsed (when Elapsed == 0) should ignore
                // current delta unscaledDeltaTime since it can be very long
                return Elapsed == 0 || _wasPaused ? Mathf.Epsilon : Time.unscaledDeltaTime;
            }
        }

        #endregion


        #region --- Mono Override ---

        protected virtual void Reset ()
        {
            if (IsDisabled) return;
            
            SwInfra.Logger.Log(EWisdomLogType.Time, Name);
            Elapsed = 0;
            DidFinish = false;
            IsPaused = false;
            StopTick();
        }

        protected virtual void Update()
        {
            if (!IsEnabled || IsPaused) return;

            if (Duration <= 0)
            {
                StopTimer();

                return;
            }

            OnProgress();

            if (_isNextUpdateAfterAppResume)
            {
                _isNextUpdateAfterAppResume = false;
            }
        }

        private void Awake ()
        {
            Reset();
        }

        protected virtual void OnApplicationPause(bool didPause)
        {
            _isNextUpdateAfterAppResume = !didPause;
        }

        #endregion


        #region --- Public Methods ---

        public static SwTimer Create(GameObject gameObject, string name = "", float durationSeconds = 0, bool pauseWhenUnityOutOfFocus = false)
        {
            return CreateGeneric<SwTimer>(gameObject, name, durationSeconds, pauseWhenUnityOutOfFocus);
        }

        protected static TSwTimerType CreateGeneric<TSwTimerType>(GameObject gameObject, string name = "", float durationSeconds = 0, bool pauseWhenUnityOutOfFocus = false) where TSwTimerType: SwTimer
        {
            var instance = gameObject.AddComponent<TSwTimerType>();
            instance.Name = string.IsNullOrEmpty(name) ? instance.Name : name;
            instance.Duration = durationSeconds;
            instance.PauseWhenUnityOutOfFocus = pauseWhenUnityOutOfFocus;

            return instance;
        }

        public ISwTimer PauseTimer ()
        {
            if (!IsEnabled || IsPaused) return this;

            SwInfra.Logger.Log(EWisdomLogType.Time, Name);
            Pause();

            return this;
        }

        public ISwTimer ResumeTimer ()
        {
            if (!IsPaused) return this;

            SwInfra.Logger.Log(EWisdomLogType.Time, Name);
            Resume();

            return this;
        }

        public ISwStartStopTimer StartTimer ()
        {
            if (IsDisabled) return this;
            
            SwInfra.Logger.Log(EWisdomLogType.Time, Name);
            Reset();
            IsEnabled = true;

            return this;
        }

        public ISwStartStopTimer StopTimer ()
        {
            if (IsDisabled) return this;
            
            SwInfra.Logger.Log(EWisdomLogType.Time, Name);
            Reset();

            OnStoppedEvent?.Invoke();
            return this;
        }

        public void SetDuration (float duration)
        {
            Duration = duration;
        }

        public void SetElapsed(float elapsed)
        {
            Elapsed = elapsed;
        }

        #endregion


        #region --- Private Methods ---

        protected virtual void BeforeInvokeTick ()
        { }

        protected void Pause ()
        {
            SwInfra.Logger.Log(EWisdomLogType.Time, Name);
            DidFinish = false;
            IsPaused = true;
            StopTick();
        }

        protected void Resume ()
        {
            SwInfra.Logger.Log(EWisdomLogType.Time, Name);
            DidFinish = false;
            IsPaused = false;
            ResumeTick();
        }

        private void OnProgress ()
        {
            var deltaTime = DeltaTime;
            var nextElapsed = Elapsed + deltaTime;

            if (deltaTime > 0.5f)
            {
                SwInfra.Logger.Log(EWisdomLogType.Time, $"Elapsed substantial addition | Elapsed={Elapsed} | deltaTime={deltaTime}");
            }

            if (nextElapsed >= Duration)
            {
                // Elapsed should never be greater than Duration.
                // So, it is being set exactly to the duration time right before finish event.
                // After finish event Elapsed == Duration is true
                Elapsed = Duration;

                DidFinish = true;
                IsEnabled = false;
                OnFinishEvent?.Invoke();
                SwInfra.Logger.Log(EWisdomLogType.Time, Name);
            }
            else
            {
                Elapsed = nextElapsed;
                BeforeInvokeTick();

                if (ShouldInvokeTick)
                {
                    OnTickEvent?.Invoke(Elapsed, Duration - Elapsed);
                }
            }
            
            _wasPaused = false;
        }

        private void ResumeTick ()
        {
            SwInfra.Logger.Log(EWisdomLogType.Time, Name);
            IsEnabled = true;
        }

        private void StopTick ()
        {
            if (!IsEnabled) return;
            
            SwInfra.Logger.Log(EWisdomLogType.Time, Name);
            IsEnabled = false;
        }

        #endregion
    }
}