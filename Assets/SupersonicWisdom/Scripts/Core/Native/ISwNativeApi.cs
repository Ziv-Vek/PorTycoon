using System.Collections;

namespace SupersonicWisdomSDK
{
    interface ISwNativeApi: ISwAdvertisingIdsGetter
    {
        public abstract void Destroy();
        public abstract void InitializeSession(SwEventMetadataDto metadata);
        public abstract string GetConnectionStatus();
        public abstract string GetMegaSessionId();
        abstract void UpdateMetadata(SwEventMetadataDto metadata);

        void UpdateWisdomConfiguration(SwNativeConfig configuration);

        public abstract void AddSessionEndedCallback(OnSessionEnded callback);
        public abstract void AddSessionStartedCallback(OnSessionStarted callback);
        public abstract void AddAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback);
        public abstract void AddServerCallbacks(OnWebResponse callback);
        public abstract void AddConnectivityCallbacks(OnConnectivityStatusChanged onConnectionStatusChanged);
        public abstract void RemoveSessionEndedCallback(OnSessionEnded callback);
        public abstract void RemoveSessionStartedCallback(OnSessionStarted callback);
        public abstract void RemoveAddAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback);
        public abstract void RemoveServerCallbacks(OnWebResponse callback);
        public abstract void RemoveConnectivityCallbacks(OnConnectivityStatusChanged onConnectionStatusChanged);
        public abstract IEnumerator Init(SwNativeConfig configuration);
        
        public abstract bool IsSupported();
        public abstract void SendRequest(string requestJsonString);
        public abstract bool ToggleBlockingLoader(bool shouldPresent);
        
        public abstract void RequestRateUsPopup();
        
        public abstract string GetAppInstallSource();
        public void RemoveAllSessionCallbacks();
        public abstract void ClearDelegates();
        public void TrackEvent(string eventName, string customsJson, string extraJson);
    }
}