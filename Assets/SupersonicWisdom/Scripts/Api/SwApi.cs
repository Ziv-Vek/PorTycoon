using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    public class SwApi
    {
        #region --- Members ---

        private static bool _hasContainer;
        private static ISwContainer _container;
        private readonly List<Action> _deferredActions = new List<Action>();

        #endregion


        #region --- Properties ---

        internal static ISwContainer Container
        {
            get
            {
                if (!_hasContainer)
                {
                    throw new SwException("You must call `SupersonicWisdom.Api.Initialize();` first.");
                }

                return _container;
            }
            
            

            private set { _container = value; }
        }

        internal bool WasInitialized { get; private set; }

        #endregion


        #region --- Public Methods ---

        public void Initialize(Dictionary<string, object> initParams = default)
        {
            if (_hasContainer)
            {
                Debug.LogWarning("SupersonicWisdom: The method `SupersonicWisdom.Api.Initialize();` was called more than once!");

                return;
            }

            Container = SwContainerUtils.CreateContainer(SwStageUtils.CurrentStageName, initParams);
            _hasContainer = true;
            WasInitialized = true;

            var validationErrors = Container.Validate();

            if (validationErrors.Any())
            {
                SwInfra.Logger.LogError(EWisdomLogType.API, $"Integration with SupersonicWisdomSDK is not complete. Fix the following issues: {string.Join(";", validationErrors)}");
                Application.Quit(1);
            }

            RunAllDeferredActions();

            SwContainerUtils.SetupContainer(Container, SwConstants.GAME_OBJECT_NAME);
        }

        #endregion


        #region --- Private Methods ---

        private static void AssertContainer(string fnName)
        {
            if (!_hasContainer)
            {
                throw new SwException($"SupersonicWisdom.Api.{fnName} cannot be applied before SupersonicWisdom.Api.Initialize is applied");
            }
        }

        protected internal void AssertContainerIsReady(string fnName)
        {
            AssertContainer(fnName);

            if (!Container.IsReady)
            {
                throw new SwException($"SupersonicWisdom.Api.{fnName} cannot be applied before SupersonicWisdomSDK is ready. Use SupersonicWisdom.Api.AddOnReadyListener to attach SDK readiness listener");
            }
        }

        internal void ForceQuit(string errorMessage)
        {
            LogInvocationError($"The app will quit due to an error: {errorMessage}");
            Application.Quit(1);
        }

        internal T GetWithContainerOrDefault<T>([NotNull] Func<T> getterFn, T defaultValue)
        {
            if (!_hasContainer)
            {
                return defaultValue;
            }

            return getterFn();
        }

        internal T GetWithContainerOrThrow<T>([NotNull] Func<T> getterFn, string fnName)
        {
            AssertContainerIsReady(fnName);

            return getterFn();
        }

        internal void LogInvocationError(string message)
        {
            Debug.LogError($"SupersonicWisdom: An API invocation error has occurred! {message}");
        }

        internal void LogInvocationWarning(string message)
        {
            Debug.LogWarning($"SupersonicWisdom: API invocation warning! {message}");
        }

        internal IEnumerator RunAsyncWithContainerOrThrow([NotNull] SwAsyncMethod asyncMethod, string fnName)
        {
            AssertContainerIsReady(fnName);

            yield return asyncMethod.Invoke();
        }

        internal void RunWhenContainerIsReady([NotNull] Action action)
        {
            if (_hasContainer)
            {
                action.Invoke();
            }
            else
            {
                _deferredActions.Add(action);
            }
        }

        internal void RunWithContainerAndReadyOrThrow([NotNull] Action action, string fnName)
        {
            AssertContainerIsReady(fnName);
            action.Invoke();
        }
        
        internal void RunWithContainerCreatedOrThrow([NotNull] Action action, string fnName)
        {
            AssertContainer(fnName);
            action.Invoke();
        }

        internal void Teardown ()
        {
            UnityEngine.Object.Destroy(_container.GetMono().gameObject);
            _container.Destroy();
            _container = null;
            _hasContainer = false;
        }

        private void RunAllDeferredActions ()
        {
            foreach (var action in _deferredActions)
            {
                action.Invoke();
            }

            _deferredActions.Clear();
        }

        #endregion
    }
}