using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwTimerManager : ISwReadyEventListener, ISwTrackerDataProvider, ISwScriptLifecycleListener, ISwSessionListener
    {
        #region --- Constants ---

        private const int PLAYTIME_TICK_INTERVAL = 1;
        private const int SAVE_PLAYTIME_INTERVAL = 5;
        private const string ACCUMULATED_SESSIONS_PLAYTIME = "AccumulatedSessionsPlaytime";
        
        private const string MEGA_NETO_PLAYTIME_KEY = "megaNetoPlaytime";
        private const string TOTAL_NETO_PLAYTIME_KEY = "totalNetoPlaytime";
        private const string MEGA_PLAYTIME_KEY = "megaPlaytime";

        #endregion


        #region --- Members ---

        protected readonly ISwTimer _currentSessionPlaytimeNetoStopWatch;
        private readonly ISwTimer _currentSessionPlaytimeStopWatch;
        private readonly float _previousSessionsPlaytimeInSeconds;
        private readonly ISwMonoBehaviour _mono;

        #endregion


        #region --- Properties ---

        public ISwTimerListener CurrentSessionPlaytimeNetoStopWatch
        {
            get { return _currentSessionPlaytimeNetoStopWatch; }
        }

        public float CurrentSessionPlaytimeElapsed
        {
            get { return CurrentSessionPlaytimeStopWatch?.Elapsed ?? -1f; }
        }

        public float AllSessionsPlaytimeNeto
        {
            get { return _previousSessionsPlaytimeInSeconds + CurrentSessionPlaytimeNetoStopWatch?.Elapsed ?? 0; }
        }

        protected internal float CurrentSessionPlaytimeNeto => CurrentSessionPlaytimeNetoStopWatch?.Elapsed ?? -1f;

        internal ISwTimerListener CurrentSessionPlaytimeStopWatch
        {
            get { return _currentSessionPlaytimeStopWatch; }
        }

        protected float CurrentSessionPlaytime => CurrentSessionPlaytimeStopWatch?.Elapsed ?? -1f;

        #endregion


        #region --- Construction ---

        public SwTimerManager(ISwMonoBehaviour mono)
        {
            _mono = mono;
            _currentSessionPlaytimeNetoStopWatch = SwStopWatch.Create(mono.GameObject, $"{ETimers.CurrentSessionNetoPlaytimeMinutes}", true, PLAYTIME_TICK_INTERVAL);
            _currentSessionPlaytimeStopWatch = SwStopWatch.Create(mono.GameObject, $"{ETimers.CurrentSessionPlaytimeMinutes}", false, PLAYTIME_TICK_INTERVAL);
            float.TryParse(SwInfra.KeyValueStore.GetString(ACCUMULATED_SESSIONS_PLAYTIME, "0"), NumberStyles.Float, CultureInfo.InvariantCulture, out _previousSessionsPlaytimeInSeconds);
            SwInfra.Logger.Log(EWisdomLogType.Test, "Previous sessions playtime: " + _previousSessionsPlaytimeInSeconds);
            _mono.ApplicationFocusEvent += OnApplicationFocus;
            _mono.ApplicationPausedEvent += OnApplicationPaused;
        }

        ~SwTimerManager()
        {
            _mono.ApplicationFocusEvent -= OnApplicationFocus;
            _mono.ApplicationPausedEvent -= OnApplicationPaused;
        }

        (SwJsonDictionary, IEnumerable<string>) ISwTrackerDataProvider.AddExtraDataToTrackEvent()
        {
            SwInfra.Logger.Log(EWisdomLogType.Test, CurrentSessionPlaytime.SwToString());
            SwInfra.Logger.Log(EWisdomLogType.Test, CurrentSessionPlaytimeNeto.SwToString());
            
            var extraData = new SwJsonDictionary
            {
                { MEGA_NETO_PLAYTIME_KEY, (int)Mathf.Round(CurrentSessionPlaytimeNeto) },
                { TOTAL_NETO_PLAYTIME_KEY, (int)Mathf.Round(AllSessionsPlaytimeNeto) },
                { MEGA_PLAYTIME_KEY, (int)Mathf.Round(CurrentSessionPlaytime) },
            };

            return (extraData, KeysToEncrypt());

            IEnumerable<string> KeysToEncrypt()
            {
                yield break; 
            }
        }

        #endregion


        #region --- Public Methods ---

        public void OnSwReady()
        {
            _currentSessionPlaytimeStopWatch.StartTimer();
            _currentSessionPlaytimeNetoStopWatch.StartTimer();
            
            SavePlaytimeRepeating();
        }
        
        public virtual void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                _currentSessionPlaytimeStopWatch.PauseTimer();
            }
            else
            {
                _currentSessionPlaytimeStopWatch.ResumeTimer();
            }
        }

        public void OnApplicationQuit() { }

        public void OnAwake() { }

        public void OnStart() { }

        public void OnUpdate() { }

        #endregion


        #region --- Private Methods ---
        
        private void SavePlaytimeRepeating()
        {
            SwInfra.CoroutineService.RunActionEndlessly(SavePlaytime, SAVE_PLAYTIME_INTERVAL, () => false);
        }

        private void SavePlaytime()
        {
            // F symbolizes float format - #.## 
            SwInfra.KeyValueStore.SetString(ACCUMULATED_SESSIONS_PLAYTIME, AllSessionsPlaytimeNeto.ToString("F", CultureInfo.InvariantCulture), save: true);
        }

        #endregion


        #region --- Event Handler ---
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus) return;

            SavePlaytime();
        }

        private void OnApplicationPaused(bool isPaused)
        {
            // On iOS, the OnApplicationFocus event is not triggered, so we need to save the playtime here as well
#if UNITY_IOS
            if(!isPaused) return;
            
            SavePlaytime();
#endif
        }
        
        public void OnSessionEnded(string sessionId)
        {
            // Ending a session in Android sometimes doesn't trigger the OnApplicationPaused event, so we need to save the playtime here as well
            SavePlaytime();
        }

        public void OnSessionStarted(string sessionId) { }

        #endregion
    }

    internal enum ETimers
    {
        CurrentSessionPlaytimeMinutes,
        CurrentSessionNetoPlaytimeMinutes,
    }
}