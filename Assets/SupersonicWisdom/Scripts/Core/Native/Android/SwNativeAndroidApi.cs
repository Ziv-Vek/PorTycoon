using System.Collections;

namespace SupersonicWisdomSDK
{
    internal class SwNativeAndroidApi : SwNativeApi
    {
        #region --- Construction ---

        public SwNativeAndroidApi(SwNativeBridge nativeBridge) : base(nativeBridge)
        { }

        #endregion


        #region --- Public Methods ---

        public override void AddSessionEndedCallback(OnSessionEnded callback)
        {
            OnSessionEndedCallbacks += callback;
            NativeBridge.RegisterSessionEndedCallback(callback);
        }

        public override void AddSessionStartedCallback(OnSessionStarted callback)
        {
            OnSessionStartedCallbacks += callback;
            NativeBridge.RegisterSessionStartedCallback(callback);
        }

        public override void AddAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback)
        {
            GetAdditionalDataJsonMethodCallbacks += callback;
            NativeBridge.RegisterGetAdditionalDataJsonMethod(callback);
        }

        public override IEnumerator Init(SwNativeConfig configuration)
        {
            yield return NativeBridge.InitSdk(configuration);
        }

        public override bool IsSupported ()
        {
            return true;
        }

        public override void RemoveSessionEndedCallback(OnSessionEnded callback)
        {
            OnSessionEndedCallbacks -= callback;
            NativeBridge.UnregisterSessionEndedCallback(callback);
        }

        public override void RemoveSessionStartedCallback(OnSessionStarted callback)
        {
            OnSessionStartedCallbacks -= callback;
            NativeBridge.UnregisterSessionStartedCallback(callback);
        }

        public override void RemoveAddAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback)
        {
            GetAdditionalDataJsonMethodCallbacks -= callback;
            NativeBridge.UnregisterGetAdditionalDataJsonMethod(callback);
        }

        public override void SendRequest(string requestJsonString)
        {
            NativeBridge.SendRequest(requestJsonString);
        }

        public override void AddServerCallbacks(OnWebResponse callback)
        {
            OnWebResponseCallbacks += callback;
            NativeBridge.RegisterWebRequestListener(callback);
        }

        public override void AddConnectivityCallbacks(OnConnectivityStatusChanged callback)
        {
            OnConnectivityStatusChangedCallbacks += callback;
            NativeBridge.RegisterConnectivityStatusChanged(callback);
        }

        public override void RemoveServerCallbacks(OnWebResponse callback)
        {
            NativeBridge.UnregisterWebRequestListener(callback);
        }

        public override void RemoveConnectivityCallbacks(OnConnectivityStatusChanged callback)
        {
            OnConnectivityStatusChangedCallbacks -= callback;
            NativeBridge.UnregisterConnectivityStatusChanged(callback);
        }

        public override bool ToggleBlockingLoader(bool shouldPresent)
        {
            return NativeBridge.ToggleBlockingLoader(shouldPresent);
        }

        public override void RequestRateUsPopup()
        {
            NativeBridge.RequestRateUsPopup();
        }

        public override string GetMegaSessionId()
        {
            return NativeBridge.GetMegaSessionId();
        }

        #endregion


        #region --- Private Methods ---

        public override void ClearDelegates()
        {
            NativeBridge.UnregisterSessionStartedCallback(OnSessionStartedCallbacks);
            NativeBridge.UnregisterSessionEndedCallback(OnSessionEndedCallbacks);
            NativeBridge.UnregisterGetAdditionalDataJsonMethod(GetAdditionalDataJsonMethodCallbacks);
        }

        public override void RemoveAllSessionCallbacks()
        {
            base.RemoveAllSessionCallbacks();
            OnSessionStartedCallbacks = null;
            OnSessionEndedCallbacks = null;
            GetAdditionalDataJsonMethodCallbacks = null;
        }

        #endregion
    }
}