using UnityEngine;
using UnityEngine.Scripting;

namespace SupersonicWisdomSDK
{
    internal class SwNativeAndroidInitListener : AndroidJavaProxy
    {
        #region --- Constants ---

        private const string FullyQualifiedClassName = "wisdom.library.api.listener.IWisdomInitListener";

        #endregion


        #region --- Members ---

        public OnInitEnded OnInitEnded;

        #endregion


        #region --- Construction ---

        public SwNativeAndroidInitListener () : base(FullyQualifiedClassName)
        { }

        #endregion


        #region --- Public Methods ---

        [Preserve]
        // ReSharper disable once InconsistentNaming
        public void onInitEnded ()
        {
            SwInfra.MainThreadRunner.RunOnMainThread(() => { OnInitEnded?.Invoke(); });
        }

        #endregion
    }
}