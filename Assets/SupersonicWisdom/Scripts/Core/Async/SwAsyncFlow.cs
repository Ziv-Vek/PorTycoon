using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SupersonicWisdomSDK
{
    internal sealed class SwAsyncFlow : ISwAsyncCatchableRunnable
    {
        #region --- Members ---

        private readonly Dictionary<int, int> _runningBackgroundSteps = new Dictionary<int, int>();
        private readonly int _maxStepIndex;
        private readonly List<SwAsyncFlowStep> _steps;
        private int _currentIndex;

        #endregion


        #region --- Construction ---

        public SwAsyncFlow(SwAsyncFlowStep[] steps)
        {
            _steps = steps.OrderBy(step => step.ExecutionIndex).ToList();
            _currentIndex = _steps.FirstOrDefault()?.ExecutionIndex ?? 0;
            _maxStepIndex = _steps.LastOrDefault()?.ExecutionIndex ?? 0;
        }

        #endregion


        #region --- Public Methods ---

        public IEnumerator Run(SwAsyncCallbackWithException callback = null)
        {
            Exception exception = null;
            var didFail = false;

            try
            {
                Validate();
            }
            catch (SwException ex)
            {
                didFail = true;
                exception = ex;
            }

            if (!didFail)
            {
                var steps = new List<SwAsyncFlowStep>(_steps);

                do
                {
                    var stepsToRun = steps.Where(step => step.ExecutionIndex == _currentIndex && step.MaxExecutionIndex == null).ToList();
                    var backgroundStepsToRun = steps.Where(step => step.ExecutionIndex == _currentIndex && step.MaxExecutionIndex != null && step.MaxExecutionIndex > _currentIndex).ToList();
                    steps = steps.Except(stepsToRun.Concat(backgroundStepsToRun)).ToList();

                    if (backgroundStepsToRun.Any())
                    {
                        foreach (var step in backgroundStepsToRun)
                        {
                            // ReSharper disable once PossibleInvalidOperationException
                            var endIndex = (int)step.MaxExecutionIndex;
                            UpdateBackgroundStepCount(endIndex, true);

                            SwInfra.CoroutineService.StartCoroutineWithCallback(step.Runnable.Run, ex =>
                            {
                                if (ex != null)
                                {
                                    didFail = true;
                                    exception = ex;
                                }

                                UpdateBackgroundStepCount(endIndex, false);
                            });
                        }
                    }

                    if (stepsToRun.Any())
                    {
                        var coroutineGetters = stepsToRun.Select(step => (SwAsyncMethod)step.Runnable.Run).ToArray();

                        yield return SwInfra.CoroutineService.RunAllInParallel(coroutineGetters, ex =>
                        {
                            if (ex != null)
                            {
                                didFail = true;
                                exception = ex;
                            }
                        });
                    }

                    while (!didFail && _runningBackgroundSteps.SwSafelyGet(_currentIndex, 0) > 0)
                    {
                        yield return null;
                    }

                    _currentIndex++;
                }
                while (!didFail && steps.Any());
            }

            if (didFail)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Async, $"{nameof(SwAsyncFlow)} | Run | Error");
                SwInfra.Logger.LogError(EWisdomLogType.Async, exception.ToString());
            }

            callback?.Invoke(exception);
        }

        #endregion


        #region --- Private Methods ---

        private void UpdateBackgroundStepCount(int index, bool shouldAdd)
        {
            _runningBackgroundSteps[index] = _runningBackgroundSteps.SwSafelyGet(index, 0) + (shouldAdd ? 1 : -1);

            if (_runningBackgroundSteps[index] == 0)
            {
                _runningBackgroundSteps.Remove(index);
            }
        }

        private void Validate ()
        {
            var stepIndexWithInvalidRange = _steps.FindIndex(step => step.MaxExecutionIndex != null && step.ExecutionIndex >= step.MaxExecutionIndex);

            if (stepIndexWithInvalidRange > -1)
            {
                var invalidStep = _steps[stepIndexWithInvalidRange];

                throw new SwException($"Invalid range for step at index={stepIndexWithInvalidRange} Range={invalidStep.ExecutionIndex}-{invalidStep.MaxExecutionIndex}");
            }

            var stepIndexWithRangeOutOfBounds = _steps.FindIndex(step => step.MaxExecutionIndex > _maxStepIndex);

            if (stepIndexWithRangeOutOfBounds > -1)
            {
                var invalidStep = _steps[stepIndexWithRangeOutOfBounds];

                throw new SwException($"Range out of bounds for step at index={stepIndexWithRangeOutOfBounds} Range={invalidStep.ExecutionIndex}-{invalidStep.MaxExecutionIndex}");
            }
        }

        #endregion
    }
}