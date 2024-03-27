using UnityEngine;
using UnityEngine.Scripting;

namespace SupersonicWisdomSDK
{
    internal class SwNativeAndroidConnectivityListener : AndroidJavaProxy
    {
        #region --- Constants ---

        private const string FullyQualifiedClassName = "wisdom.library.api.listener.IWisdomConnectivityListener";

        #endregion


        #region --- Members ---
        
        public OnConnectivityStatusChanged ConnectivityStatusChangedEvent;

        #endregion


        #region --- Construction ---

        public SwNativeAndroidConnectivityListener () : base(FullyQualifiedClassName)
        { }

        #endregion


        #region --- Public Methods ---

        [Preserve]
        // ReSharper disable once InconsistentNaming
        public void onConnectionStatusChanged(string connectionStatus)
        {
            SwInfra.MainThreadRunner.RunOnMainThread(() =>
            {
                ConnectivityStatusChangedEvent?.Invoke(connectionStatus);
            });
        }

        #endregion
    }
}