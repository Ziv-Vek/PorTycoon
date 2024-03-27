using System;
using System.Collections;

#if UNITY_IOS
using System.Runtime.InteropServices;
using UnityEngine;
#endif

namespace SupersonicWisdomSDK
{
    internal class SwNativeIosBridge : SwNativeBridge
    {
        public override IEnumerator InitSdk(SwNativeConfig configuration)
        {
#if UNITY_IOS
            initSdk(configuration);
#endif
            yield break;
        }

        public override void UpdateWisdomConfiguration(SwNativeConfig configuration)
        {
#if UNITY_IOS
            updateWisdomConfiguration(configuration);
#endif
        }

        public override void SendRequest(string requestJsonString)
        {
#if UNITY_IOS
            sendRequest(requestJsonString);
#endif
        }

        public override void InitializeSession(SwEventMetadataDto metadata)
        {
#if UNITY_IOS
            initializeSession(JsonUtility.ToJson(metadata));
#endif
        }

        public override void RegisterSessionStartedCallback(OnSessionStarted callback)
        {
#if UNITY_IOS
            registerSessionStartedCallback(callback);
#endif
        }

        public override void RegisterSessionEndedCallback(OnSessionEnded callback)
        {
#if UNITY_IOS
            registerSessionEndedCallback(callback);
#endif
        }

        public override void RegisterGetAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback)
        {
#if UNITY_IOS
            registerGetAdditionalDataJsonMethod(callback);
#endif
        }

        public override void RegisterWebRequestListener(OnWebResponse callback)
        {
#if UNITY_IOS
            registerWebRequestCallback(callback);
#endif
        }

        public override void UnregisterSessionStartedCallback(OnSessionStarted callback)
        {
#if UNITY_IOS
            unregisterSessionStartedCallback(callback);
#endif
        }

        public override void UnregisterSessionEndedCallback(OnSessionEnded callback)
        {
#if UNITY_IOS
            unregisterSessionEndedCallback(callback);
#endif
        }

        public override void UnregisterGetAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback)
        {
#if UNITY_IOS
            unregisterGetAdditionalDataJsonMethod(callback);
#endif
        }

        public override void UnregisterWebRequestListener(OnWebResponse callback)
        {
#if UNITY_IOS
            unregisterWebRequestCallback(callback);
#endif
        }

        public override bool ToggleBlockingLoader(bool shouldPresent)
        {
#if UNITY_IOS
            return toggleBlockingLoader(shouldPresent);
#else
            return false;
#endif
        }

        public override void SetEventMetadata(SwEventMetadataDto metadata)
        {
#if UNITY_IOS
            setEventMetadata(JsonUtility.ToJson(metadata));
#endif
        }

        public override void UpdateEventMetadata(SwEventMetadataDto metadata)
        {
#if UNITY_IOS
            updateEventMetadata(JsonUtility.ToJson(metadata));
#endif
        }

        public override string GetOrganizationAdvertisingId ()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return UnityEngine.iOS.Device.vendorIdentifier;
#else
            return string.Empty; // Unsupported symbol
#endif
        }

        public override string GetAdvertisingId ()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return UnityEngine.iOS.Device.advertisingIdentifier;
#else
            return string.Empty; // Unsupported symbol
#endif
        }

        public override string GetMegaSessionId()
        {
#if UNITY_IOS
            return getMegaSessionId();
#else
            return "";
#endif
        }

        public override string GetAppInstallSource()
        {
            return ""; // Unsupported symbol
        }

        public override void TrackEvent(string eventName, string customsJson, string extraJson)
        {
#if UNITY_IOS
            trackEvent(eventName, customsJson, extraJson);
#endif
        }

        public override void Destroy ()
        {
#if UNITY_IOS
            destroy();
#endif
        }

        public override void RequestRateUsPopup()
        {
#if UNITY_IOS && !UNITY_EDITOR
            UnityEngine.iOS.Device.RequestStoreReview();
#else
            return; // Unsupported symbol
#endif
        }

        public override void RegisterConnectivityStatusChanged(OnConnectivityStatusChanged callback)
        {
#if UNITY_IOS && !UNITY_EDITOR
            registerConnectivityListener(callback);
#endif
        }

        public override void UnregisterConnectivityStatusChanged(OnConnectivityStatusChanged callback)
        {
#if UNITY_IOS && !UNITY_EDITOR
            unregisterConnectivityListener(callback);
#endif
        }

        public override string GetConnectionStatus()
        {
#if UNITY_IOS && !UNITY_EDITOR
            return getConnectionStatus();
#else
            return string.Empty;
#endif
        }

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern void initSdk(SwNativeConfig configuration);

        [DllImport("__Internal")]
        private static extern void updateWisdomConfiguration(SwNativeConfig configuration);

        [DllImport("__Internal")]
        private static extern void initializeSession(string metadataJson);

        [DllImport("__Internal")]
        private static extern void registerSessionStartedCallback(OnSessionStarted callback);

        [DllImport("__Internal")]
        private static extern void registerSessionEndedCallback(OnSessionEnded callback);
        
        [DllImport("__Internal")]
        private static extern void registerGetAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback);

        [DllImport("__Internal")]
        private static extern void registerWebRequestCallback(OnWebResponse callback);

        [DllImport("__Internal")]
        private static extern void registerConnectivityListener(OnConnectivityStatusChanged callback);
        
        [DllImport("__Internal")]
        private static extern void unregisterSessionStartedCallback(OnSessionStarted callback);

        [DllImport("__Internal")]
        private static extern void unregisterSessionEndedCallback(OnSessionEnded callback);
        
        [DllImport("__Internal")]
        private static extern void unregisterGetAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback);

        [DllImport("__Internal")]
        private static extern void unregisterWebRequestCallback(OnWebResponse callback);
        
        [DllImport("__Internal")]
        private static extern void unregisterConnectivityListener(OnConnectivityStatusChanged callback);
        
        [DllImport("__Internal")]
        private static extern string getConnectionStatus();

        [DllImport("__Internal")]
        protected internal static extern bool toggleBlockingLoader(bool shouldPresent);

        [DllImport("__Internal")]
        private static extern void setEventMetadata(string metadataJson);

        [DllImport("__Internal")]
        private static extern void updateEventMetadata(string metadataJson);

        [DllImport("__Internal")]
        private static extern void trackEvent(string eventName, string customsJson, string extraJson);

        [DllImport("__Internal")]
        private static extern void destroy();
        
        [DllImport("__Internal")]
        private static extern void sendRequest(string requestJson);
        
        [DllImport("__Internal")]
        private static extern string getMegaSessionId();
#endif
    }
}