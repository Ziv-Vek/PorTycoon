using UnityEngine;
using UnityEngine.Scripting;

namespace SupersonicWisdomSDK
{
    internal class SwNativeAndroidWebRequestListener : AndroidJavaProxy
    {
        #region --- Constants ---

        private const string FullyQualifiedClassName = "wisdom.library.api.listener.IWisdomRequestListener";

        #endregion


        #region --- Members ---
        
        public OnWebResponse OnWebResponse;

        #endregion


        #region --- Construction ---

        public SwNativeAndroidWebRequestListener () : base(FullyQualifiedClassName)
        { }

        #endregion


        #region --- Public Methods ---

        [Preserve]
        // ReSharper disable once InconsistentNaming
        public void onResponse(string responseJsonString)
        {
            SwInfra.MainThreadRunner.RunOnMainThread(() => { OnWebResponse?.Invoke(responseJsonString); });
        }

        #endregion
    }
}