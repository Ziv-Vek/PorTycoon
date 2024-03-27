using System;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    internal interface ISwMainThreadRunner
    {
        #region --- Public Methods ---

        void RunOnMainThread([NotNull] Action action);

        #endregion
    }
}