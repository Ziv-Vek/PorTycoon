using System;
using System.Collections;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    /// <summary>
    /// Note: Although it is natural that <seealso cref="SwCoreMonoBehaviour"/> will be
    /// the first to get `Update()` events, we want to ensure that it will be the earliest notified
    /// component in Wisdom. So we forced the order prioritization with <seealso cref="DefaultExecutionOrder"/>,
    /// this object should be the first to get notified.
    ///
    /// For example: <seealso cref="SupersonicWisdomSDK.SwNotificationsManager"/> should operate before all <seealso cref="SupersonicWisdomSDK.SwTimer"/> objects do.
    /// </summary>
    [DefaultExecutionOrder(0)]
    internal abstract class SwCoreMonoBehaviour : MonoBehaviour, ISwMonoBehaviour
    {
        #region --- Events ---

        internal event Action AwakeEvent;
        internal event Action StartEvent;
        internal event Action UpdateEvent;
        internal event Action ApplicationQuitEvent;
        public event Action<bool> ApplicationPausedEvent;
        public event Action<bool> ApplicationFocusEvent;

        #endregion


        #region --- Members ---

        private SwDrawGui _swDrawGui;
        private static bool _hasInstance;
        private ISwScriptLifecycleListener _lifecycleListener;
        private readonly SwMainThreadActionsQueue _mainThreadActionsQueue = new SwMainThreadActionsQueue();

        #endregion


        #region --- Properties ---

        public GameObject GameObject
        {
            get
            {
                return gameObject;
            }
        }

        internal ISwScriptLifecycleListener LifecycleListener
        {
            set { _lifecycleListener = value; }
            get
            {
                // Make sure that even if the game developer manually adds our Prefab - the SDK flow won't get compromise.
                if (!(_lifecycleListener is ISwContainer))
                {
                    throw new SwException("SupersonicWisdom's main game object wasn't initialized properly! It should be added automatically after calling `SupersonicWisdom.Api.Initialize();`. Did you accidentally add it manually to one of your scenes? If so, please remove it.");
                }

                return _lifecycleListener;
            }
        }

        #endregion
        
        
        #region --- Private Methods ---

        private static IEnumerator RunActionAfterDelay(float delaySeconds, Action action)
        {
            if (delaySeconds > 0)
            {
                yield return new WaitForSeconds(delaySeconds);
            }
            
            action?.Invoke();
        }

        private static IEnumerator RunActionEndlessly(float secondsInterval, Action action, float startDelaySeconds, Func<bool> exitCondition)
        {
            if (startDelaySeconds > 0)
            {
                yield return new WaitForSeconds(startDelaySeconds);
            }
            
            var waitForSeconds = new WaitForSeconds(secondsInterval);
            
            while (!exitCondition())
            {
                action?.Invoke();

                yield return waitForSeconds;
            }
        }
        
        #endregion
        
        
        #region --- Public Methods ---
        
        public new T GetComponent<T>()
        {
            var component = base.GetComponent<T>();

            if (component == null)
            {
                throw new Exception($"{this} Does not have a component of type {typeof(T).FullName}");
            }

            return component;
        }

        /// <summary>
        ///     Run actions on main thread.
        ///     A notable caveat is that you can use it only when Unity is not in focus.
        ///     Otherwise, it can crash the app due to a lock applied on Main Thread
        /// </summary>
        /// <param name="action"></param>
        public void RunOnMainThread(Action action)
        {
            if (SwUtils.System.IsRunningOnMainThread)
            {
                action.Invoke();

                return;
            }

            _mainThreadActionsQueue.Add(action);
        }

        public Coroutine RunActionEndlessly(Action action, float intervalSeconds, Func<bool> exitCondition)
        {
            return RunActionEndlessly(action, intervalSeconds, 0, exitCondition);
        }
        
        public Coroutine RunActionEndlessly(Action action, float intervalSeconds, float startDelaySeconds, Func<bool> exitCondition)
        {
            return StartCoroutine(RunActionEndlessly(intervalSeconds, action, startDelaySeconds, exitCondition));
        }

        public new void StopCoroutine(Coroutine coroutine)
        {
            if (coroutine == null) return;

            base.StopCoroutine(coroutine);
        }

        #endregion


        #region MonoBehavior Implementation

        private void Awake()
        {
            if (_hasInstance)
            {
                Destroy(gameObject);

                return;
            }

            _hasInstance = true;

            DontDestroyOnLoad(gameObject);
            LifecycleListener.OnAwake();
            AwakeEvent?.Invoke();
        }

        private void Start()
        {
            if (!_hasInstance) return;
            
            LifecycleListener.OnStart();
            StartEvent?.Invoke();
        }

        private void Update()
        {
            if (!_hasInstance) return;

            _mainThreadActionsQueue.Run();
            LifecycleListener.OnUpdate();
            UpdateEvent?.Invoke();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (!_hasInstance) return;
            
            LifecycleListener.OnApplicationPause(pauseStatus);
            ApplicationPausedEvent?.Invoke(pauseStatus);
        }

        private void OnApplicationQuit()
        {
            if (!_hasInstance) return;
            
            LifecycleListener.OnApplicationQuit();
            ApplicationQuitEvent?.Invoke();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!_hasInstance) return;
            
            ApplicationFocusEvent?.Invoke(hasFocus);
        }

        private void OnDestroy()
        {
            _hasInstance = false;
        }
        
        private void OnGUI()
        {
            
#if DEVELOPMENT_BUILD
            _swDrawGui ??= new SwDrawGui();
            _swDrawGui.ShowLoadDevelopmentWaterMark();
#endif
            
        }
        
        #endregion
    }
}