using System;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal interface ISwMonoBehaviour : ISwMainThreadRunner
    {
        #region --- Events ---

        public event Action<bool> ApplicationPausedEvent;
        public event Action<bool> ApplicationFocusEvent;

        #endregion
        
        
        #region --- Properties ---

        GameObject GameObject { get; }

        #endregion


        #region --- Public Methods ---

        public Coroutine RunActionEndlessly(Action action, float intervalSeconds, float startDelaySeconds, Func<bool> exitCondition);
        public Coroutine RunActionEndlessly(Action action, float intervalSeconds, Func<bool> exitCondition);
        public void StopCoroutine(Coroutine coroutine);

        #endregion
    }
}