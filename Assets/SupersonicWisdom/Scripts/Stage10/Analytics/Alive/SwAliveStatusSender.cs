#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwAliveStatusSender : SwBaseAnalyticsManager, ISwCoreConfigListener, ISwGameStateSystemListener, ISwLocalConfigProvider
    {
        #region --- Constants ---

        private const int INVALID_INTERVAL_VALUE = -1;
        private const string ALIVE_EVENT_NAME = "Alive";

        #endregion


        #region --- Members ---

        private readonly bool _isTimeBased;
        private readonly ISwMonoBehaviour _mono;
        private readonly SwGameStateSystem _gameStateSystem;
        private readonly SwEventDataMeasurementProvider _measurementProvider;
        
        private bool _isEnabled;
        private string _aliveEventIntervalsRawConfig;
        private Coroutine _recurringAliveSenderCoroutine;
        private readonly Dictionary<SwSystemState.EGameState, int> _configDict; 


        #endregion


        #region --- Properties ---

        public Tuple<EConfigListenerType, EConfigListenerType> ListenerType
        {
            get { return new Tuple<EConfigListenerType, EConfigListenerType>(EConfigListenerType.FinishWaitingForRemote, EConfigListenerType.EndOfGame); }
        }
        
        private int CurrentInterval
        {
            get
            {
                return _configDict.TryGetValue(_gameStateSystem.CurrentGameState, out var interval) 
                    ? interval
                    : _isTimeBased 
                        ? SwAliveSenderLocalConfig.ALIVE_EVENT_INTERVALS_CONFIG_VALUE 
                        : INVALID_INTERVAL_VALUE;
            }
        }

        #endregion


        #region --- Construction ---

        public SwAliveStatusSender(SwCoreFpsMeasurementManager fpsMeasurementManager, ISwTracker tracker, ISwMonoBehaviour monoBehaviour, bool isTimeBased, SwGameStateSystem gameStateSystem) : base(tracker)
        {
            _gameStateSystem = gameStateSystem;
            _isTimeBased = isTimeBased;
            _mono = monoBehaviour;

            _measurementProvider = new SwEventDataMeasurementProvider(fpsMeasurementManager);
            _configDict = new Dictionary<SwSystemState.EGameState, int>();
        }

        #endregion


        #region --- Public Methods ---

        public void OnConfigResolved(ISwCoreInternalConfig configAccessor, ISwConfigManagerState state)
        {
            _aliveEventIntervalsRawConfig = configAccessor.GetValue(SwAliveSenderLocalConfig.ALIVE_EVENT_INTERVALS_CONFIG_KEY, SwAliveSenderLocalConfig.ALIVE_EVENT_INTERVALS_CONFIG_KEY);

            if (_aliveEventIntervalsRawConfig == SwAliveSenderLocalConfig.ALIVE_EVENT_INTERVALS_CONFIG_VALUE.ToString())
            {
                _configDict.Clear();
                _isEnabled = false;

                return;
            }

            _isEnabled = true;

            if (int.TryParse(_aliveEventIntervalsRawConfig, out var result))
            {
                _configDict.FillWith(result);
            }
            else
            {
                _configDict.FillWith(INVALID_INTERVAL_VALUE);
                OverrideCustomCustomConfig();
            }
            
            SwInfra.Logger.Log(EWisdomLogType.Alive, $"Configuration \"{_aliveEventIntervalsRawConfig}\" parse to {JsonConvert.SerializeObject(_configDict)}");
            
            Restart();
        }

        public void OnGameSystemStateChange(SwSystemStateEventArgs eventArgs)
        {
            if (!_isEnabled) return;

            Restart();
        }

        public SwLocalConfig GetLocalConfig()
        {
            return new SwAliveSenderLocalConfig();
        }

        #endregion


        #region --- Private Methods ---

        private void Restart()
        {
            SwInfra.Logger.Log(EWisdomLogType.Alive, "Restarting measure");
            
            RestartCoroutine(CurrentInterval);
            _measurementProvider.GetDataAndRestart();
        }

        private void OverrideCustomCustomConfig()
        {
            try
            {
                var settings = new JsonSerializerSettings
                {
                    Converters = new List<JsonConverter> { new StringEnumConverter() },
                };

                var stateConfigDict = JsonConvert.DeserializeObject<Dictionary<SwSystemState.EGameState, int>>(_aliveEventIntervalsRawConfig, settings);

                _configDict.SwMerge(true, stateConfigDict);
            }
            catch (Exception e)
            {
                SwInfra.Logger.Log(EWisdomLogType.Alive, e.Message);
            }
        }

        private void SendAliveEvent()
        {
            SwInfra.Logger.Log(EWisdomLogType.Alive);
            
            var data = _measurementProvider.GetDataAndRestart();
            _tracker.TrackEventWithParams(ALIVE_EVENT_NAME, new SwJsonDictionary(data));
        }

        private void RestartCoroutine(int currentInterval)
        {
            SwInfra.Logger.Log(EWisdomLogType.Alive, $"currentInterval = {currentInterval}");
            
            if (currentInterval < 0) return;
            
            if (_recurringAliveSenderCoroutine != null)
            {
                _mono.StopCoroutine(_recurringAliveSenderCoroutine);
                _recurringAliveSenderCoroutine = null;
            }

            _recurringAliveSenderCoroutine = _mono.RunActionEndlessly(SendAliveEvent, currentInterval, currentInterval, () => false);
        }

        #endregion
    }
}
#endif