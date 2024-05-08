using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal abstract class SwCoreConfigManager : ISwConfigManager, ISwNativeRequestListener, ISwLocalConfigProvider, ISwReadyEventListener, ISwGameStateSystemListener
    {
        #region --- Constants ---

        private const int CONFIG_DEFAULT_CAP = 10;
        private const int CONFIG_CONNECTION_TIMEOUT = 10000;
        private const int CONFIG_READ_TIMEOUT = 15000;
        private const int MAXIMUM_REQUEST_WAITING_TIME = 2;

        private const string API_VERSION = "2";
        private const string CONFIG_END_POINT = "https://{0}.config.mobilegamestats.com/";
        private const string SERVER_RESPONSE_EVENT_NAME = "ServerResponse";
        private const string AB_CONCLUDED_EVENT_NAME = "AbConcluded";
        private const string AB_CONCLUSION_SCOPE = "abConclusionScope";
        public const string AB_REMAINING_DAYS = "abRemainingDays";
        private const string HOST_APP_ERROR_ON_CALLBACK = "HostAppErrorOnCallback";

        private const string REPORT_CONFIG_RESPONSE_TIME = "configResponseTime";
        private const string REPORT_CONFIG_RESPONSE_CODE = "configResponseCode";
        private const string REPORT_CONFIG_RESPONSE_ERROR = "configResponseError";
        private const string REPORT_CONFIG_RESPONSE_ITERATION = "configResponseIteration";
        private const string INTERNAL_AB = "Internal";
        private const string EXTERNAL_AB = "External";
        public const string SW_INTERNAL_AB_CONCLUDED = "swInternalAbConcluded";
        public const string SW_EXTERNAL_AB_CONCLUDED = "swExtrnalAbConcluded";

        #endregion


        #region --- Events ---

        private event OnLoaded LoadedEvent;

        #endregion


        #region --- Members ---

        private readonly string _subdomain;
        private readonly ISwSettings _settings;
        private readonly SwCoreTracker _tracker;
        protected SwCoreNativeAdapter _nativeAdapter;
        private readonly SwCoreUserData _coreUserData;

        private bool _didInit;
        private SwAbConfig _ab;
        private bool _configArrived;
        private SwCoreConfig _coreConfig;
        private bool _internalAbConcluded;
        private bool _externalAbConcluded;
        
        private List<ISwCoreConfigListener> _listeners;
        protected HashSet<string> _predefinedKeys;
        private Dictionary<string, object> _localDynamicConfig;

        #endregion


        #region --- Properties ---

        public bool DidResolve { get; private set; }
        public bool IsTimeout { get; private set; }
        public EConfigStatus Status { get; private set; }

        public SwCoreConfig Config
        {
            get { return _coreConfig;}
            private set
            {
                _coreConfig = value;
                UpdateAb();
            }
        }


        public SwWebRequestError WebRequestError { get; private set; }
        public EConfigListenerType Timing { get; set; }

        private bool CanConcludeInternalAb
        {
            get { return !_internalAbConcluded && Timing <= EConfigListenerType.GameStarted; }
        }

        private bool CanConcludeExternalAb
        {
            get { return !_externalAbConcluded && Timing <= EConfigListenerType.FinishWaitingForRemote; }
        }

        private SwAbConfig Ab
        {
            get
            {
                return _ab;
            }
            set
            {
                if (value == null) return;

                _ab = value;
                UpdateAb();
                _nativeAdapter.UpdateAbData(_ab.id, _ab.key, _ab.group, Status);
            }
        }

        private bool ShouldSaveAb(string abKey)
        {
            SwInfra.Logger.Log(EWisdomLogType.Config, $"{nameof(_internalAbConcluded)} = {_internalAbConcluded} | {nameof(_externalAbConcluded)} = {_externalAbConcluded} | {nameof(abKey)} = {abKey}");
            
            if (_externalAbConcluded && _internalAbConcluded) return false;
            if (string.IsNullOrEmpty(abKey)) return true;
            
            var isInternalAb = _predefinedKeys.Contains(abKey);
            var shouldConcludeAb = isInternalAb ? CanConcludeInternalAb : CanConcludeExternalAb;

            SwInfra.Logger.Log(EWisdomLogType.Config, $"{nameof(isInternalAb)} = {isInternalAb} | {nameof(CanConcludeInternalAb)} = {CanConcludeInternalAb} | {nameof(CanConcludeExternalAb)} = {CanConcludeExternalAb}");
            return shouldConcludeAb;
        }

        #endregion


        #region --- Construction ---

        protected SwCoreConfigManager(ISwSettings settings, SwCoreUserData coreUserData, SwCoreTracker tracker, SwCoreNativeAdapter nativeAdapter)
        {
            _nativeAdapter = nativeAdapter;
            _subdomain = _nativeAdapter.GetSubdomain();
            _coreUserData = coreUserData;
            _settings = settings;
            _tracker = tracker;
            Status = EConfigStatus.NotInitialized;
            _listeners = new List<ISwCoreConfigListener>();
        }

        #endregion


        #region --- Public Methods ---

        public SwLocalConfig GetLocalConfig()
        {
            return new SwConfigManagerLocalConfig();
        }

        public void Init(ISwLocalConfigProvider[] localConfigProviders)
        {
            Timing = EConfigListenerType.Construction;
            
            LoadLocalConfig(localConfigProviders);
            TryLoadLatestSuccessfulRemoteConfig();
            TryLoadSavedAb();
            ResolveIsAbConcluded();
            
            OnConfigReady();
        }

        // For backward compatibility
        private void ResolveIsAbConcluded()
        {
            if (_coreUserData.IsNew)
            {
                _internalAbConcluded = false;
                _externalAbConcluded = false;
                SwInfra.KeyValueStore.SetBoolean(SW_INTERNAL_AB_CONCLUDED, _internalAbConcluded);
                SwInfra.KeyValueStore.SetBoolean(SW_EXTERNAL_AB_CONCLUDED, _externalAbConcluded);
            }
            else
            {
                var comeFromUpdate = !SwInfra.KeyValueStore.HasKey(SW_EXTERNAL_AB_CONCLUDED) || !SwInfra.KeyValueStore.HasKey(SW_INTERNAL_AB_CONCLUDED);

                if (comeFromUpdate)
                {
                    _internalAbConcluded = true;
                    _externalAbConcluded = true;
                    SwInfra.KeyValueStore.SetBoolean(SW_INTERNAL_AB_CONCLUDED, _internalAbConcluded);
                    SwInfra.KeyValueStore.SetBoolean(SW_EXTERNAL_AB_CONCLUDED, _externalAbConcluded);
                }
                else
                {
                    _internalAbConcluded = SwInfra.KeyValueStore.GetBoolean(SW_INTERNAL_AB_CONCLUDED);
                    _externalAbConcluded = SwInfra.KeyValueStore.GetBoolean(SW_EXTERNAL_AB_CONCLUDED);
                }
            }
        }

        public void OnSwReady()
        {
            TryTrackExternalAbConcludedEvent();
        }

        public void AddListeners(List<ISwCoreConfigListener> listeners)
        {
            _listeners.AddRange(listeners);
        }

        public void AddOnLoadedListener(OnLoaded onLoadedCallback)
        {
            LoadedEvent += onLoadedCallback;
        }
        
        public void OnGameSystemStateChange(SwSystemStateEventArgs eventArgs)
        {
            if (IsNotInGameplayState(eventArgs.NewGameState)) return;

            OnGameStarted();
        }

        public IEnumerator Fetch()
        {
            Timing = EConfigListenerType.FinishWaitingForRemote;
            _didInit = true;
            
            if (!_didInit)
            {
                throw new SwException($"{nameof(SwCoreConfigManager)} | {nameof(Fetch)} | Init method must be called before calling Fetch.");
            }
            
            SendRemoteConfigRequest();

            yield return WaitForRemoteOrTimeout(MAXIMUM_REQUEST_WAITING_TIME);

            if (IsTimeout)
            {
                OnConfigReady();
            }

            Timing = EConfigListenerType.GameStarted;
        }

        public void RemoveOnLoadedListener(OnLoaded onLoadedCallback)
        {
            LoadedEvent -= onLoadedCallback;
        }

        public void OnIteration(SwWebResponse response)
        {
            SwInfra.Logger.Log(EWisdomLogType.Config);

            if (Config != null && !Config.GetValue(SwConfigManagerLocalConfig.SHOULD_REPORT_CONFIG_ITERATION_KEY, SwConfigManagerLocalConfig.SHOULD_REPORT_CONFIG_ITERATION_VALUE))
            {
                return;
            }

            TrackResponse(response);
        }

        public void OnFail(SwWebResponse response)
        {
            SwInfra.Logger.Log(EWisdomLogType.Config);

            TrackResponse(response);
        }

        public void OnSuccess(SwWebResponse response)
        {
            if (_configArrived) return;

            SwInfra.Logger.Log(EWisdomLogType.Config);

            _configArrived = true;

            try
            {
                var parsedRemoteConfig = TryParseRemoteConfig(response);

                if (parsedRemoteConfig != null)
                {
                    if (ShouldSaveAb(parsedRemoteConfig.Ab?.key))
                    {
                        SetNewConfig(parsedRemoteConfig, parsedRemoteConfig.Ab, EConfigStatus.Remote);
                        SaveInitialConfigResponse(response.Text);
                    }
                    else
                    {
                        SetNewConfig(parsedRemoteConfig, Ab, EConfigStatus.Remote);
                    }

                    SaveCachedConfigResponse(response.Text);

                    TryTrackInternalAbConcludedEvent();
                    TryTrackExternalAbConcludedEvent();
                }
            }
            finally
            {
                OnConfigReady();
                NotifyExternalListeners();
                TrackResponse(response);
            }
        }

        #endregion


        #region --- Private Methods ---

        protected virtual void OnConfigReady()
        {
            NotifyConfigResolved();
        }

        protected virtual SwRemoteConfigRequestPayload CreatePayload()
        {
            var organizationAdvertisingId = _coreUserData.OrganizationAdvertisingId;

            return new SwRemoteConfigRequestPayload
            {
                bundle = _coreUserData.BundleIdentifier,
                gameId = _settings.GetGameId(),
                os = _coreUserData.Platform,
                osver = SystemInfo.operatingSystem,
                uuid = _coreUserData.Uuid,
                session = _coreUserData.ImmutableUserState().SessionId,
                device = SystemInfo.deviceModel,
                version = Application.version,
                sdkVersion = SwConstants.SDK_VERSION,
                sdkVersionId = SwConstants.SdkVersionId,
                stage = SwStageUtils.CurrentStage.sdkStage,
                sysLang = Application.systemLanguage.ToString(),
                isNew = _coreUserData.IsNew ? "1" : "0",
                apiVersion = API_VERSION,
                installSdkVersion = _coreUserData.InstallSdkVersion,
                installSdkVersionId = _coreUserData.InstallSdkVersionId,
#if UNITY_IOS
                idfv = organizationAdvertisingId,
#endif
#if UNITY_ANDROID
                appSetId = organizationAdvertisingId,
#endif
            };
        }

        protected abstract SwCoreConfig ParseConfig(string configStr);

        protected void NotifyConfigResolved()
        {
            SwInfra.Logger.Log(EWisdomLogType.Config, $"Config resolved | Timing = {Timing} | Status - {Status}");

            DidResolve = true;

            NotifyInternalListeners();
        }

        private void TryTrackInternalAbConcludedEvent()
        {
            SwInfra.Logger.Log(EWisdomLogType.Config, $"{nameof(_internalAbConcluded)} = {_internalAbConcluded}");
            if (_internalAbConcluded) return;
            
            _internalAbConcluded = true;
            SwInfra.KeyValueStore.SetBoolean(SW_INTERNAL_AB_CONCLUDED, _internalAbConcluded);

            SendAbConcludedEvent(INTERNAL_AB);
        }

        private void SendAbConcludedEvent(string abConclusionScope)
        {
            var customs = SwCoreTracker.GenerateEventCustoms(AB_CONCLUDED_EVENT_NAME, abConclusionScope);
            customs.SwAddOrReplace(AB_CONCLUSION_SCOPE, abConclusionScope);

            var remainingDays = Ab?.RemainingDays(); // The A/B object still could be null here.

            if (remainingDays > 0)
            {
                // This A/B Test has termination conditions
                customs.SwAddOrReplace(AB_REMAINING_DAYS, remainingDays.Value);
            }
            
            _tracker?.TrackInfraEvent(customs);
        }

        private void TryTrackExternalAbConcludedEvent()
        {
            SwInfra.Logger.Log(EWisdomLogType.Config, $"{nameof(_externalAbConcluded)} = {_externalAbConcluded}");
            if (_externalAbConcluded) return;

            _externalAbConcluded = true;
            SwInfra.KeyValueStore.SetBoolean(SW_EXTERNAL_AB_CONCLUDED, _externalAbConcluded);
            SendAbConcludedEvent(EXTERNAL_AB);
        }

        private void LoadLocalConfig(ISwLocalConfigProvider[] localConfigProviders = null)
        {
            var localDynamicConfig = new SwLocalConfigHandler(localConfigProviders).ConfigValues;
            _predefinedKeys = localDynamicConfig.Keys.SwToHashSet();
            
            var localConfig = CreateLocalConfig(localDynamicConfig);
            _localDynamicConfig = localConfig.DynamicConfig;
            SetNewConfig(localConfig, null, EConfigStatus.Local, false);
        }

        private void TryLoadSavedAb()
        {
            var ab = LoadConfigFromPlayerPrefs(EStoredConfig.Initial)?.Ab;

            Ab ??= ab;
        }

        protected abstract SwCoreConfig CreateLocalConfig(Dictionary<string, object> localConfigValues);

        private void NotifyExternalListeners()
        {
            try
            {
                LoadedEvent?.Invoke(Status == EConfigStatus.Remote, WebRequestError);
            }
            catch (Exception e)
            {
                _tracker?.TrackInfraEvent(HOST_APP_ERROR_ON_CALLBACK, e.Message, e.StackTrace);
                SwInfra.Logger.LogError(EWisdomLogType.Config, e.Message);
            }
        }

        private SwCoreConfig LoadConfigFromPlayerPrefs(EStoredConfig type)
        {
            var configStr = string.Empty;
            
            switch (type)
            {
                case EStoredConfig.Initial:
                    configStr = SwInfra.KeyValueStore.GetString(SwStoreKeys.InitialConfig);
                    break;
                
                case EStoredConfig.Latest:
                    configStr = SwInfra.KeyValueStore.GetString(SwStoreKeys.LatestSuccessfulConfigResponse);
                    break;
            }

            return DeserializeConfigJson(configStr);
        }

        private void SendRemoteConfigRequest()
        {
            SwInfra.Logger.Log(EWisdomLogType.Config, "Send remote config request");

            var bodyJsonString = JsonConvert.SerializeObject(CreatePayload());
            
            _nativeAdapter.SendRequest(CONFIG_END_POINT.Format(_subdomain), bodyJsonString, this, "", CONFIG_CONNECTION_TIMEOUT, CONFIG_READ_TIMEOUT, CONFIG_DEFAULT_CAP);
        }

        private IEnumerator WaitForRemoteOrTimeout(float maximumWaitingIntervalInSeconds)
        {
            var timer = 0f;

            while (timer < maximumWaitingIntervalInSeconds)
            {
                if (_configArrived)
                {
                    yield break;
                }

                timer += Time.deltaTime;

                yield return null;
            }

            IsTimeout = true;
            WebRequestError = SwWebRequestError.Timeout;
        }

        private void TrackResponse(SwWebResponse response)
        {
            _tracker?.TrackEventWithParams(SERVER_RESPONSE_EVENT_NAME, new Dictionary<string, object>()
            {
                {REPORT_CONFIG_RESPONSE_TIME, response.time},
                {REPORT_CONFIG_RESPONSE_CODE, response.code},
                {REPORT_CONFIG_RESPONSE_ERROR, response.error},
                {REPORT_CONFIG_RESPONSE_ITERATION, response.iteration},
            });
        }

        private SwCoreConfig DeserializeConfigJson(string remoteConfigJsonString)
        {
            return remoteConfigJsonString.SwIsNullOrEmpty() ? null : ParseConfig(remoteConfigJsonString);
        }

        protected virtual void NotifyInternalListeners()
        {
            if (_listeners != null && _listeners.Count > 0)
            {
                foreach (var listener in _listeners)
                {
                    if (listener is null) continue;
                    
                    if (listener.ListenerType.Item1 <= Timing && listener.ListenerType.Item2 >= Timing)
                    {
                        try
                        {
                            listener.OnConfigResolved(Config, this);
                        }
                        catch (Exception e)
                        {
                            SwInfra.Logger.LogError(EWisdomLogType.Config, $"An error was thrown by one of the listeners: {e.Message}");
                        }
                    }
                }
            }
        }

        private void SaveInitialConfigResponse(string responseText)
        {
            SwInfra.KeyValueStore.SetString(SwStoreKeys.InitialConfig, responseText);
        }

        private void SaveCachedConfigResponse(string responseText)
        {
            SwInfra.KeyValueStore
                   .SetString(SwStoreKeys.LatestSuccessfulConfigResponse, responseText)
                   .Save();
        }

        private void TryLoadLatestSuccessfulRemoteConfig()
        {
            var latestSuccessfulRemoteConfig = LoadConfigFromPlayerPrefs(EStoredConfig.Latest);

            if (latestSuccessfulRemoteConfig != null)
            {
                SetNewConfig(latestSuccessfulRemoteConfig, null, EConfigStatus.Cached);
            }
        }

        private void SetNewConfig(SwCoreConfig configToSet, SwAbConfig ab, EConfigStatus newStatus, bool addLocalConfig = true)
        {
            SwInfra.Logger.Log(EWisdomLogType.Config, $"will set config: {configToSet}, ab: {ab}");
            Config = configToSet;
            SwInfra.Logger.Log(EWisdomLogType.Config, $"did set config: {configToSet}, ab: {ab}");
            Config.DynamicConfig ??= new Dictionary<string, object>();

            if (addLocalConfig && _localDynamicConfig != null)
            {
                Config.DynamicConfig.SwMerge(false, _localDynamicConfig);
            }
            
            Status = newStatus;
            Ab = ab;
        }

        private SwCoreConfig TryParseRemoteConfig(SwWebResponse response)
        {
            if (!response.DidSucceed) return null;

            try
            {
                var remoteConfig = DeserializeConfigJson(response.Text);
                WebRequestError = response.error;
                
                return remoteConfig;
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Config, $"An exception was thrown while trying to parse remote config: {e.Message}");
                return null;
            }
        }

        private void UpdateAb()
        {
            Config.Ab = Ab ?? new SwAbConfig();
            
            if (Config.Ab.IsValid && !Config.Ab.IsExpired)
            {
                var dynamicAb = SwConfigUtils.ResolveAbConfig(Ab);
                Config.DynamicConfig ??= new Dictionary<string, object>();
                Config.DynamicConfig.SwMerge(true, dynamicAb);

                SwInfra.Logger.Log(EWisdomLogType.Config, $"abKey = {Ab.key} | abKey = {Ab.value}");
            }

            SwInfra.Logger.Log(EWisdomLogType.Config, $"Config.ab: {Config.Ab}");
        }

        private void OnGameStarted()
        {
            TryTrackInternalAbConcludedEvent();
            Timing = EConfigListenerType.EndOfGame;
        }

        private static bool IsNotInGameplayState(SwSystemState.EGameState state)
        {
            return state != SwSystemState.EGameState.Regular && state != SwSystemState.EGameState.Bonus && state != SwSystemState.EGameState.Tutorial && state != SwSystemState.EGameState.Time;
        }

        #endregion


        #region --- Inner classes ---

        private enum EStoredConfig
        {
            Initial,
            Latest,
        }

        #endregion
    }
}
