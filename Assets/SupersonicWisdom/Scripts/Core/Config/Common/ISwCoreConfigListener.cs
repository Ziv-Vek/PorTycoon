using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    internal interface ISwCoreConfigListener
    {
        #region --- Properties ---

        // The tuple represent the <start, end> timing which accepted by the listener (inclusive)
        Tuple<EConfigListenerType, EConfigListenerType> ListenerType { get; }

        #endregion


        #region --- Public Methods ---

        /// This method can be invoked more than one time
        void OnConfigResolved([NotNull] ISwCoreInternalConfig swConfigAccessor, ISwConfigManagerState state);

        #endregion
    }

    internal enum EConfigListenerType
    {
        Construction = 0,
        FinishWaitingForRemote = 1,
        GameStarted = 2,
        EndOfGame = 3,
    }

    internal enum EConfigStatus
    {
        NotInitialized,
        Local,
        Cached,
        Remote,
    }
}