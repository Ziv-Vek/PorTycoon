using System.Collections;

namespace SupersonicWisdomSDK
{
    internal abstract class SwNativeBridge
    {
        #region --- Public Methods ---

        public abstract void Destroy();

        public abstract string GetAdvertisingId();

        public abstract string GetOrganizationAdvertisingId();
        
        public abstract string GetMegaSessionId();
        
        public abstract string GetAppInstallSource();

        public abstract void InitializeSession(SwEventMetadataDto metadata);
        public abstract IEnumerator InitSdk(SwNativeConfig configuration);

        public abstract void RegisterSessionEndedCallback(OnSessionEnded callback);

        public abstract void RegisterSessionStartedCallback(OnSessionStarted callback);
        
        public abstract void RegisterGetAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback);

        public abstract void SetEventMetadata(SwEventMetadataDto metadata);

        public abstract bool ToggleBlockingLoader(bool shouldPresent);

        public abstract void TrackEvent(string eventName, string customsJson, string extraJson);

        public abstract void UnregisterSessionEndedCallback(OnSessionEnded callback);

        public abstract void UnregisterSessionStartedCallback(OnSessionStarted callback);
        
        public abstract void UnregisterGetAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback);

        public abstract void UpdateEventMetadata(SwEventMetadataDto metadata);

        public abstract void UpdateWisdomConfiguration(SwNativeConfig configuration);

        public abstract void UnregisterWebRequestListener(OnWebResponse callback);

        public abstract void RegisterWebRequestListener(OnWebResponse callback);

        public abstract void SendRequest(string requestJsonString);

        public abstract void RequestRateUsPopup();

        public abstract void UnregisterConnectivityStatusChanged(OnConnectivityStatusChanged callback);
        
        public abstract void RegisterConnectivityStatusChanged(OnConnectivityStatusChanged callback);

        public abstract string GetConnectionStatus();

        #endregion
    }
}