using System.Collections;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal abstract class SwNativeApi : ISwNativeApi
    {
        #region --- Members ---

        protected static OnSessionEnded OnSessionEndedCallbacks;
        protected static OnSessionStarted OnSessionStartedCallbacks;
        protected static GetAdditionalDataJsonMethod GetAdditionalDataJsonMethodCallbacks;
        protected static OnWebResponse OnWebResponseCallbacks;
        protected static OnConnectivityStatusChanged OnConnectivityStatusChangedCallbacks;

        protected readonly SwNativeBridge NativeBridge;

        #endregion


        #region --- Construction ---

        public SwNativeApi(SwNativeBridge nativeBridge)
        {
            NativeBridge = nativeBridge;
        }

        #endregion


        #region --- Public Methods ---

        public virtual void Destroy ()
        {
            RemoveAllSessionCallbacks();
            NativeBridge.Destroy();
        }

        public virtual string GetAdvertisingId ()
        {
            return NativeBridge.GetAdvertisingId();
        }

        public virtual string GetMegaSessionId()
        {
            return NativeBridge.GetMegaSessionId();
        }

        public virtual string GetConnectionStatus()
        {
            return NativeBridge.GetConnectionStatus();
        }

        public virtual string GetOrganizationAdvertisingId()
        {
            return NativeBridge.GetOrganizationAdvertisingId();
        }
        
        public virtual string GetAppInstallSource()
        {
            return NativeBridge.GetAppInstallSource();
        }

        public virtual void InitializeSession(SwEventMetadataDto metadata)
        {
            NativeBridge.InitializeSession(metadata);
        }

        public virtual void UpdateMetadata(SwEventMetadataDto metadata)
        {
            NativeBridge.UpdateEventMetadata(metadata);
        }

        public virtual void UpdateWisdomConfiguration(SwNativeConfig configuration)
        {
            NativeBridge.UpdateWisdomConfiguration(configuration);
        }

        public abstract void AddSessionEndedCallback(OnSessionEnded callback);

        public abstract void AddSessionStartedCallback(OnSessionStarted callback);
        
        public abstract void AddAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback);
        
        public abstract void AddServerCallbacks(OnWebResponse callback);
        
        public abstract void RemoveSessionEndedCallback(OnSessionEnded callback);

        public abstract void RemoveSessionStartedCallback(OnSessionStarted callback);
        
        public abstract void RemoveAddAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback);
        
        public abstract void RemoveServerCallbacks(OnWebResponse callback);

        public abstract IEnumerator Init(SwNativeConfig configuration);

        public abstract bool IsSupported();

        public abstract void SendRequest(string requestJsonString);

        public abstract bool ToggleBlockingLoader(bool shouldPresent);

        public abstract void RequestRateUsPopup();

        public abstract void AddConnectivityCallbacks(OnConnectivityStatusChanged callback);
        
        public abstract void RemoveConnectivityCallbacks(OnConnectivityStatusChanged callback);

        public virtual void RemoveAllSessionCallbacks()
        {
            ClearDelegates();
        }

        public abstract void ClearDelegates();

        public virtual void TrackEvent(string eventName, string customsJson, string extraJson)
        {
            NativeBridge.TrackEvent(eventName, customsJson, extraJson);
        }

        #endregion
    }
}