using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace SupersonicWisdomSDK
{
    internal abstract class SwCoreContainer : ISwContainer
    {
        #region --- Constants ---
        
        private const string READY_EVENT_NAME = "Ready";
        private const string APP_OPEN_EVENT_NAME = "AppOpen";
        
        #endregion
        
        
        #region --- Events ---
        public event OnReady OnReadyEvent;
        
        #endregion
        
        
        #region --- Members ---
        
        protected internal readonly ISwInitParams InitParams;
        protected internal readonly SwCoreMonoBehaviour Mono;
        protected internal readonly ISwAsyncCatchableRunnable StageSpecificCustomInitRunnable;
        protected internal readonly SwSettings Settings;
        protected internal readonly SwCoreNativeAdapter WisdomCoreNativeAdapter;
        protected internal readonly SwDeepLinkHandler DeepLinkHandler;
        protected internal readonly SwLocalConfigHandler LocalConfigHandler;
        protected internal readonly SwDevTools DevTools;
        protected internal readonly SwCoreUserData CoreUserData;
        protected internal readonly SwCoreTracker Tracker;
        protected internal readonly ISwConfigManager ConfigManager;
        protected internal readonly ISwAdapter[] SwAdapters;
        protected internal readonly ISwReadyEventListener[] ReadyEventListeners;
        protected internal readonly ISwUserStateListener[] UserStateListeners;
        protected internal readonly ISwLocalConfigProvider[] ConfigProviders;
        protected internal readonly SwTimerManager TimerManager;
        protected internal readonly SwGameStateSystem GameStateSystem;
        protected internal SwCoreDataBridge DataBridge;
        protected bool WasDestroyed;
        
        #endregion
        
        
        #region --- Properties ---
        
        public bool IsReady { get; private set; }
        
        #endregion

        
        #region --- Construction ---
        
        protected SwCoreContainer(Dictionary<string, object> initParamsDictionary, SwCoreMonoBehaviour mono, ISwAsyncCatchableRunnable stageSpecificCustomInitRunnable, SwSettingsManager<SwSettings> settingsManager, ISwReadyEventListener[] readyEventListeners, ISwUserStateListener[] userStateListeners, ISwLocalConfigProvider[] configProviders, ISwAdapter[] swAdapters, SwCoreNativeAdapter wisdomCoreNativeAdapter, SwDeepLinkHandler deepLinkHandler, SwDevTools devTools, SwCoreUserData coreUserData, SwCoreTracker tracker, ISwConfigManager configManager, SwTimerManager timerManager, SwGameStateSystem gameStateSystem, SwCoreDataBridge dataBridge)
        {
            // ReSharper disable VirtualMemberCallInConstructor
            InitParams = CreateInitParams();
            PopulateInitParams(initParamsDictionary ?? new Dictionary<string, object>());
            // ReSharper restore VirtualMemberCallInConstructor

            StageSpecificCustomInitRunnable = stageSpecificCustomInitRunnable;
            ReadyEventListeners = readyEventListeners;
            SwAdapters = swAdapters;
            
            Settings = settingsManager.Settings;
            WisdomCoreNativeAdapter = wisdomCoreNativeAdapter;
            DeepLinkHandler = deepLinkHandler;
            DevTools = devTools;
            CoreUserData = coreUserData;
            Tracker = tracker;
            ConfigProviders = configProviders;
            ConfigManager = configManager;
            TimerManager = timerManager;
            GameStateSystem = gameStateSystem;
            DataBridge = dataBridge;

            if (userStateListeners != null)
            {
                foreach (var userStateListener in userStateListeners)
                {
                    CoreUserData.OnUserStateChangeEvent += userStateListener.OnCoreUserStateChange;
                }
            }

            Mono = mono;
            Mono.LifecycleListener = this;
        }

        #endregion
        
        
        #region --- Public Methods ---

        public SwCoreMonoBehaviour GetMono ()
        {
            return Mono;
        }

        public static ISwContainer GetInstance(Dictionary<string, object> initParamsDictionary)
        {
            throw new NotImplementedException();
        }

        public abstract ISwInitParams CreateInitParams ();

        public virtual void PopulateInitParams(Dictionary<string, object> initParamsDictionary)
        { }

        public virtual void OnAwake ()
        {
            SwContainerUtils.InitContainerAsync(this, StageSpecificCustomInitRunnable);
        }

        public virtual IEnumerator InitAsync()
        {
            yield return DeepLinkHandler.SetupDeepLink();
            
            yield return DevTools.EnableDevtools(Settings.enableDevtools);

            yield return WisdomCoreNativeAdapter.InitSDK();
            
            ConfigManager.Init(ConfigProviders);
            CoreUserData.Load(InitParams);
            ConfigManager.Init(ConfigProviders);

            yield return WisdomCoreNativeAdapter.InitNativeSession();

            var eventCustoms = SwCoreTracker.GenerateEventCustoms(APP_OPEN_EVENT_NAME, CoreUserData.IsNew ? "first" : "");
            var abRemainingDays = ConfigManager.Config.Ab.RemainingDays();
            
            if (abRemainingDays > 0)
            {
                eventCustoms.SwAddOrReplace(SwCoreConfigManager.AB_REMAINING_DAYS, abRemainingDays);
            }
            
            Tracker.TrackInfraEvent(eventCustoms);
        }

        public virtual void AfterInit(Exception exception)
        {
            if (exception != null)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Container, $"Container init error: {exception.Message}\n{exception.StackTrace}");
                // @todo [next] log error to (some) cloud so it can be monitored
            }
            
            SwInfra.CoroutineService.StartCoroutine(NotifyReadiness());
        }
        
        
        public bool IsDestroyed ()
        {
            return WasDestroyed;
        }
        
        public List<string> Validate ()
        {
            // ReSharper disable once CollectionNeverUpdated.Local
            var validationErrors = new List<string>();
#if UNITY_IOS && !UNITY_EDITOR
            var skAdNetworks = _swGetSkAdNetworks();
            if (string.IsNullOrEmpty(skAdNetworks))
            {
                validationErrors.Add($"Missing {SwAttributionConstants.SkAdNetworkItemsKey} in Info.plist. Make sure internet access is available when building the unity project.");
            }

            var advertisingAttributionReportEndpoint = _swGetAdvertisingAttributionReportEndpoint();
            if (string.IsNullOrEmpty(advertisingAttributionReportEndpoint))
            {
                validationErrors.Add($"{SwAttributionConstants.AdvertisingAttributionReportEndpointKey} in Info.plist does not equal to {SwAttributionConstants.AdvertisingAttributionReportEndpoint}");
            }
#endif
            return validationErrors;
        }

        #endregion
        
        
        #region --- Lifecycle Events ---

        public abstract void OnStart();

        public virtual void OnUpdate()
        {
            
        }

        public abstract void OnApplicationPause(bool pauseStatus);

        public abstract void OnApplicationQuit ();

        public virtual void Destroy ()
        {
            WasDestroyed = true;
        }
        
        #endregion

        
        #region --- Private Methods ---

        protected virtual IEnumerator BeforeReady ()
        {
            Settings.LogKeys();
            yield break;
        }

        protected IEnumerator NotifyReadiness()
        {
            if (IsReady) yield break;

            yield return BeforeReady();

            IsReady = true;
            
            foreach (var readyEventListener in ReadyEventListeners)
            {
                try
                {
                    readyEventListener?.OnSwReady();
                }
                catch (Exception e)
                {
                    SwInfra.Logger.LogError(EWisdomLogType.Container, $"{nameof(SwCoreContainer)} | {nameof(OnReadyEvent)} | Error | {e.Message}");
                    Tracker.TrackInfraEvent("ReadyError", e.Message, e.Source);
                }
            }
            
            Tracker.TrackInfraEvent(READY_EVENT_NAME, GetAdapterVersionAndStatus(), WisdomCoreNativeAdapter.GetAppInstallSource());
            SwInfra.Logger.Log(EWisdomLogType.Container, $"{nameof(SwCoreContainer)} | {nameof(OnReadyEvent)}");
            
            NotifyExternalReady();
        }

        private void NotifyExternalReady()
        {
            // OnReadyEvent is accessed from outside the SDK, so we need to catch any exceptions
            // Internal OnReadyEvent listeners are already wrapped in a try-catch in the NotifyReadiness method
            try
            {
                OnReadyEvent?.Invoke();
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Container, $"{nameof(SwCoreContainer)} | {nameof(NotifyExternalReady)} | Error | {e.Message}");
            }
        }

        private string GetAdapterVersionAndStatus()
        {
            if (SwAdapters.Length <= 0) return string.Empty;
            var listOfAdapters = new List<SwAdapterData>();

            foreach (var adapter in SwAdapters)
            {
                listOfAdapters.Add(adapter.GetAdapterStatusAndVersion());
            }

            return JsonConvert.SerializeObject(listOfAdapters);
        }
        
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string _swGetSkAdNetworks();

        [DllImport("__Internal")]
        private static extern string _swGetAdvertisingAttributionReportEndpoint();
#endif
        
        #endregion
    }
}