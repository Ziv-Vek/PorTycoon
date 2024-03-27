#if SW_STAGE_STAGE10_OR_ABOVE

using System.Collections.Generic;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    internal class SwStage10ConfigManager : SwCoreConfigManager
    {
        #region --- Members ---

        private readonly SwDeepLinkHandler _deepLinkHandler;

        private readonly List<ISwStage10ConfigListener> _listeners;

        #endregion


        #region --- Construction ---

        public SwStage10ConfigManager(ISwSettings settings, SwCoreUserData coreUserData, SwCoreTracker tracker, SwStage10NativeAdapter swStage10NativeAdapter, SwDeepLinkHandler deepLinkHandler) : base(settings, coreUserData, tracker, swStage10NativeAdapter)
        {
            _deepLinkHandler = deepLinkHandler;
            _listeners = new List<ISwStage10ConfigListener>();
        }

        #endregion


        #region --- Public Methods ---

        public void AddListeners(List<ISwStage10ConfigListener> listeners)
        {
            _listeners.AddRange(listeners);
        }

        #endregion


        #region --- Private Methods ---
        
        protected override SwCoreConfig CreateLocalConfig(Dictionary<string, object> localConfigValues)
        {
            return new SwStage10Config(localConfigValues);;
        }

        protected override SwCoreConfig ParseConfig(string configStr)
        {
            return JsonConvert.DeserializeObject<SwStage10Config>(configStr);
        }
        
        protected override void NotifyInternalListeners()
        {
            base.NotifyInternalListeners();

            var config = Config as SwStage10Config;
            
            if (_listeners != null && _listeners.Count > 0)
            {
                foreach (var listener in _listeners)
                {
                    if (listener is null) continue;

                    if (listener.ListenerType.Item1 <= Timing && listener.ListenerType.Item2 >= Timing)
                    {
                        listener.OnConfigResolved(config, this);
                    }
                }
            }
        }

        protected override void OnConfigReady()
        {
            base.OnConfigReady();

            TryLoadDeepLinkConfig();
        }

        private void TryLoadDeepLinkConfig()
        {
            var resolvedDeepLinkConfig = SwConfigUtils.ResolveDeepLinkConfig(_deepLinkHandler.DeepLinkParams);
            SwInfra.Logger.Log(EWisdomLogType.Config, resolvedDeepLinkConfig.SwToJsonString());
            Config.DynamicConfig.SwMerge(true, resolvedDeepLinkConfig);
        }

        protected override SwRemoteConfigRequestPayload CreatePayload()
        {
            var payload = base.CreatePayload();
            
            payload.abTestVariant = TryDeeplinkForAbTestVariant();
            
            return payload;
        }

        private string TryDeeplinkForAbTestVariant()
        {
            if (_deepLinkHandler?.DeepLinkParams == null) return null;
            
            var containAbVariant = _deepLinkHandler.DeepLinkParams.TryGetValue(SwStage10DeepLinkConstants.AB_TEST_VARIENT, out var result);
            var variant = containAbVariant ? result : null;
            
            return variant;
        }

        #endregion
    }
}

#endif
