using System;
using System.Collections;

namespace SupersonicWisdomSDK
{
    internal interface ISwAsyncRunnable
    {
        #region --- Public Methods ---

        IEnumerator Run ();

        #endregion
    }
}