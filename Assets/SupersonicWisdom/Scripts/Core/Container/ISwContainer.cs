using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    internal interface ISwContainer : ISwScriptLifecycleListener
    {
        #region --- Properties ---

        bool IsReady { get; }

        #endregion


        #region --- Public Methods ---

        void AfterInit(Exception exception);

        ISwInitParams CreateInitParams ();

        void Destroy ();
        SwCoreMonoBehaviour GetMono ();

        IEnumerator InitAsync ();

        bool IsDestroyed ();

        void PopulateInitParams([NotNull] Dictionary<string, object> initParamsDictionary);

        List<string> Validate ();

        #endregion
    }
}