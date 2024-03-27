using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SupersonicWisdomSDK
{
    internal delegate IEnumerator SwAsyncMethod ();

    internal class SwCoroutineService
    {
        #region --- Members ---

        private readonly MonoBehaviour _runner;

        #endregion


        #region --- Construction ---

        public SwCoroutineService(MonoBehaviour runner)
        {
            _runner = runner;
        }

        #endregion


        #region --- Public Methods ---

        public IEnumerator Try(IEnumerator enumerator, SwAsyncCallbackWithException callback)
        {
            while (true)
            {
                object current;

                try
                {
                    if (enumerator.MoveNext() == false)
                    {
                        callback.Invoke(null);

                        break;
                    }

                    current = enumerator.Current;
                }
                catch (Exception ex)
                {
                    callback?.Invoke(ex);

                    yield break;
                }

                yield return current;
            }
        }

        public IEnumerator RunAllInParallel(SwAsyncMethod[] coroutineMethods, SwAsyncCallbackWithException callback)
        {
            var didFail = false;
            Exception exception = null;
            var counter = coroutineMethods.Length;

            if (counter == 0)
            {
                yield break;
            }

            foreach (var coroutineMethod in coroutineMethods)
            {
                StartCoroutineWithCallback(coroutineMethod, ex =>
                {
                    if (ex == null)
                    {
                        counter--;
                    }
                    else
                    {
                        didFail = true;
                        exception = ex;
                    }
                });
            }

            while (counter > 0 && !didFail)
            {
                yield return null;
            }

            callback?.Invoke(exception);
        }

        public Coroutine RunThrottledForever(Action callback, int frameInterval = 10)
        {
            return _runner.StartCoroutine(RunThrottledForeverCoroutine(callback, frameInterval));
        }

        public Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return _runner.StartCoroutine(coroutine);
        }

        public Coroutine StartCoroutineWithCallback(SwAsyncMethod getCoroutine, Action callback)
        {
            var startedCoroutine = StartCoroutine(StartCoroutineAndRunCallback(getCoroutine.Invoke(), callback));

            return startedCoroutine;
        }

        public Coroutine StartCoroutineWithCallback(SwAsyncMethod getCoroutine, SwAsyncCallbackWithException callback)
        {
            var startedCoroutine = StartCoroutine(StartCoroutineAndRunCallback(getCoroutine.Invoke(), callback));

            return startedCoroutine;
        }

        public void StopCoroutine(Coroutine coroutine)
        {
            try
            {
                _runner.StopCoroutine(coroutine);
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError(EWisdomLogType.CoroutineService, e.Message);
            }
        }
        
        public Coroutine RunActionEndlessly(Action action, float secondsInterval, Func<bool> exitCondition)
        {
            return _runner.StartCoroutine(RunEndlessly(action, secondsInterval, exitCondition));
        }
        
        #endregion


        #region --- Private Methods ---

        private static IEnumerator StartCoroutineAndRunCallback(IEnumerator coroutine, Action callback)
        {
            yield return coroutine;
            callback?.Invoke();
        }

        private IEnumerator StartCoroutineAndRunCallback(IEnumerator coroutine, SwAsyncCallbackWithException callback)
        {
            yield return Try(coroutine, callback);
        }

        private IEnumerator WaitForSecondsInternal(float seconds, [NotNull] Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback.Invoke();
        }

        internal Coroutine WaitForSeconds(float seconds, [NotNull] Action action)
        {
            return StartCoroutine(WaitForSecondsInternal(seconds, action));
        }

        private IEnumerator RunThrottledForeverCoroutine(Action callback, int frameInterval = 10)
        {
            do
            {
                callback.Invoke();

                for (var i = 0; i < frameInterval; i++)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            while (true);
            // ReSharper disable once IteratorNeverReturns
        }
        
        private IEnumerator RunEndlessly(Action action, float secondsInterval, Func<bool> exitCondition)
        {
            var waitForSeconds = new WaitForSeconds(secondsInterval);
            
            while (!exitCondition())
            {
                action?.Invoke();

                yield return waitForSeconds;
            }
        }

        #endregion
    }
}