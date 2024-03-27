using System.Collections;

namespace SupersonicWisdomSDK
{
    internal class SwNativeIosApi : SwNativeApi
    {
        #region --- Construction ---

        public SwNativeIosApi(SwNativeBridge nativeBridge) : base(nativeBridge)
        { }

        #endregion


        #region --- Public Methods ---

        [AOT.MonoPInvokeCallback(typeof(OnSessionEnded))]
        public static void OnSessionEnded(string sessionId)
        {
            OnSessionEndedCallbacks?.Invoke(sessionId);
        }

        [AOT.MonoPInvokeCallback(typeof(OnSessionStarted))]
        public static void OnSessionStarted(string sessionId)
        {
            OnSessionStartedCallbacks?.Invoke(sessionId);
        }
        
        [AOT.MonoPInvokeCallback(typeof(GetAdditionalDataJsonMethod))]
        public static string GetAdditionalDataJsonMethod()
        {
            return GetAdditionalDataJsonMethodCallbacks?.Invoke();
        }

        [AOT.MonoPInvokeCallback(typeof(OnWebResponse))]
        public static void OnWebResponse(string response)
        {
            OnWebResponseCallbacks?.Invoke(response);
        }
        
        [AOT.MonoPInvokeCallback(typeof(OnConnectivityStatusChanged))]
        public static void OnConnectivityStatusChanged(string connectionStatus)
        {
            OnConnectivityStatusChangedCallbacks?.Invoke(connectionStatus);
        }

        public override void AddSessionEndedCallback(OnSessionEnded callback)
        {
            OnSessionEndedCallbacks += callback;
        }

        public override void AddSessionStartedCallback(OnSessionStarted callback)
        {
            OnSessionStartedCallbacks += callback;
        }
        
        public override void AddAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback)
        {
            GetAdditionalDataJsonMethodCallbacks += callback;
        }

        public override void Destroy ()
        {
            NativeBridge.UnregisterSessionStartedCallback(OnSessionStartedCallbacks);
            NativeBridge.UnregisterSessionEndedCallback(OnSessionEndedCallbacks);
            NativeBridge.UnregisterWebRequestListener(OnWebResponseCallbacks);
            NativeBridge.UnregisterConnectivityStatusChanged(OnConnectivityStatusChangedCallbacks);
            NativeBridge.UnregisterGetAdditionalDataJsonMethod(GetAdditionalDataJsonMethodCallbacks);
            base.Destroy();
        }

        public override IEnumerator Init(SwNativeConfig configuration)
        {
            yield return NativeBridge.InitSdk(configuration);
            NativeBridge.RegisterSessionStartedCallback(OnSessionStarted);
            NativeBridge.RegisterSessionEndedCallback(OnSessionEnded);
            NativeBridge.RegisterGetAdditionalDataJsonMethod(GetAdditionalDataJsonMethod);
            NativeBridge.RegisterWebRequestListener(OnWebResponse);
            NativeBridge.RegisterConnectivityStatusChanged(OnConnectivityStatusChanged);
        }

        public override bool IsSupported ()
        {
            return true;
        }

        public override void RemoveSessionEndedCallback(OnSessionEnded callback)
        {
            OnSessionEndedCallbacks -= callback;
        }

        public override void RemoveSessionStartedCallback(OnSessionStarted callback)
        {
            OnSessionStartedCallbacks -= callback;
        }
        
        public override void RemoveAddAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback)
        {
            GetAdditionalDataJsonMethodCallbacks -= callback;
        }

        public override void SendRequest(string requestJsonString)
        {
            NativeBridge.SendRequest(requestJsonString);
        }

        public override void AddServerCallbacks(OnWebResponse callback)
        {
            OnWebResponseCallbacks += callback;
        }

        public override void RemoveServerCallbacks(OnWebResponse callback)
        {
            OnWebResponseCallbacks -= callback;
        }

        public override bool ToggleBlockingLoader(bool shouldPresent)
        {
            return NativeBridge.ToggleBlockingLoader(shouldPresent);
        }

        public override void RequestRateUsPopup()
        {
            NativeBridge.RequestRateUsPopup();
        }

        public override void AddConnectivityCallbacks(OnConnectivityStatusChanged callback)
        {
            OnConnectivityStatusChangedCallbacks += callback;
        }

        public override void RemoveConnectivityCallbacks(OnConnectivityStatusChanged callback)
        {
            OnConnectivityStatusChangedCallbacks -= callback;
        }

        public override string GetMegaSessionId()
        {
            return NativeBridge.GetMegaSessionId();
        }

        #endregion


        #region --- Private Methods ---

        public override void ClearDelegates()
        {
            var delegates = OnSessionStartedCallbacks?.GetInvocationList();

            if (delegates != null)
            {
                foreach (var item in delegates)
                {
                    OnSessionStartedCallbacks -= item as OnSessionStarted;
                }
            }

            delegates = OnSessionEndedCallbacks?.GetInvocationList();

            if (delegates == null) return;

            foreach (var item in delegates)
            {
                OnSessionEndedCallbacks -= item as OnSessionEnded;
            }
            
            delegates = GetAdditionalDataJsonMethodCallbacks?.GetInvocationList();
            
            if (delegates == null) return;
            
            foreach (var item in delegates)
            {
                GetAdditionalDataJsonMethodCallbacks -= item as GetAdditionalDataJsonMethod;
            }
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