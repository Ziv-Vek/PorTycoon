using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwCoreNativeAdapter : ISwReadyEventListener, ISwCoreConfigListener, ISwUserStateListener
    {
        #region --- Constants ---

        private const string EventsRemoteConfigStorageKey = "SupersonicWisdomEventsConfig";

        #endregion


        #region --- Members ---

        protected readonly SwCoreUserData CoreUserData;
        private bool _didFirstSessionStart;

        private string _abId = "";
        private string _abName = "";
        private string _abVariant = "";
        private EConfigStatus _configStatus = EConfigStatus.NotInitialized;
        
        private readonly ISwSessionListener[] _sessionListeners;
        private readonly ISwSettings _settings;
        protected readonly ISwNativeApi WisdomNativeApi;
        private readonly SwNativeRequestManager _nativeRequestManager;
        private readonly SwNativeAdditionalDataProvider _nativeAdditionalDataProvider;

        #endregion


        #region --- Properties ---

        public Tuple<EConfigListenerType, EConfigListenerType> ListenerType
        {
            get { return new Tuple<EConfigListenerType, EConfigListenerType>(EConfigListenerType.FinishWaitingForRemote, EConfigListenerType.EndOfGame); }
        }

        #endregion


        #region --- Construction ---

        public SwCoreNativeAdapter(ISwNativeApi wisdomNativeApi, ISwSettings settings, SwCoreUserData coreUserData, ISwSessionListener[] listeners, SwNativeAdditionalDataProvider nativeAdditionalDataProvider)
        {
            WisdomNativeApi = wisdomNativeApi;
            _settings = settings;
            CoreUserData = coreUserData;
            _sessionListeners = listeners ?? new ISwSessionListener[] { };
            _nativeRequestManager = new SwNativeRequestManager(wisdomNativeApi);
            _nativeAdditionalDataProvider = nativeAdditionalDataProvider;
			CoreUserData.OnUserStateChangeEvent += OnCoreUserStateChange;
        }

        #endregion


        #region --- Public Methods ---

        public virtual IEnumerator InitNativeSession()
        {
            WisdomNativeApi.InitializeSession(GetEventMetadata());

            if (WisdomNativeApi.IsSupported() && GetEventsConfig().enabled)
            {
                while (!_didFirstSessionStart)
                {
                    yield return null;
                }
            }
        }

        public virtual IEnumerator InitSDK()
        {
            yield return WisdomNativeApi.Init(GetWisdomNativeConfiguration());

            WisdomNativeApi.AddSessionStartedCallback(OnSessionStarted);
            WisdomNativeApi.AddSessionEndedCallback(OnSessionEnded);
            WisdomNativeApi.AddAdditionalDataJsonMethod(GetAdditionalDataJson);
            _nativeRequestManager.Init();

            UpdateMetadata();
        }

        public virtual string GetSubdomain()
        {
            var version = Application.version.Replace('.', '-');

            return $"{version}-{_settings.GetGameId()}";
        }

        public void StoreNativeConfig(SwNativeEventsConfig config)
        {
            var jsonConfig = JsonUtility.ToJson(config);

            if (string.IsNullOrEmpty(jsonConfig))
            {
                SwInfra.Logger.Log(EWisdomLogType.Native, "Config is null");

                return;
            }

            SwInfra.KeyValueStore.SetString(EventsRemoteConfigStorageKey, jsonConfig);
            SwInfra.KeyValueStore.Save();
        }

        public bool ToggleBlockingLoader(bool shouldPresent)
        {
            return WisdomNativeApi.ToggleBlockingLoader(shouldPresent);
        }

        public void TrackEvent(string eventName, string customsJson, string extraJson)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, $"eventName={eventName} | customsJson={customsJson} | extraJson = {extraJson}");
            WisdomNativeApi.TrackEvent(eventName, customsJson, extraJson);
        }

        public void UpdateAbData(string abId, string abKey, string abGroup, EConfigStatus configStatus)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, $"abKey = {abKey}");

            _abId = abId;
            _abName = abKey;
            _abVariant = abGroup;
            _configStatus = configStatus; 

            UpdateMetadata();
        }

        public void UpdateConfig()
        {
            if (GetEventsConfig().enabled)
            {
                WisdomNativeApi.UpdateWisdomConfiguration(GetWisdomNativeConfiguration());
            }
            else
            {
                WisdomNativeApi.Destroy();
            }
        }

        public void UpdateMetadata()
        {
            WisdomNativeApi.UpdateMetadata(GetEventMetadata());
        }

        public virtual void RequestRateUsPopup()
        {
            WisdomNativeApi.RequestRateUsPopup();
        }
        
        public string GetAppInstallSource()
        {
            return WisdomNativeApi.GetAppInstallSource();
        }
        
        public void OnSwReady ()
        {
            // Waiting for readiness for updating appsFlyerId which is available only after appsFlyer init complete
            UpdateMetadata();
        }

        public void SendRequest(string url, string body, ISwNativeRequestListener listener, string headers, int connectionTimeout, int readTimeout, int cap)
        {
            _nativeRequestManager.SendRequest(url, headers, body, listener, connectionTimeout, readTimeout, cap);
        }

        public void OnConfigResolved(ISwCoreInternalConfig configAccessor, ISwConfigManagerState state)
        {
            _configStatus = state.Status;

            UpdateMetadata();
        }

        #endregion


        #region --- Private Methods ---

        protected virtual SwNativeEventsConfig GetDefaultConfig()
        {
            return new SwNativeEventsConfig();
        }

        protected virtual SwEventMetadataDto GetEventMetadata()
        {
            var attStatus = SwAttUtils.GetStatus();

            var eventMetadata = new SwEventMetadataDto
            {
                bundle = CoreUserData.BundleIdentifier,
                os = CoreUserData.Platform,
                osVer = SystemInfo.operatingSystem,
                uuid = CoreUserData.Uuid,
                swInstallationId = CoreUserData.CustomUuid,
                device = SystemInfo.deviceModel,
                version = Application.version,
                sdkVersion = SwConstants.SDK_VERSION,
                sdkVersionId = SwConstants.SdkVersionId,
                sdkStage = SwStageUtils.CurrentStage.sdkStage.ToString(),
                installDate = CoreUserData.InstallDate,
                apiKey = _settings.GetAppKey(),
                gameId = _settings.GetGameId(),
                feature = SwConstants.FEATURE,
                featureVersion = SwConstants.FEATURE_VERSION,
                unityVersion = SwUtils.System.UnityVersion,
                attStatus = attStatus == SwAttAuthorizationStatus.Unsupported ? "" : $"{attStatus}",
                abId = _abId ?? "",
                abName = _abName ?? "",
                abVariant = _abVariant ?? "",
                configStatus = _configStatus.ToString(),
                installSdkVersion = CoreUserData.InstallSdkVersion,
                installSdkVersionId = CoreUserData.InstallSdkVersionId,
            };
            
            var organizationAdvertisingId = CoreUserData.OrganizationAdvertisingId;
#if UNITY_IOS
            eventMetadata.sandbox = SwUtils.Native.IsIosSandbox ? "1" : "0";
            eventMetadata.idfv = organizationAdvertisingId;
#endif
#if UNITY_ANDROID
            eventMetadata.appSetId = organizationAdvertisingId;
#endif

            return eventMetadata;
        }

        protected virtual SwNativeEventsConfig GetEventsConfig()
        {
            var jsonConfig = SwInfra.KeyValueStore.GetString(EventsRemoteConfigStorageKey, null);

            return !string.IsNullOrEmpty(jsonConfig) ? JsonConvert.DeserializeObject<SwNativeEventsConfig>(jsonConfig) : GetDefaultConfig();
        }

        protected virtual SwNativeConfig GetWisdomNativeConfiguration()
        {
            return CreateWisdomNativeConfiguration();
        }

        private SwNativeConfig CreateWisdomNativeConfiguration()
        {
            var config = GetEventsConfig();
            var blockingLoaderResourceRelativePath = SwUtils.System.IsRunningOnIos() ? "SupersonicWisdom/LoaderFrames" : "SupersonicWisdom/LoaderGif/animated_loader.gif";

            return new SwNativeConfig
            {
                Subdomain = GetSubdomain(),
                ConnectTimeout = config.connectTimeout,
                ReadTimeout = config.readTimeout,
                IsLoggingEnabled = _settings.IsDebugEnabled(),
                InitialSyncInterval = config.initialSyncInterval,
                StreamingAssetsFolderPath = Application.streamingAssetsPath,
                BlockingLoaderResourceRelativePath = blockingLoaderResourceRelativePath,
                BlockingLoaderViewportPercentage = 20,
            };
        }

        private void OnSessionEnded(string sessionId)
        {
            _sessionListeners?.ToList().ForEach(e => e.OnSessionEnded(sessionId));
        }

        private void OnSessionStarted(string sessionId)
        {
            _sessionListeners?.ToList().ForEach(e => e.OnSessionStarted(sessionId));

            if (!_didFirstSessionStart)
            {
                _didFirstSessionStart = true;
            }
        }
        
        private string GetAdditionalDataJson()
        {
            return _nativeAdditionalDataProvider?.GetAdditionalDataJson() ?? string.Empty;
        }

        #endregion


        #region --- Event Handler ---

        public void OnCoreUserStateChange(SwUserState newState, SwUserStateChangeReason reason)
        {
            UpdateMetadata();
        }

        #endregion
    }
}