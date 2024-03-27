#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwStage10FpsMonitor : ISwFpsMonitor
    {
        #region --- Events ---
        
        // Stand for Elapsed frames(int), elapsed time (float), fps (int)
        private readonly Action<int, float, int> InvalidFpsEvent;
        
        #endregion
        
        
        #region --- Members ---

        private readonly ISwMonoBehaviour _coreMonoBehaviour;
        private readonly Queue<FpsSample> _sampleHistory = new Queue<FpsSample>();
        private readonly SwTimerManager _timerManager;

        private bool _isEnabled;
        private Coroutine _collectSamplesCoroutine;
        private float _sampleStartTime;
        private int _sampleStartFrame;
        private readonly int _measurementsInterval;
        private readonly int _criticalThreshold;
        private SwBaseAnalyticsManager _previousDataProvider;

        #endregion


        #region --- Properties ---

        public bool IsCollecting
        {
            get { return _collectSamplesCoroutine != null; }
        }

        private List<FpsSample> GetSamples
        {
            get { return _sampleHistory.ToList(); }
        }

        #endregion


        #region --- Construction ---

        public SwStage10FpsMonitor(ISwMonoBehaviour mono, SwTimerManager timerManager, int measurementsInterval, int criticalThreshold, Action<int, float, int> onInvalidFpsEvent)
        {
            _coreMonoBehaviour = mono;
            _timerManager = timerManager;
            _measurementsInterval = measurementsInterval;
            _criticalThreshold = criticalThreshold;
            InvalidFpsEvent += onInvalidFpsEvent;

            Start();
        }

        ~SwStage10FpsMonitor()
        {
            if (_collectSamplesCoroutine != null)
            {
                _coreMonoBehaviour.RunOnMainThread(Reset);
            }
        }

        #endregion


        #region --- Mono Override ---

        public void Start()
        {
            if (_measurementsInterval <= 0) return;
            
            if (IsCollecting)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Fps, "Already measuring FPS...");

                return;
            }

            _sampleHistory.Clear();
            ResetSample();
            
            SwInfra.Logger.Log(EWisdomLogType.Fps, "Started measuring");
            _collectSamplesCoroutine = _coreMonoBehaviour.RunActionEndlessly(CollectSample, _measurementsInterval, _measurementsInterval, () => false);
        }

        private void ResetSample()
        {
            _sampleStartFrame = Time.frameCount;
            _sampleStartTime = _timerManager.CurrentSessionPlaytimeNetoStopWatch.Elapsed;
        }

        #endregion


        #region --- Public Methods ---

        public FpsMeasurementSummary Measure()
        {
            if (!IsCollecting)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Fps, $"{nameof(SwStage10FpsMonitor)} | Called {nameof(Measure)} while not running.");

                return null;
            }

            CollectSample();
            
            return CreateSummary();
        }

        public void Stop()
        {
            if (!IsCollecting)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Fps, $"{nameof(SwStage10FpsMonitor)} | Called {nameof(Stop)} while not running.");

                return;
            }

            Reset();
            SwInfra.Logger.Log(EWisdomLogType.Fps, "Stopped measuring...");
        }

        #endregion


        #region --- Private Methods ---

        private void CollectSample()
        {
            var elapsedFrames = Time.frameCount - _sampleStartFrame; // `Update()` invocation count since previous time
            var elapsedTimeTotal = _timerManager.CurrentSessionPlaytimeNetoStopWatch.Elapsed;
            var elapsedTime = elapsedTimeTotal - _sampleStartTime;

            if (elapsedTime == 0)
            {
                InvalidFpsEvent?.Invoke(elapsedFrames, elapsedTime, -1);
                
                return;
            }

            var fps = elapsedFrames / elapsedTime; // Where `elapsedFrames` counts the OnUpdate invocations.
            var intFps = (int)fps;

            if (elapsedFrames == 1 || fps.Equals(float.NaN) || intFps < 0)
            {
                InvalidFpsEvent?.Invoke(elapsedFrames, elapsedTime, intFps);
                
                return;
            }

            var collectedSample = new FpsSample(intFps, elapsedTimeTotal, elapsedTime, elapsedFrames);
            _sampleHistory.Enqueue(collectedSample);

            ResetSample();

            SwInfra.Logger.Log(EWisdomLogType.Fps, $"(`Update` invocation counter) added sample = {collectedSample}");
        }

        private void Reset()
        {
            _coreMonoBehaviour?.StopCoroutine(_collectSamplesCoroutine);
            _collectSamplesCoroutine = null;
            _sampleHistory.Clear();
        }

        private static int GetMedian(List<FpsSample> fpsSamples)
        {
            if (fpsSamples.SwIsEmpty())
            {
                return -1;
            }

            var history = fpsSamples.ConvertAll(sample => sample.FpsValue);

            if (history.Count == 1)
            {
                return history[0];
            }

            history.Sort();

            int median;
            var halfHistoryCount = history.Count / 2;

            if (history.Count % 2 == 0)
            {
                var middle = history[halfHistoryCount] + history[halfHistoryCount - 1];
                median = middle / 2;
            }
            else
            {
                median = history[halfHistoryCount];
            }

            return median;
        }

        private FpsMeasurementSummary CreateSummary()
        {
            var fpsSamples = GetSamples;

            if (fpsSamples.SwIsEmpty())
            {
                SwInfra.Logger.Log(EWisdomLogType.Fps, "No FPS samples...");

                return new FpsMeasurementSummary();
            }

            SwInfra.Logger.Log(EWisdomLogType.Fps, $"All FPS samples: {fpsSamples.SwToString()}");

            var fpsSum = 0;
            float fpsCriticalDuration = 0;
            
            var criticalMinimumFps = int.MaxValue;

            foreach (var sample in fpsSamples)
            {
                var sampleFps = sample.FpsValue;

                if (IsCritical(sampleFps))
                {
                    fpsCriticalDuration += sample.Duration;
                    criticalMinimumFps = Math.Min(criticalMinimumFps, sampleFps);
                }
                
                fpsSum += sampleFps;
            }

            // No Critical min fps
            if (criticalMinimumFps == int.MaxValue)
            {
                criticalMinimumFps = -1;
            }

            var median = GetMedian(fpsSamples);
            var averageFps = fpsSum / fpsSamples.Count;

            var fpsMeasurementSummary = new FpsMeasurementSummary
            {
                averageFps = averageFps,
                medianFps = median,
                samplesCount = fpsSamples.Count,
                fpsMeasurementDuration = fpsSamples.Last().SessionTimestamp - fpsSamples.First().StartTime,
                criticalMinFps = criticalMinimumFps,
                criticalFpsDuration = fpsCriticalDuration,
            };
            
            SwInfra.Logger.Log(EWisdomLogType.Fps, $"All FPS measurement summary: {JsonConvert.SerializeObject(fpsMeasurementSummary)}");

            return fpsMeasurementSummary;
        }

        private bool IsCritical(float fps)
        {
            return fps < _criticalThreshold;
        }

        #endregion
    }
}
#endif