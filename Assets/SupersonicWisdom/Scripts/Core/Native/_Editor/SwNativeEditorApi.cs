#if UNITY_EDITOR

using System.Collections;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwNativeEditorApi : SwNativeApi
    {
        #region --- Constants ---

        private const string IS_AVAILABLE = "isAvailable";

        #endregion


        #region --- Members ---

        private OnConnectivityStatusChanged _connectivityCallback;
        private OnSessionStarted _onSessionStartedCallback;

        private OnWebResponse _webResponseCallback;

        #endregion
        
        public static SwNativeEditorApi Instance { get; private set; }

        public bool CurrentConnectionStatus { get; private set; }


        #region --- Construction ---

        public SwNativeEditorApi(SwNativeBridge nativeBridge) : base(nativeBridge)
        {
            Instance = this;
            CurrentConnectionStatus = true;
        }

        #endregion


        #region --- Public Methods ---

        public override void AddSessionEndedCallback(OnSessionEnded callback)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native);
        }

        public override void AddSessionStartedCallback(OnSessionStarted callback)
        {
            _onSessionStartedCallback += callback;
        }
        
        public override void AddAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native);
        }

        public override void Destroy()
        {
            SwInfra.Logger.Log(EWisdomLogType.Native);
        }

        public override string GetAdvertisingId()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }
        
        public override string GetAppInstallSource()
        {
            return "";
        }
        
        public override string GetMegaSessionId()
        {
            return "";
        }

        public override string GetConnectionStatus()
        {
            SwInfra.Logger.Log(EWisdomLogType.Native);

            return "{\"" + IS_AVAILABLE + "\":" + CurrentConnectionStatus.ToString().ToLower() + "}";
        }

        public override string GetOrganizationAdvertisingId()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }

        public override IEnumerator Init(SwNativeConfig configuration)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, $"{configuration}");

            // Uncomment below line in case you want to toggle change connection status
            // SwInfra.CoroutineService.StartCoroutine(ChangeConnectionStatusRepeating());
            
            yield break;
        }
        
        public static void InvokeConnectivity(bool isAvailable, bool isFlightMode)
        {
            Instance?._connectivityCallback?.Invoke( "{\"" + IS_AVAILABLE + "\":" + isAvailable.ToString().ToLower() + "}");
        }
        
        public static void InvokeSessionStarted(string sessionId)
        {
            Instance?._onSessionStartedCallback?.Invoke(sessionId);
        }

        public override void InitializeSession(SwEventMetadataDto metadata)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, $"Uuid: {metadata.uuid}, CustomInstallationId: {metadata.swInstallationId}");
        }

        public override bool IsSupported()
        {
            return false;
        }

        public override void RemoveSessionEndedCallback(OnSessionEnded callback)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native);
        }

        public override void RemoveSessionStartedCallback(OnSessionStarted callback)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native);
        }
        
        public override void RemoveAddAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native);
        }

        public override void SendRequest(string requestJsonString)
        {
            var request = JsonUtility.FromJson<SwRemoteRequest>(requestJsonString);
            SwInfra.CoroutineService.StartCoroutine(SendRequestRoutine(request.key, request.url, request.headers, request.body));
        }

        public override void AddServerCallbacks(OnWebResponse callback)
        {
            _webResponseCallback = callback;
        }

        public override void RemoveServerCallbacks(OnWebResponse callback)
        {
            _webResponseCallback = callback;
        }

        public override bool ToggleBlockingLoader(bool shouldPresent)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, $"ToggleBlockingLoader(" + shouldPresent + ") - returning `false`");

            return shouldPresent;
        }

        public override void RequestRateUsPopup()
        {
            SwInfra.Logger.Log(EWisdomLogType.Native);
        }

        public override void AddConnectivityCallbacks(OnConnectivityStatusChanged callback)
        {
            _connectivityCallback += callback;
        }

        public override void RemoveConnectivityCallbacks(OnConnectivityStatusChanged callback)
        {
            _connectivityCallback -= callback;
        }

        public override void UpdateMetadata(SwEventMetadataDto metadata)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, $"Uuid: {metadata.uuid}, CustomInstallationId: {metadata.swInstallationId}");
        }

        public override void UpdateWisdomConfiguration(SwNativeConfig configuration)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, $"configuration: {configuration}");
        }

        #endregion


        #region --- Private Methods ---

        public override void ClearDelegates()
        {
            SwInfra.Logger.Log(EWisdomLogType.Native);
        }

        public override void RemoveAllSessionCallbacks()
        {
            SwInfra.Logger.Log(EWisdomLogType.Native);
        }

        public override void TrackEvent(string eventName, string customsJson, string extraJson)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, $"{eventName} | {customsJson} | {extraJson}");
        }

        private IEnumerator ChangeConnectionStatusRepeating()
        {
            var isAvailable = true;

            while (true)
            {
                yield return new WaitForSeconds(5);

                isAvailable = !isAvailable;
                _connectivityCallback?.Invoke(isAvailable ? "{\"" + IS_AVAILABLE + "\":true}" : "{\"" + IS_AVAILABLE + "\":false}");
            }
        }

        private IEnumerator SendRequestRoutine(string key, string url, string headers, string body)
        {
            var client = new SwUnityWebRequestClient();

            yield return new WaitForSeconds(1);

            var response = new SwWebResponse
            {
                key = key,
            };

            yield return SwInfra.CoroutineService.StartCoroutine(client.Post(url, body, response, 10, null, true));

            if (response.DidSucceed)
            {
                if (response.data != null)
                {
                    _webResponseCallback?.Invoke(JsonUtility.ToJson(response));
                }
                else
                {
                    SwInfra.Logger.LogError(EWisdomLogType.Native, "Skipping callback");
                }
            }
            else
            {
                _webResponseCallback?.Invoke(JsonUtility.ToJson(response));
            }
        }

        #endregion
    }
}
#endif