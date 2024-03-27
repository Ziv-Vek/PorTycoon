using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwBlockingApiHandler
    {
        #region --- Members ---

        /// <summary>
        ///     The current running blocking api invocation as string
        ///     e.g "NotifyLevelCompleted(10, \"level_10\")"
        /// </summary>
        [PublicAPI]
        protected string CurrentRunningBlockingApiInvocation;

        /// <summary>
        ///     middlewares wre running serially on each blocking api call
        ///     Each one perform an async operation that suspend the blocking api call
        /// </summary>
        private readonly ISwBlockingApiMiddleware[] _middlewares;

        private readonly Lazy<SwBlockingSimulator> _lazySwBlockingSimulator = new Lazy<SwBlockingSimulator>(() => UnityEngine.Object.Instantiate(Resources.Load("Core/Simulators/SwBlockingSimulator", typeof(SwBlockingSimulator))) as SwBlockingSimulator);
        private readonly SwSettings _settings;
        private readonly SwGameStateSystem _swGameStateSystem;
        
        #endregion


        #region --- Properties ---

        /// <summary>
        ///     An interface for showing the blocking api invocation as text in a popup
        ///     Relevant only when _settings.testBlockingApiInvocation == true
        /// </summary>
        private SwBlockingSimulator BlockingSimulator
        {
            get { return _lazySwBlockingSimulator.Value; }
        }

        private bool CanNotifyRewardedVideoOpportunityMissed
        {
            get => !_settings.isTimeBased && !_swGameStateSystem.IsDuringLevel;
        }
        
        #endregion


        #region --- Construction ---

        internal SwBlockingApiHandler(SwSettings settings, SwGameStateSystem swGameStateSystem, ISwBlockingApiMiddleware[] blockingApiMiddlewares)
        {
            _settings = settings;
            _swGameStateSystem = swGameStateSystem;
            _middlewares = blockingApiMiddlewares ?? new ISwBlockingApiMiddleware[] { };
        }

        #endregion


        #region --- Public Methods ---

        public void NotifyTimeBasedGameStarted(Action action)
        {
            SwInfra.CoroutineService.StartCoroutineWithCallback(NotifyTimeBasedGameStarted, action);
        }

        public IEnumerator NotifyTimeBasedGameStarted()
        {
            SwInfra.Logger.Log(EWisdomLogType.IntegrationTool);
            
            var waitForBlockerClose = Lock($"NotifyTimeBasedGameStarted()");
            
            _swGameStateSystem.ProcessTimeBasedGameStarted();

            foreach (var middleware in _middlewares)
            {
                yield return middleware.ProcessTimeBasedGameStarted();
            }

            yield return Unlock(waitForBlockerClose);
        }
        
        public IEnumerator PrepareForGameStarted()
        {
            foreach (var middleware in _middlewares)
            {
                yield return middleware.PrepareForGameStarted();
            }
        }

        public IEnumerator NotifyLevelCompleted(ESwLevelType? levelType, long level, string customString)
        {
            var waitForBlockerClose = Lock($"NotifyLevelCompleted({level}, \"{customString}\")");
            // if levelType is null, we assume it's a regular level
            var swLevelType = levelType ?? ESwLevelType.Regular;
            
            SwInfra.Logger.Log(EWisdomLogType.IntegrationTool, $"{swLevelType} {level}");

            _swGameStateSystem.ProcessLevelCompleted(swLevelType, level, customString);

            foreach (var middleware in _middlewares)
            {
                yield return middleware.ProcessLevelCompleted();
            }

            yield return Unlock(waitForBlockerClose);
        }
        
        public void NotifyLevelCompleted(ESwLevelType? levelType, long level, Action action, string customString)
        {
            SwInfra.CoroutineService.StartCoroutineWithCallback(() => NotifyLevelCompleted(levelType, level, customString), action);
        }

        public IEnumerator NotifyLevelFailed(ESwLevelType? levelType, long level, string customString)
        {
            var waitForBlockerClose = Lock($"NotifyLevelFailed({level}, \"{customString}\")");
            // if levelType is null, we assume it's a regular level
            var swLevelType = levelType ?? ESwLevelType.Regular;
            
            SwInfra.Logger.Log(EWisdomLogType.IntegrationTool, $"{swLevelType} {level}");

            _swGameStateSystem.ProcessLevelFailed(swLevelType, level, customString);

            foreach (var middleware in _middlewares)
            {
                yield return middleware.ProcessLevelFailed();
            }

            yield return Unlock(waitForBlockerClose);
        }

        public void NotifyLevelFailed(ESwLevelType? levelType, long level, Action action, string customString)
        {
            SwInfra.CoroutineService.StartCoroutineWithCallback(() => NotifyLevelFailed(levelType, level, customString), action);
        }

        public IEnumerator NotifyLevelRevived(ESwLevelType? levelType, long level, string customString)
        {
            var waitForBlockerClose = Lock($"NotifyLevelRevived({level}, \"{customString}\")");
            // if levelType is null, we assume it's a regular level
            var swLevelType = levelType ?? ESwLevelType.Regular;
            
            SwInfra.Logger.Log(EWisdomLogType.IntegrationTool, $"{swLevelType} {level}");

            _swGameStateSystem.ProcessLevelRevived(swLevelType, level, customString);

            foreach (var middleware in _middlewares)
            {
                yield return middleware.ProcessLevelRevived();
            }

            yield return Unlock(waitForBlockerClose);
        }

        public void NotifyLevelRevived(ESwLevelType? levelType, long level, Action action,  string customString)
        {
            SwInfra.CoroutineService.StartCoroutineWithCallback(() => NotifyLevelRevived(levelType, level, customString), action);
        }
        
        public IEnumerator NotifyLevelSkipped(ESwLevelType? levelType, long level, string customString)
        {
            var waitForBlockerClose = Lock($"NotifyLevelSkipped({level}, \"{customString}\")");
            // if levelType is null, we assume it's a regular level
            var swLevelType = levelType ?? ESwLevelType.Regular;
            
            SwInfra.Logger.Log(EWisdomLogType.IntegrationTool, $"{swLevelType} {level}");

            _swGameStateSystem.ProcessLevelSkipped(swLevelType, level, customString);

            foreach (var middleware in _middlewares)
            {
                yield return middleware.ProcessLevelSkipped();
            }

            yield return Unlock(waitForBlockerClose);
        }

        public void NotifyLevelSkipped(ESwLevelType? levelType, long level, Action action, string customString)
        {
            SwInfra.CoroutineService.StartCoroutineWithCallback(() => NotifyLevelSkipped(levelType, level, customString), action);
        }

        public IEnumerator NotifyLevelStarted(ESwLevelType? levelType, long level, string customString)
        {
            var waitForBlockerClose = Lock($"NotifyLevelStarted({level}, \"{customString}\")");
            // if levelType is null, we assume it's a regular level
            var swLevelType = levelType ?? ESwLevelType.Regular;
            
            SwInfra.Logger.Log(EWisdomLogType.IntegrationTool, $"{swLevelType} {level}");

            _swGameStateSystem.ProcessLevelStarted(swLevelType, level, customString);

            foreach (var middleware in _middlewares)
            {
                yield return middleware.ProcessLevelStarted();
            }

            yield return Unlock(waitForBlockerClose);
        }

        public void NotifyLevelStarted(ESwLevelType? levelType, long level, Action action, string customString)
        {
            SwInfra.CoroutineService.StartCoroutineWithCallback(() => NotifyLevelStarted(levelType, level, customString), action);
        }

        public void NotifyRewardedVideoOpportunityMissedAfterLevelEnd(Action action)
        {
            SwInfra.CoroutineService.StartCoroutineWithCallback(NotifyRewardedVideoOpportunityMissedAfterLevelEnd, action);
        }


        public IEnumerator NotifyRewardedVideoOpportunityMissedAfterLevelEnd()
        {
            if (!CanNotifyRewardedVideoOpportunityMissed)
            {
                SwInfra.Logger.LogWarning(EWisdomLogType.BlockingApi, "SwBlockingApiHandler | Can only notify opportunity missed if game is level based and level has ended");
                yield break;
            }
            
            var waitForBlockerClose = Lock("RewardedVideoOpportunityMissed()");

            foreach (var middleware in _middlewares)
            {
                yield return middleware.ProcessRewardedVideoOpportunityMissed();
            }

            yield return Unlock(waitForBlockerClose);
        }

        public void NotifyMetaStarted (Action action, string customString = "")
        {
            SwInfra.CoroutineService.StartCoroutineWithCallback(() => NotifyMetaStarted(customString), action);
        }

        private IEnumerator NotifyMetaStarted (string customString)
        {
            SwInfra.Logger.Log(EWisdomLogType.IntegrationTool);
            
            var waitForBlockerClose = Lock("NotifyMetaStarted()");
            
            _swGameStateSystem.ProcessMetaStarted(customString);

            foreach (var middleware in _middlewares)
            {
                yield return middleware.ProcessMetaStarted();
            }

            yield return Unlock(waitForBlockerClose);
        }
        
        public void NotifyMetaEnded (Action action, string customString = "")
        {
            SwInfra.CoroutineService.StartCoroutineWithCallback(() => NotifyMetaEnded(customString), action);
        }

        private IEnumerator NotifyMetaEnded (string customString)
        {
            SwInfra.Logger.Log(EWisdomLogType.IntegrationTool);
            
            var waitForBlockerClose = Lock("NotifyMetaEnded()");
            
            _swGameStateSystem.ProcessMetaEnded(customString);

            foreach (var middleware in _middlewares)
            {
                yield return middleware.ProcessMetaEnded();
            }

            yield return Unlock(waitForBlockerClose);
        }

        #endregion


        #region --- Private Methods ---

        /// <summary>
        ///     This methods is run upon every blocking api invocation
        ///     It sets the singular CurrentRunningBlockingApiInvocation with new value
        /// </summary>
        /// <param name="blockingApiInvocation"></param>
        /// <returns>a function which returns an IEnumerator that ends when the blocking popup is closed</returns>
        private SwAsyncMethod Lock(string blockingApiInvocation)
        {
            var error = "";

            if (!string.IsNullOrEmpty(CurrentRunningBlockingApiInvocation))
            {
                // In this case another blocking API is still running, produce a human-readable error
                error = $"Blocking API Error: {blockingApiInvocation} was called before {CurrentRunningBlockingApiInvocation} was resolved";
                SwInfra.Logger.LogError(EWisdomLogType.BlockingApi, $"SwBlockingApiHandler | {error}");

#if DEVELOPMENT_BUILD
                // If the game was build in development mode, crash the game so that the developer will notice the issue.
                Application.Quit(1);
#endif

                // In case _settings.testBlockingApiInvocation = true
                // The current popup is hidden and a new popup with the error is shown right after that.
                if (_settings.testBlockingApiInvocation)
                {
                    BlockingSimulator.Hide();
                }
            }

            CurrentRunningBlockingApiInvocation = blockingApiInvocation;

            if (!_settings.testBlockingApiInvocation) return null;

            BlockingSimulator.Closable = false;

            // returns an async function (IEnumerator), that resolves when the popup is closed.
            return BlockingSimulator.Show(CurrentRunningBlockingApiInvocation, error);
        }

        /// <summary>
        ///     Resets CurrentRunningBlockingApiInvocation member
        ///     Waits for blocking simulator popup to close
        /// </summary>
        /// <param name="waitForBlockerClose">an async function (IEnumerator), that resolves when the popup is closed</param>
        /// <returns></returns>
        private IEnumerator Unlock(SwAsyncMethod waitForBlockerClose)
        {
            CurrentRunningBlockingApiInvocation = "";

            if (!_settings.testBlockingApiInvocation) yield break;

            BlockingSimulator.Closable = true;

            yield return waitForBlockerClose();
        }
        

        #endregion
    }
}