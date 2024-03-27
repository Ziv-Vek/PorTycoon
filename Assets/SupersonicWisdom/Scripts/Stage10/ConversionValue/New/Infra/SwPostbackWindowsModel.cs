#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwPostbackWindowsModel
    {
        #region --- Events ---
        
        public event Action WindowLockedEvent;
        
        #endregion
        
        
        #region --- Constants ---

        private const int WINDOW_ZERO_INTERVAL_IN_DAYS = 2;
        private const int WINDOW_ONE_INTERVAL_IN_DAYS = 5;
        private const int WINDOW_TWO_INTERVAL_IN_DAYS = 28;

        private const string POSTBACK_WINDOWS_DATE_TIMES = "swPostbackWindowDateTimes";
        private const string WINDOW_LOCKED_LOG_FORMAT = "Window {0} lcoked!\nStart time - {1}\nEnd time - {2}";

        #endregion


        #region --- Members ---

        private static readonly DateTime NO_LOCK_TIME_VALUE = DateTime.MaxValue;

        [NonSerialized] private DateTime zeroConfiguredLockTime = NO_LOCK_TIME_VALUE;
        [NonSerialized] private DateTime oneConfiguredLockTime = NO_LOCK_TIME_VALUE;
        [NonSerialized] private DateTime twoConfiguredLockTime = NO_LOCK_TIME_VALUE;

        [JsonProperty] private PostbackWindow _windowZero;
        [JsonProperty] private PostbackWindow _windowOne;
        [JsonProperty] private PostbackWindow _windowTwo;
        [JsonProperty] private PostbackWindow _windowDone;

        #endregion


        #region --- Properties ---

        public DateTime CurrentLockTime { get; protected set; }

        public DateTime CurrentStartTime
        {
            get
            {
                return CurrentWindow.startTime;
            }
        }

        public DateTime WindowEndTime
        {
            get { return CurrentWindow.endTime; }
        }

        public EPostbackWindow CurrentWindowType
        {
            get { return CurrentWindow.type; }
        }

        public PostbackWindow CurrentWindow { get; set; }

        internal bool PassLockTime
        {
            get { return CurrentLockTime < DateTime.UtcNow; }
        }

        #endregion


        #region --- Mono Override ---

        public void Start(DateTime installDate)
        {
            InitWindows(installDate);
            Calculate(DateTime.UtcNow);
            SaveToPlayerPrefs();
        }

        #endregion


        #region --- Public Methods ---

        public static SwPostbackWindowsModel Load()
        {
            return SwInfra.KeyValueStore.GetGenericSerializedData<SwPostbackWindowsModel>(POSTBACK_WINDOWS_DATE_TIMES, null);
        }

        public void Setup(Dictionary<EPostbackWindow, float> lockFromTimeDict, Action OnWindowLocked)
        {
            WindowLockedEvent += OnWindowLocked;
            
            UpdateConfiguredLockTimes(lockFromTimeDict);
            Calculate(DateTime.UtcNow);
        }

        public void Calculate(DateTime nowDateTime)
        {
            if (nowDateTime < _windowZero.endTime)
            {
                CurrentWindow = _windowZero;
                CurrentLockTime = zeroConfiguredLockTime < CurrentWindow.endTime ? zeroConfiguredLockTime : CurrentWindow.endTime;
            }
            else if (nowDateTime < _windowOne.endTime)
            {
                CurrentWindow = _windowOne;
                CurrentLockTime = oneConfiguredLockTime < CurrentWindow.endTime ? oneConfiguredLockTime : CurrentWindow.endTime;
            }
            else if (nowDateTime < _windowTwo.endTime)
            {
                CurrentWindow = _windowTwo;
                CurrentLockTime = twoConfiguredLockTime < CurrentWindow.endTime ? twoConfiguredLockTime : CurrentWindow.endTime;
            }
            else
            {
                CurrentWindow = _windowDone;
                CurrentLockTime = NO_LOCK_TIME_VALUE;
            }

            if (nowDateTime >= CurrentLockTime && !CurrentWindow.isLocked)
            {
                WindowLockedEvent?.Invoke();
            }

            SwInfra.Logger.Log(EWisdomLogType.ConversionValue, $"Current date time: {nowDateTime.SwToString()}");
            SwInfra.Logger.Log(EWisdomLogType.ConversionValue, $"CurrentWindowType: {CurrentWindowType}\nCurrentStartTime: {CurrentStartTime.SwToString()}\nCurrentLockTime: {CurrentLockTime.SwToString()}\nWindowEndTime: {WindowEndTime.SwToString()}");
        }

        public bool IsLocked(EPostbackWindow currentWindowType)
        {
            switch (currentWindowType)
            {
                case EPostbackWindow.Zero:
                    return _windowZero.isLocked;
                case EPostbackWindow.One:
                    return _windowOne.isLocked;
                case EPostbackWindow.Two:
                    return _windowTwo.isLocked;
            }

            return false;
        }

        public void SetLock(EPostbackWindow currentWindowType)
        {
            SwInfra.Logger.Log(EWisdomLogType.ConversionValue, WINDOW_LOCKED_LOG_FORMAT.Format(CurrentWindowType, CurrentStartTime, CurrentLockTime));
            
            switch (currentWindowType)
            {
                case EPostbackWindow.Zero:
                    _windowZero.isLocked = true;

                    break;
                case EPostbackWindow.One:
                    _windowOne.isLocked = true;

                    break;
                case EPostbackWindow.Two:
                    _windowTwo.isLocked = true;

                    break;
            }
            
            SaveToPlayerPrefs();
        }

        #endregion


        #region --- Private Methods ---

        private void InitWindows(DateTime installDate)
        {
            var zeroEndTime = installDate.AddDays(WINDOW_ZERO_INTERVAL_IN_DAYS);
            var oneEndTime = zeroEndTime.AddDays(WINDOW_ONE_INTERVAL_IN_DAYS);
            var twoEndTime = oneEndTime.AddDays(WINDOW_TWO_INTERVAL_IN_DAYS);

            _windowZero = new PostbackWindow
            {
                type = EPostbackWindow.Zero,
                startTime = installDate,
                isLocked = false,
                endTime = zeroEndTime,
            };

            _windowOne = new PostbackWindow
            {
                type = EPostbackWindow.One,
                startTime = zeroEndTime,
                isLocked = false,
                endTime = oneEndTime,
            };

            _windowTwo = new PostbackWindow
            {
                type = EPostbackWindow.Two,
                startTime = oneEndTime,
                isLocked = false,
                endTime = twoEndTime,
            };

            _windowDone = new PostbackWindow
            {
                type = EPostbackWindow.Done,
                startTime = twoEndTime,
                isLocked = false,
                endTime = DateTime.MaxValue,
            };
        }

        private void SaveToPlayerPrefs()
        {
            SwInfra.KeyValueStore.SetGenericSerializedData(POSTBACK_WINDOWS_DATE_TIMES, this);
        }

        private void UpdateConfiguredLockTimes(Dictionary<EPostbackWindow, float> lockFromTimeDict)
        {
            if (lockFromTimeDict.ContainsKey(EPostbackWindow.Zero))
            {
                zeroConfiguredLockTime = _windowZero.startTime.AddHours(lockFromTimeDict[EPostbackWindow.Zero]);
            }
            
            if (lockFromTimeDict.ContainsKey(EPostbackWindow.One))
            {
                oneConfiguredLockTime = _windowOne.startTime.AddHours(lockFromTimeDict[EPostbackWindow.One]);
            }
            
            if (lockFromTimeDict.ContainsKey(EPostbackWindow.Two))
            {
                twoConfiguredLockTime = _windowTwo.startTime.AddHours(lockFromTimeDict[EPostbackWindow.Two]);
            }
        }

        #endregion
    }
}
#endif