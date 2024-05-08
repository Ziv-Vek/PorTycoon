#if SW_STAGE_STAGE10_OR_ABOVE

using System.Collections;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace SupersonicWisdomSDK
{
    internal class SwStage10Container : SwCoreContainer
    {
        #region --- Members ---

        protected internal readonly SwBlockingApiHandler BlockingApiHandler;
        protected internal readonly SwStage10FacebookAdapter FacebookAdapter;
        protected internal readonly SwStage10GameAnalyticsAdapter GameAnalyticsAdapter;
        protected internal readonly SwStage10AppsFlyerAdapter AppsFlyerAdapter;
        protected internal readonly SwStage10Tracker Stage10Tracker;
        protected internal readonly SwFilesCacheManager FilesCacheManager;
        protected internal readonly SwStage10CvUpdater CvUpdater;
        protected internal readonly SwStage10RevenueCalculator RevenueCalculator;
        protected readonly List<ISwScriptLifecycleListener> ApplicationFocusListeners;
        protected internal readonly SwCoreFpsMeasurementManager FpsMeasurementManager;
        private readonly SwProgressionStatusSender ProgressionStatusSender;
        private readonly SwAliveStatusSender AliveStatusSender;
        private readonly SwUserActiveDay UserActiveDay;

        #endregion


        #region --- Construction ---

        internal SwStage10Container(
            Dictionary<string, object> initParamsDictionary,
            SwStage10MonoBehaviour mono,
            SwFilesCacheManager filesCacheManager,
            ISwAsyncCatchableRunnable stageSpecificCustomInitRunnable,
            SwSettingsManager<SwSettings> settingsManager,
            ISwReadyEventListener[] readyEventListeners,
            ISwUserStateListener[] userStateListeners,
            ISwLocalConfigProvider[] configProviders,
            ISwAdapter[] coreAdapters,
            SwStage10NativeAdapter wisdomNativeAdapter,
            SwStage10DeepLinkHandler deepLinkHandler,
            SwStage10DevTools devTools,
            SwCoreUserData coreUserData,
            SwStage10Tracker tracker,
            ISwConfigManager configManager,
            SwBlockingApiHandler blockingApiHandler,
            SwGameStateSystem gameStateSystem,
            SwStage10DataBridge dataBridge,
            SwStage10AppsFlyerAdapter appsFlyerAdapter,
            SwStage10FacebookAdapter facebookAdapter,
            SwStage10GameAnalyticsAdapter gameAnalyticsAdapter,
            SwTimerManager timerManager,
            SwStage10CvUpdater cvUpdater,
            SwStage10RevenueCalculator revenueCalculator,
            SwAliveStatusSender aliveStatusSender,
            SwProgressionStatusSender progressionStatusSender,
            SwCoreFpsMeasurementManager fpsMeasurementManager,
            SwUserActiveDay userActiveDay)
            : base(initParamsDictionary,
                mono,
                stageSpecificCustomInitRunnable,
                settingsManager,
                readyEventListeners,
                userStateListeners,
                configProviders,
                coreAdapters,
                wisdomNativeAdapter,
                deepLinkHandler,
                devTools,
                coreUserData,
                tracker,
                configManager,
                timerManager,
                gameStateSystem,
                dataBridge)
        {
            FilesCacheManager = filesCacheManager;
            BlockingApiHandler = blockingApiHandler;
            FacebookAdapter = facebookAdapter;
            GameAnalyticsAdapter = gameAnalyticsAdapter;
            AppsFlyerAdapter = appsFlyerAdapter;
            Stage10Tracker = tracker;
            CvUpdater = cvUpdater;
            RevenueCalculator = revenueCalculator;
            FacebookAdapter.OnFacebookInitCompleteEvent += OnFacebookInitComplete;
            AliveStatusSender = aliveStatusSender;
            ProgressionStatusSender = progressionStatusSender;
            FpsMeasurementManager = fpsMeasurementManager;
            UserActiveDay = userActiveDay;

            ApplicationFocusListeners = new List<ISwScriptLifecycleListener>
            {
                FacebookAdapter,
                TimerManager,
                UserActiveDay,
            };
        }


        #endregion


        #region --- Mono Override ---

        public override void OnApplicationPause(bool pauseStatus)
        {
            ApplicationFocusListeners.ForEach(e => e.OnApplicationPause(pauseStatus));
            
            SwInfra.Logger.Log(EWisdomLogType.Container, $"OnApplicationPause | {nameof(pauseStatus)}: {pauseStatus}");
        }

        public override void OnApplicationQuit()
        {
            SwInfra.Logger.Log(EWisdomLogType.Container);
        }

        #endregion


        #region --- Public Methods ---

        [Preserve]
        public new static ISwContainer GetInstance(Dictionary<string, object> initParamsDictionary)
        {
            var resourcePath = $"{SwStageUtils.CurrentStageName}/{SwConstants.GAME_OBJECT_NAME}{SwStageUtils.CurrentStageName}";
            var mono = SwContainerUtils.InstantiateSupersonicWisdom<SwStage10MonoBehaviour>(resourcePath);
            SwInfra.Initialize(mono, mono);
            
            var filesCacheManager = new SwFilesCacheManager();
            var settingsManager = new SwSettingsManager<SwSettings>();
            var wisdomNativeApi = SwNativeApiFactory.GetInstance();
            var userActiveDay = new SwUserActiveDay();
            var userData = new SwStage10UserData(settingsManager.Settings, wisdomNativeApi, userActiveDay);
            var sessionListener = new SwStage10SessionListener(userData);
            var timerManager = new SwTimerManager(mono);
            var revenueCalculator = new SwStage10RevenueCalculator(userData);
            var nativeAdditionalDataAssistant = new SwNativeAdditionalDataProvider(wisdomNativeApi, userData);
            ISwSessionListener[] swSessionListeners = { sessionListener, timerManager };
            var testUserState = new SwStage10TestUserState();
            var wisdomNativeAdapter = new SwStage10NativeAdapter(wisdomNativeApi, settingsManager.Settings, userData, swSessionListeners, testUserState, nativeAdditionalDataAssistant);
            var webRequestClient = new SwUnityWebRequestClient();
            var deepLinkHandler = new SwStage10DeepLinkHandler(settingsManager.Settings, webRequestClient);
            var devTools = new SwStage10DevTools(filesCacheManager);
            var dataBridge = new SwStage10DataBridge(userData, wisdomNativeApi, settingsManager.Settings);
            var tracker = new SwStage10Tracker(wisdomNativeAdapter, userData, webRequestClient, timerManager, dataBridge);
            var fpsMeasurementManager = new SwCoreFpsMeasurementManager(mono, timerManager, tracker);
            var gameStateSystem = new SwGameStateSystem(userData);
            var appsFlyerEventDispatcher = mono.GetComponent<SwAppsFlyerEventDispatcher>();
            var appsFlyerAdapter = new SwStage10AppsFlyerAdapter(appsFlyerEventDispatcher, userData, settingsManager, tracker);

            var facebookAdapter = new SwStage10FacebookAdapter();
            var gameAnalyticsAdapter = new SwStage10GameAnalyticsAdapter();
            var configManager = new SwStage10ConfigManager(settingsManager.Settings, userData, tracker, wisdomNativeAdapter, deepLinkHandler);
            var aliveStatusSender = new SwAliveStatusSender(fpsMeasurementManager, tracker, mono, settingsManager.Settings.isTimeBased, gameStateSystem);
            var progressionStatusSender = new SwProgressionStatusSender(fpsMeasurementManager, tracker, timerManager, userData);
            var blockingApiHandler = new SwBlockingApiHandler(settingsManager.Settings, gameStateSystem, null);
            
            var initThirdPartiesStep = new SwStage10InitThirdParties(facebookAdapter, gameAnalyticsAdapter);
            var fetchRemoteConfigStep = new SwStage10FetchRemoteConfig(configManager);
            var initAppsflyerStep = new SwStage10InitAppsflyer(appsFlyerAdapter);
            var cvUpdater = new SwStage10CvUpdater(mono, revenueCalculator, userData, tracker);

            var stageSpecificCustomInitRunnable = new SwAsyncFlow(new[]
            {
                new SwAsyncFlowStep(fetchRemoteConfigStep, 0),
                new SwAsyncFlowStep(initThirdPartiesStep, 0),
                //We excluded the initialization of AppsFlyer due to a dependency in a remote config value that determines the AF hostname.
                new SwAsyncFlowStep(initAppsflyerStep, 1),
            });

            ISwAdapter[] swAdapters = { appsFlyerAdapter, gameAnalyticsAdapter, facebookAdapter };
            // User data should be after config manager
            ISwReadyEventListener[] readyEventListeners = { configManager, appsFlyerAdapter, wisdomNativeAdapter, timerManager };
            ISwUserStateListener[] userStateListeners = { };
            ISwLocalConfigProvider[] configProviders = { configManager, appsFlyerAdapter, fpsMeasurementManager, aliveStatusSender };
            
            configManager.AddListeners(new List<ISwCoreConfigListener> { fpsMeasurementManager, aliveStatusSender });
            configManager.AddListeners(new List<ISwStage10ConfigListener> { userData, appsFlyerAdapter, wisdomNativeAdapter });
            gameStateSystem.AddGameStateListeners(new List<ISwGameStateSystemListener>() { configManager, aliveStatusSender });
            gameStateSystem.AddGameProgressionListeners(new ISwGameProgressionListener[] { gameAnalyticsAdapter, progressionStatusSender });
            deepLinkHandler.AddListeners(new List<ISwDeepLinkListener>() { devTools, testUserState });

            var trackerDataProviders = new List<ISwTrackerDataProvider> { timerManager, userData, userActiveDay };
            nativeAdditionalDataAssistant.SetTrackerDataProviders(trackerDataProviders);
            tracker.AddListeners(trackerDataProviders);

            return new SwStage10Container(initParamsDictionary, mono, filesCacheManager, stageSpecificCustomInitRunnable, settingsManager, readyEventListeners, userStateListeners, configProviders, swAdapters, wisdomNativeAdapter, deepLinkHandler, devTools, userData, tracker, configManager, blockingApiHandler, gameStateSystem, dataBridge, appsFlyerAdapter, facebookAdapter, gameAnalyticsAdapter, timerManager, cvUpdater, revenueCalculator, aliveStatusSender, progressionStatusSender, fpsMeasurementManager, userActiveDay);
        }

        public override ISwInitParams CreateInitParams()
        {
            return new SwStage10InitParams();
        }

        public override void OnAwake()
        {
            base.OnAwake();
            SwInfra.Logger.Log(EWisdomLogType.Container);
        }

        public override void OnStart()
        {
            SwInfra.Logger.Log(EWisdomLogType.Container);
        }

        #endregion


        #region --- Private Methods ---

        protected override IEnumerator BeforeReady()
        {
            yield return base.BeforeReady();
            yield return BlockingApiHandler.PrepareForGameStarted();
        }

        internal SwUserState CopyOfUserState()
        {
            return CoreUserData.ImmutableUserState();
        }
        
        private void OnFacebookInitComplete()
        {
            SwInfra.CoroutineService.StartCoroutine(CvUpdater.TryUpdateFirstCvUpdate());
        }

        #endregion
    }
}
#endif