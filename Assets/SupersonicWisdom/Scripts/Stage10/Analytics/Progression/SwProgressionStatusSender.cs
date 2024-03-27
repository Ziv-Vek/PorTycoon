#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal sealed class SwProgressionStatusSender : SwBaseAnalyticsManager, ISwGameProgressionListener
    {
        #region --- Constants ---

        private const string CUSTOM6 = "custom6";
        private const string PLAYTIME_KEY = "playtime";
        private const string LEVEL_TYPE_KEY = "levelType";
        private const string LEVEL_NUMBER_KEY = "levelNumber";
        private const string LEVEL_CUSTOM_STRING_KEY = CUSTOM6;
        private const string GAMEPLAY_TYPE_KEY = "gameplayType";
        private const string LEVEL_REVIVES_KEY = "levelRevives";
        private const string LEVEL_ATTEMPTS_KEY = "levelAttempts";
        private const string PREVIOUS_LEVEL_TYPE_KEY = "previousLevelType";
        private const string GAMEPLAY_PROGRESS_TYPE_KEY = "gameplayProgressType";
        private const string PREVIOUS_LEVEL_TYPE_NUMBER_KEY = "previousLevelTypeNumber";

        #endregion


        #region --- Members ---

        private readonly SwCoreUserData _coreUserData;
        private readonly SwTimerManager _timerManager;
        private readonly SwEventDataMeasurementProvider _measurementProvider;

        #endregion


        #region --- Properties ---

        private float PlaytimeElapsed
        {
            get { return _timerManager.CurrentSessionPlaytimeElapsed; }
        }

        #endregion


        #region --- Construction ---

        public SwProgressionStatusSender(SwCoreFpsMeasurementManager fpsMeasurementManager, ISwTracker tracker, SwTimerManager timerManager, SwCoreUserData coreUserData) : base(tracker)
        {
            _timerManager = timerManager;
            _coreUserData = coreUserData;

            _measurementProvider = new SwEventDataMeasurementProvider(fpsMeasurementManager);
        }

        #endregion


        #region --- Public Methods ---

        public void OnTimeBasedGameStarted()
        {
            //The progression events are duplicated to keep backwards compatibility with the old events (new event structure was inserted in v7.4)
            const SwProgressEvent progressEvent = SwProgressEvent.TimeBasedGameStart;
            var eventCustoms = SwCoreTracker.GenerateEventCustoms(progressEvent.ToString());
            eventCustoms.SwAddOrReplace(GAMEPLAY_PROGRESS_TYPE_KEY, progressEvent);
            
            // The fps measurement never started so no need to START or STOP
            _tracker.TrackGameProgressEvent(eventCustoms);
        }

        public void OnLevelCompleted(ESwLevelType levelType, long level, string levelName, long attempts, long revives)
        {
            OnLevelApiCalled(SwProgressEvent.LevelCompleted, levelType, level, levelName, attempts, revives);
        }

        public void OnLevelFailed(ESwLevelType levelType, long level, string levelName, long attempts, long revives)
        {
            OnLevelApiCalled(SwProgressEvent.LevelFailed, levelType, level, levelName, attempts, revives);
        }

        public void OnLevelRevived(ESwLevelType levelType, long level, string levelName, long attempts, long revives)
        {
            OnLevelApiCalled(SwProgressEvent.LevelRevived, levelType, level, levelName, attempts, revives);
        }

        public void OnLevelSkipped(ESwLevelType levelType, long level, string levelName, long attempts, long revives)
        {
            OnLevelApiCalled(SwProgressEvent.LevelSkipped, levelType, level, levelName, attempts, revives);
        }

        public void OnLevelStarted(ESwLevelType levelType, long level, string levelName, long attempts, long revives)
        {
            OnLevelApiCalled(SwProgressEvent.LevelStarted, levelType, level, levelName, attempts, revives);
        }

        public void OnMetaStarted(string customString)
        {
            OnMetaApiCalled(customString, SwProgressEvent.MetaStarted);
        }

        public void OnMetaEnded(string customString)
        {
            OnMetaApiCalled(customString, SwProgressEvent.MetaEnded);
        }

        #endregion


        #region --- Private Methods ---

        private static void AddLegacyLevelCustoms(SwProgressEvent progressEvent, long levelNum, string customString, long attempts, long revives, Dictionary<string, object> gameProgressDictionary)
        {
            //The progression events are duplicated to keep backwards compatibility with the old events (new event structure was inserted in v7.4)
            var eventCustoms = SwCoreTracker.GenerateEventCustoms($"{progressEvent}", $"{levelNum}", $"{attempts}", "", $"{revives}", $"{customString}");

            gameProgressDictionary.SwMerge(true, eventCustoms);
        }

        private static Dictionary<string, object> CreateLevelProgressionEventDict(SwProgressEvent progressEvent, ESwLevelType levelType, long levelNum, string customString, long attempts, long revives, SwUserState userState)
        {
            var gameProgressDictionary = new Dictionary<string, object>
            {
                [GAMEPLAY_TYPE_KEY] = ESwGameplayType.Level,
                [GAMEPLAY_PROGRESS_TYPE_KEY] = progressEvent,
                [LEVEL_CUSTOM_STRING_KEY] = customString,
                [PREVIOUS_LEVEL_TYPE_KEY] = userState.previousLevelType,
                [PREVIOUS_LEVEL_TYPE_NUMBER_KEY] = userState.previousLevelTypeNumber,
                [LEVEL_TYPE_KEY] = levelType,
                [LEVEL_NUMBER_KEY] = levelNum,
                [LEVEL_ATTEMPTS_KEY] = attempts,
                [LEVEL_REVIVES_KEY] = revives,
            };

            return gameProgressDictionary;
        }

        private void OnLevelApiCalled(SwProgressEvent progressEvent, ESwLevelType levelType, long levelNum, string customString, long attempts, long revives = 0)
        {
            TrackLevelProgressEvent(progressEvent, levelType, levelNum, customString, attempts, revives);
        }

        private void OnMetaApiCalled(string customString, SwProgressEvent progressEvent)
        {
            TrackMetaProgressionEvent(customString, progressEvent);
        }

        private void TrackMetaProgressionEvent(string customString, SwProgressEvent progressEvent)
        {
            var userState = _coreUserData.ImmutableUserState();

            var metaEventDict = new Dictionary<string, object>();
            metaEventDict.Clear();
            metaEventDict[GAMEPLAY_TYPE_KEY] = ESwGameplayType.Meta;
            metaEventDict[GAMEPLAY_PROGRESS_TYPE_KEY] = progressEvent;
            metaEventDict[LEVEL_CUSTOM_STRING_KEY] = customString;
            metaEventDict[PREVIOUS_LEVEL_TYPE_KEY] = userState.previousLevelType;
            metaEventDict[PREVIOUS_LEVEL_TYPE_NUMBER_KEY] = userState.previousLevelTypeNumber;

            AddFpsMeasurementToEventDict(metaEventDict);

            _tracker.TrackGameProgressEvent(metaEventDict);
        }

        private void TrackLevelProgressEvent(SwProgressEvent progressEvent, ESwLevelType levelType, long levelNum, string customString, long attempts, long revives = 0)
        {
            var userState = _coreUserData.ImmutableUserState();

            var gameProgressDictionary = CreateLevelProgressionEventDict(progressEvent, levelType, levelNum, customString, attempts, revives, userState);

            AddLegacyLevelCustoms(progressEvent, levelNum, customString, attempts, revives, gameProgressDictionary);
            AddFpsMeasurementToEventDict(gameProgressDictionary);

            _tracker?.TrackEventWithParams(SwCoreTracker.PROGRESS_EVENT_TYPE, gameProgressDictionary);

            //Save the previous level type and level number for the next level
            ModifyUserState(levelType, levelNum);
        }

        private void AddFpsMeasurementToEventDict(Dictionary<string, object> gameProgressDictionary)
        {
            gameProgressDictionary.SwMerge(true, _measurementProvider.GetDataAndRestart());
        }

        private void ModifyUserState(ESwLevelType levelType, long levelNum)
        {
            _coreUserData.ModifyUserStateSync(state =>
            {
                state.previousLevelType = levelType;
                state.previousLevelTypeNumber = levelNum;
            });
        }

        #endregion
    }
}
#endif