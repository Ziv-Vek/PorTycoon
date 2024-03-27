using System;
using System.Collections;

namespace SupersonicWisdomSDK
{
    internal delegate void SwAsyncCallbackWithException(Exception e);

    internal interface ISwAsyncCatchableRunnable
    {
        #region --- Public Methods ---

        IEnumerator Run(SwAsyncCallbackWithException callback = null);

        #endregion
    }
}