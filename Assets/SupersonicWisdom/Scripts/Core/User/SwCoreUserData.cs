using System;
using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwCoreUserData : ISwUserData, ISwTrackerDataProvider
    {
        #region --- Constants ---

        private const string USER_STATE_STORAGE_KEY = "SupersonicWisdomUserState";
        private const string MAIN_LEVEL_WISDOM_ANALYTICS_KEY = "mainLevel";

        #endregion


        #region --- Events ---

        internal delegate void OnUserStateChange(SwUserState newState, SwUserStateChangeReason reason);

        internal event OnUserStateChange OnUserStateChangeEvent;

        #endregion


        #region --- Members ---

        public readonly string BundleIdentifier;

        public readonly string Language;
        public readonly string Platform;
        public string Country;

        protected readonly ISwSettings Settings;
        private readonly ISwAdvertisingIdsGetter _idsGetter;
        private long _installDateInSeconds;
        private string _customUuid;

        private string _installDateTimeString;

        private SwUserState _userState;
        private bool _completeInitOnce;

        private SwUserActiveDay _activeDay;
        private string _installSdkVersion;

        #endregion


        #region --- Properties ---

        public Tuple<EConfigListenerType, EConfigListenerType> ListenerType
        {
            get { return new Tuple<EConfigListenerType, EConfigListenerType>(EConfigListenerType.FinishWaitingForRemote, EConfigListenerType.GameStarted); }
        }

        /// <summary>
        ///     Indicates if this is a new running app session (not the user's session that affected by foreground / background
        ///     changes).
        /// </summary>
        public bool IsNew { private set; get; }
        
        public DateTime InstallDateTime { private set; get; }
        
        public long InstallDateInSeconds
        {
            get { return _installDateInSeconds; }
            private set
            {
                _installDateInSeconds = value;
                SwInfra.KeyValueStore.SetString(SwStoreKeys.InstallTime, _installDateInSeconds.ToString());
                SwInfra.KeyValueStore.Save();
            }
        }

        public string OrganizationAdvertisingId { get; private set; }

        public string Uuid { get; private set; }

        public string CustomUuid
        {
            get { return _customUuid; }
            private set
            {
                _customUuid = value;
                SwInfra.KeyValueStore.SetString(SwStoreKeys.CustomUuid, _customUuid);
                SwInfra.KeyValueStore.Save();
            }
        }

        public string InstallSdkVersion
        {
            get { return _installSdkVersion; }
            private set
            {
                _installSdkVersion = value;
                SwInfra.KeyValueStore.SetString(SwStoreKeys.InstallSdkVersion, _installSdkVersion);
                SwInfra.KeyValueStore.Save();
            }
        }
        
        public long InstallSdkVersionId { get; private set; }
        
        public string InstallDate
        {
            get
            {
                return InstallDateTime.SwToString(SwConstants.INSTALL_DATE_FORMAT);
            }
        }

        public string InstallDateTimeString
        {
            get { return _installDateTimeString; }
            private set
            {
                _installDateTimeString = value;
                SwInfra.KeyValueStore.SetString(SwStoreKeys.InstallDate, _installDateTimeString);
                SwInfra.KeyValueStore.Save();
            }
        }
        
        #endregion


        #region --- Construction ---

        public SwCoreUserData(ISwSettings settings, ISwAdvertisingIdsGetter idsGetter, SwUserActiveDay activeDay)
        {
            Settings = settings;
            _idsGetter = idsGetter;
            _activeDay = activeDay;

            Language = SwUtils.LangAndCountry.GetSystemLanguageIso6391();
            Country = SwUtils.LangAndCountry.GetCountry();
            BundleIdentifier = Application.identifier;
            Platform = Application.platform.ToString();
        }

        #endregion


        #region --- Public Methods ---

        public virtual void Load(ISwInitParams initParams)
        {
            LoadUuid();
            LoadInstallDate();
            LoadActiveDays();
            LoadCustomUuid();
            LoadUserState();
            LoadInstallSdkVersion();
        }

        public long GetSecondsFromInstall()
        {
            if (InstallDateInSeconds == 0)
            {
                return -1;
            }

            return SwUtils.DateAndTime.GetTotalSeconds(DateTime.UtcNow) - InstallDateInSeconds;
        }

        public void LoadUuid()
        {
            var advertisingId = _idsGetter.GetAdvertisingId();
            var organizationAdvertisingId = _idsGetter.GetOrganizationAdvertisingId();

            SwInfra.Logger.Log(EWisdomLogType.User, $"Got advertising ID ('{advertisingId}') and organization advertising ID ('{organizationAdvertisingId}')");

            Uuid = advertisingId;
            OrganizationAdvertisingId = organizationAdvertisingId;
        }

        public bool ModifyUserStateSync(Action<SwUserState> modifier)
        {
            return ModifyUserStateSync(s =>
            {
                modifier.Invoke(s);

                return true;
            });
        }

        public bool ModifyUserStateSync(Func<SwUserState, bool> modifier)
        {
            var copyOfUserState = _userState.Copy();
            var didChange = modifier.Invoke(copyOfUserState);

            if (didChange)
            {
                _userState = copyOfUserState;
                PersistUserState();
            }

            return didChange;
        }

        public bool UpdateAge(SwUserState userState)
        {
            var didChange = false;
            var currentAge = CalculateCurrentAgeDays();

            if (currentAge != userState.age)
            {
                userState.age = currentAge;
                userState.todaySessionsCount = 0;
                didChange = true;
            }

            return didChange;
        }

        public (SwJsonDictionary dataDictionary, IEnumerable<string> keysToEncrypt) ConditionallyAddExtraDataToTrackEvent(SwCoreUserData coreUserData)
        {
            var extraData = new SwJsonDictionary
            {
                { MAIN_LEVEL_WISDOM_ANALYTICS_KEY, ImmutableUserState().lastLevelStarted },
            };

            return (extraData, KeysToEncrypt());

            IEnumerable<string> KeysToEncrypt()
            {
                yield break;
            }
        }
        
        #endregion


        #region --- Private Methods ---

        [NotNull]
        protected virtual SwUserState DeserializeUserState(string userStateString)
        {
            return JsonUtility.FromJson<SwUserState>(userStateString) ?? new SwUserState();
        }

        // TODO Perry: We might want to include this logic inside the `ModifyUserStateSync` later, but currently we avoid it due to task complexity.
        internal void AfterUserStateChangeInternal(SwUserStateChangeReason reason, bool silent = false)
        {
            if (!silent)
            {
                OnUserStateChangeEvent?.Invoke(ImmutableUserState(), reason);
            }
        }

        internal SwUserState ImmutableUserState()
        {
            return _userState?.Copy();
        }

        private long CalculateCurrentAgeDays()
        {
            return Convert.ToInt64((DateTime.UtcNow - InstallDateTime).Days);
        }

        private void InjectTestDataToUserState([NotNull] SwUserState userStateValue)
        {
            if (SwTestUtils.CustomUserState != null)
            {
                if (SwTestUtils.CustomUserState.ContainsKey("todaySessionsCount"))
                {
                    userStateValue.todaySessionsCount = (long)SwTestUtils.CustomUserState["todaySessionsCount"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("totalSessionsCount"))
                {
                    userStateValue.totalSessionsCount = (long)SwTestUtils.CustomUserState["totalSessionsCount"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("completedLevels"))
                {
                    userStateValue.completedLevels = (long)SwTestUtils.CustomUserState["completedLevels"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("playedLevels"))
                {
                    userStateValue.playedLevels = (long)SwTestUtils.CustomUserState["playedLevels"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("consecutiveFailedLevels"))
                {
                    userStateValue.consecutiveFailedLevels = (long)SwTestUtils.CustomUserState["consecutiveFailedLevels"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("consecutiveCompletedLevels"))
                {
                    userStateValue.consecutiveCompletedLevels = (long)SwTestUtils.CustomUserState["consecutiveCompletedLevels"];
                }

                if (SwTestUtils.CustomUserState.ContainsKey("isDuringLevel"))
                {
                    userStateValue.isDuringLevel = (bool)SwTestUtils.CustomUserState["isDuringLevel"];
                }
            }
        }

        private void LoadCustomUuid()
        {
            _customUuid = SwInfra.KeyValueStore.GetString(SwStoreKeys.CustomUuid);

            if (string.IsNullOrEmpty(_customUuid))
            {
                CustomUuid = Guid.NewGuid().ToString();
            }
        }

        private void LoadInstallDate()
        {
            var dateInSecondsFromPrefs = SwInfra.KeyValueStore.GetString(SwStoreKeys.InstallTime);
            var dateFromPrefs = SwInfra.KeyValueStore.GetString(SwStoreKeys.InstallDate);
            
            if (string.IsNullOrEmpty(InstallDateTimeString) && !string.IsNullOrEmpty(dateFromPrefs))
            {
                InstallDateTimeString = dateFromPrefs;
            }

            if (!string.IsNullOrEmpty(dateInSecondsFromPrefs))
            {
                try
                {
                    InstallDateInSeconds = Convert.ToInt64(dateInSecondsFromPrefs);
                }
                catch (Exception)
                {
                    SwInfra.Logger.LogError(EWisdomLogType.User, "Cannot parse install in seconds from PlayerPerfs");
                }
            }

            // User is considered new if there are no saved values by wisdom
            IsNew = string.IsNullOrEmpty(InstallDateTimeString);
            
            var settings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                Culture = CultureInfo.InvariantCulture,
            };

            if (IsNew)
            {
                InstallDateTime = DateTime.UtcNow;
                InstallDateTimeString = JsonConvert.SerializeObject(InstallDateTime, settings);
                
                var seconds = SwUtils.DateAndTime.GetTotalSeconds(InstallDateTime);
                
                if (InstallDateInSeconds == 0)
                {
                    InstallDateInSeconds = seconds;
                }
            }
            else
            {
                // Backward compatibility: in version 7.4 and lower -  Install date save in format yyyy-MM-dd 
                InstallDateTime = SwUtils.DateAndTime.TryParseDateTimeUtc(InstallDateTimeString, SwConstants.INSTALL_DATE_FORMAT) ?? JsonConvert.DeserializeObject<DateTime>(InstallDateTimeString, settings);
            }
        }

        protected void LoadUserState()
        {
            var userStateString = SwInfra.KeyValueStore.GetString(USER_STATE_STORAGE_KEY, "{}");
            var userStateValue = DeserializeUserState(userStateString);

            InjectTestDataToUserState(userStateValue);
            UpdateAge(userStateValue);
            _userState = userStateValue.Copy();
        }

        private void PersistUserState()
        {
            SwInfra.KeyValueStore.SetString(USER_STATE_STORAGE_KEY, JsonUtility.ToJson(ImmutableUserState()));
            SwInfra.KeyValueStore.Save();
        }

        private void LoadActiveDays()
        {
            _activeDay.Load();
        }

        private void LoadInstallSdkVersion()
        {
            if (IsNew)
            {
                InstallSdkVersion = SwConstants.SDK_VERSION;
            }
            else
            {
                if (!SwInfra.KeyValueStore.HasKey(SwStoreKeys.InstallSdkVersion)) return;
                
                _installSdkVersion = SwInfra.KeyValueStore.GetString(SwStoreKeys.InstallSdkVersion);
            }
            
            InstallSdkVersionId = SwUtils.System.ComputeVersionId(InstallSdkVersion);
        }
        
        #endregion
    }
}