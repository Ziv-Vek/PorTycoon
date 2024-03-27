using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    /// <summary>
    ///     Imposes a limit on the maximum number of coroutines that can be running at any given time. Runs
    ///     coroutines until the limit is reached and then begins queueing coroutines instead. When
    ///     coroutines finish, queued coroutines are run.
    /// </summary>
    public class SwCoroutineQueue
    {
        #region --- Members ---

        /// <summary>
        ///     Delegate to start coroutines with
        /// </summary>
        private readonly Func<IEnumerator, Coroutine> _coroutineStarter;

        /// <summary>
        ///     Maximum number of coroutines to run at once
        /// </summary>
        private readonly int _maxActive;

        /// <summary>
        ///     Queue of coroutines waiting to start
        /// </summary>
        private readonly Queue<IEnumerator> queue;

        /// <summary>
        ///     Number of currently active coroutines
        /// </summary>
        private uint _numActive;

        #endregion


        #region --- Construction ---

        /// <summary>
        ///     Create the queue, initially with no coroutines
        /// </summary>
        /// <param name="maxActive">
        ///     Maximum number of coroutines to run at once. This must be at least one.
        /// </param>
        /// <param name="coroutineStarter">
        ///     Delegate to start coroutines with. Normally you'd pass
        ///     <see cref="MonoBehaviour.StartCoroutine" /> for this.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     If maxActive is zero.
        /// </exception>
        public SwCoroutineQueue(int maxActive, Func<IEnumerator, Coroutine> coroutineStarter)
        {
            if (maxActive == 0)
            {
                throw new ArgumentException("Must be at least one", "maxActive");
            }

            _maxActive = maxActive;
            _coroutineStarter = coroutineStarter;
            queue = new Queue<IEnumerator>();
        }

        #endregion


        #region --- Public Methods ---

        /// <summary>
        ///     If the number of active coroutines is under the limit specified in the constructor, run the
        ///     given coroutine. Otherwise, queue it to be run when other coroutines finish.
        /// </summary>
        /// <param name="coroutine">Coroutine to run or queue</param>
        public void Run(IEnumerator coroutine)
        {
            if (_numActive < _maxActive)
            {
                var runner = CoroutineRunner(coroutine);
                _coroutineStarter(runner);
            }
            else
            {
                SwInfra.Logger.Log(EWisdomLogType.Utils, "enqueue coroutine");
                queue.Enqueue(coroutine);
            }
        }

        #endregion


        #region --- Private Methods ---

        /// <summary>
        ///     Runs a coroutine then runs the next queued coroutine (via <see cref="Run" />) if available.
        ///     Increments <see cref="_numActive" /> before running the coroutine and decrements it after.
        /// </summary>
        /// <returns>Values yielded by the given coroutine</returns>
        /// <param name="coroutine">Coroutine to run</param>
        private IEnumerator CoroutineRunner(IEnumerator coroutine)
        {
            _numActive++;
            SwInfra.Logger.Log(EWisdomLogType.Utils, $"active coroutines {_numActive}");

            while (coroutine.MoveNext())
            {
                yield return coroutine.Current;
            }

            _numActive--;
            SwInfra.Logger.Log(EWisdomLogType.Utils, $"active coroutines {_numActive}");

            if (queue.Count > 0)
            {
                var next = queue.Dequeue();
                Run(next);
                SwInfra.Logger.Log(EWisdomLogType.Utils, "dequeue coroutine");
            }
        }

        #endregion
    }
}