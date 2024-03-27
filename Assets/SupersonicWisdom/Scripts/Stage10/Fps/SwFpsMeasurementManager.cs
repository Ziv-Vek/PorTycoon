#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwCoreFpsMeasurementManager : ISwCoreConfigListener, ISwLocalConfigProvider
    {
        #region --- Constants ---
        
        private const string INVALID_FPS_EVENT = "InvalidFpsEvent";

        #endregion
        

        #region --- Members ---

        private readonly ISwMonoBehaviour _mono;
        private readonly SwStage10Tracker _tracker;
        private readonly SwTimerManager _timerManager;
        private readonly Dictionary<Guid, ISwFpsMonitor> _dict;

        private int _measurementsInterval =  SwFpsMonitorLocalConfig.FPS_MEASUREMENTS_INTERVAL_VALUE;
        private int _criticalThreshold = SwFpsMonitorLocalConfig.FPS_CRITICAL_THRESHOLD_VALUE;
        private bool _shouldReportInvalidMeasurement = SwFpsMonitorLocalConfig.FPS_SHOULD_REPORT_INVALID_FPS_VALUE;

        #endregion


        #region --- Properties ---

        public Tuple<EConfigListenerType, EConfigListenerType> ListenerType
        {
            get { return new Tuple<EConfigListenerType, EConfigListenerType>(EConfigListenerType.FinishWaitingForRemote, EConfigListenerType.GameStarted); }
        }

        #endregion


        #region --- Construction ---

        public SwCoreFpsMeasurementManager(ISwMonoBehaviour mono, SwTimerManager timerManager, SwStage10Tracker tracker)
        {
            _mono = mono;
            _timerManager = timerManager;
            _tracker = tracker;
            _dict = new Dictionary<Guid, ISwFpsMonitor>();
        }

        #endregion


        #region --- Public Methods ---

        public Guid Start()
        {
            var fpsProducer = new SwStage10FpsMonitor(_mono, _timerManager, _measurementsInterval, _criticalThreshold, OnInvalidFps);
            var newGuid = Guid.NewGuid();
            
            _dict[newGuid] = fpsProducer;

            return newGuid;
        }

        private void OnInvalidFps(int elapsedFrames, float elapsedTime, int intFps)
        {
            if (!_shouldReportInvalidMeasurement) return;
            
            _tracker.TrackEvent(INVALID_FPS_EVENT, elapsedFrames.SwToString(), elapsedTime.SwToString(), intFps.SwToString());
        }

        public void OnConfigResolved(ISwCoreInternalConfig configAccessor, ISwConfigManagerState state)
        {
            _measurementsInterval = configAccessor.GetValue(SwFpsMonitorLocalConfig.FPS_MEASUREMENTS_INTERVAL_KEY, SwFpsMonitorLocalConfig.FPS_MEASUREMENTS_INTERVAL_VALUE);
            _criticalThreshold = configAccessor.GetValue(SwFpsMonitorLocalConfig.FPS_CRITICAL_THRESHOLD_KEY, SwFpsMonitorLocalConfig.FPS_CRITICAL_THRESHOLD_VALUE);
            _shouldReportInvalidMeasurement = configAccessor.GetValue(SwFpsMonitorLocalConfig.FPS_SHOULD_REPORT_INVALID_FPS_KEY, SwFpsMonitorLocalConfig.FPS_SHOULD_REPORT_INVALID_FPS_VALUE);
        }

        public SwLocalConfig GetLocalConfig()
        {
            return new SwFpsMonitorLocalConfig();
        }

        public FpsMeasurementSummary Measure(Guid guid)
        {
            return !_dict.SwContains(guid) ? null : _dict[guid].Measure();
        }

        public void Stop(Guid guid)
        {
            if (!_dict.SwContains(guid)) return;

            _dict[guid].Stop();
            _dict.Remove(guid);
        }

        public bool IsMeasuring(Guid guid)
        {
            return _dict.SwContains(guid) && _dict[guid].IsCollecting;
        }

        #endregion
    }
}
#endif