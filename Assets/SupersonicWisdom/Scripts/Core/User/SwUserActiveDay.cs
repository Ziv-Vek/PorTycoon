using System;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwUserActiveDay : ISwTrackerDataProvider, ISwScriptLifecycleListener
    {
        #region --- Constants ---

        private const float CHECK_DAY_PASS_EVERY_SECONDS = 60;
        private const string ACTIVE_DAY_WISDOM_ANALYTICS_KEY = "activeDay";

        #endregion


        #region --- Events ---

        internal delegate void UpdateUserData(SwUserStateChangeReason reason, bool silent = false);

        #endregion


        #region --- Members ---

        internal UpdateUserData OnActiveDayUpdated;
        private DateTime _lastActiveDate;

        private SwDateTimer _dateTimer;

        #endregion


        #region --- Properties ---

        public int ActiveDay { get; private set; }

        #endregion


        #region --- Construction ---

        public void Load()
        {
            var nowDate = DateTime.UtcNow.Date;
            
            LoadActiveDay(nowDate);
            TryAddActiveDay(nowDate);
            StartTimer(nowDate);
        }

        private void LoadActiveDay(DateTime defaultValue)
        {
            if (!SwInfra.KeyValueStore.HasKey(SwStoreKeys.ActiveDay))
            {
                SwInfra.Logger.Log(EWisdomLogType.ActiveDay, $"No Last active day in prefs - using now! = {defaultValue}");
                UpdateAndSaveActiveDay(defaultValue);
            }
            else
            {
                ActiveDay = SwInfra.KeyValueStore.GetInt(SwStoreKeys.ActiveDay, 1);
                _lastActiveDate = SwInfra.KeyValueStore.GetGenericSerializedData(SwStoreKeys.LastActiveDate, defaultValue);
        
                SwInfra.Logger.Log(EWisdomLogType.ActiveDay, $"Active day = {ActiveDay}");
                SwInfra.Logger.Log(EWisdomLogType.ActiveDay, $"Last active day = {_lastActiveDate}");
            }
        }

        public (SwJsonDictionary, IEnumerable<string>) ConditionallyAddExtraDataToTrackEvent(SwCoreUserData coreUserData)
        {
            var shouldTrackPastMinimumSdkVersion = SwTrackerConstants.ShouldTrackPastMinimumSdkVersion(coreUserData);
            
            if (!shouldTrackPastMinimumSdkVersion)
            {
                SwInfra.Logger.Log(EWisdomLogType.ActiveDay, $"Not tracking ActiveDay as SDK version is below minimum required");
                return (new SwJsonDictionary(), KeysToEncrypt());
            }
            
            var dataDictionary = new SwJsonDictionary
            {
                { ACTIVE_DAY_WISDOM_ANALYTICS_KEY, ActiveDay },
            };

            return (dataDictionary, KeysToEncrypt());

            IEnumerable<string> KeysToEncrypt()
            {
                yield break; 
            }
        }
        
        public void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus) return;
            
            TryAddActiveDay();
        }

        public void OnApplicationQuit() { }

        public void OnAwake() { }

        public void OnStart() { }

        public void OnUpdate() { }

        #endregion


        #region --- Private Methods ---

        private void UpdateAndSaveActiveDay(DateTime newActiveDayDate)
        {
            ActiveDay++;
            _lastActiveDate = newActiveDayDate;

            SwInfra.KeyValueStore.SetInt(SwStoreKeys.ActiveDay, ActiveDay);
            SwInfra.KeyValueStore.SetGenericSerializedData(SwStoreKeys.LastActiveDate, _lastActiveDate).Save();
            
            SwInfra.Logger.Log(EWisdomLogType.ActiveDay, $"Active day updated = {ActiveDay}");
            SwInfra.Logger.Log(EWisdomLogType.ActiveDay, $"Last active day updated = {newActiveDayDate}");
        }
        
        private bool TryAddActiveDay(DateTime newActiveDayDateToCheck)
        {
            SwInfra.Logger.Log(EWisdomLogType.ActiveDay, $"Try Add day | previousSavedDate = {_lastActiveDate} | newActiveDayDateToCheck = {newActiveDayDateToCheck}");
            
            // We have to check _lastActiveDate for MinValue as the Android OnPause event get triggered at the very beginning, and before LoadActiveDay
            if (newActiveDayDateToCheck <= _lastActiveDate || _lastActiveDate == DateTime.MinValue) return false;

            UpdateAndSaveActiveDay(newActiveDayDateToCheck);

            return true;
        }

        private void ResetTimer(DateTime nowDate)
        {
            _dateTimer?.StopTimer();
            StartTimer(nowDate);
        }
        
        private void StartTimer(DateTime nowDate)
        {
            _dateTimer = new SwDateTimer(nowDate.AddDays(1), CHECK_DAY_PASS_EVERY_SECONDS);
            _dateTimer.OnFinishEvent += TryAddActiveDay;
            _dateTimer.StartTimer();
        }

        #endregion


        #region --- Event Handler ---

        private void TryAddActiveDay()
        {
            var nowDate = DateTime.UtcNow.Date;
            
            if (TryAddActiveDay(nowDate))
            {
                ResetTimer(nowDate);
            }
        }

        #endregion
    }
}