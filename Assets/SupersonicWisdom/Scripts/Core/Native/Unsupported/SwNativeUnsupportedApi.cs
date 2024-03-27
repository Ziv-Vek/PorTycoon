using System;
using System.Collections;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwNativeUnsupportedApi : SwNativeApi
    {
        private OnWebResponse _callback;

        #region --- Construction ---

        public SwNativeUnsupportedApi(SwNativeBridge nativeBridge) : base(nativeBridge)
        { }

        #endregion


        #region --- Public Methods ---

        public override void AddSessionEndedCallback(OnSessionEnded callback)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, "");
        }

        public override void AddSessionStartedCallback(OnSessionStarted callback)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, "");
        }
        
        public override void AddAdditionalDataJsonMethod(GetAdditionalDataJsonMethod callback)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, "");
        }

        public override void Destroy ()
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, "");
        }

        public override string GetAdvertisingId ()
        {
            return string.Empty;
        }

        public override string GetConnectionStatus()
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, "");
            
            return string.Empty;
        }

        public override string GetOrganizationAdvertisingId ()
        {
            return string.Empty;
        }

        public override string GetAppInstallSource()
        {
            return "";
        }

        public override string GetMegaSessionId()
        {
            return "";
        }

        public override IEnumerator Init(SwNativeConfig configuration)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, $"{configuration}");

            yield break;
        }

        public override void InitializeSession(SwEventMetadataDto metadata)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, $"Uuid: {metadata.uuid}, CustomInstallationId: {metadata.swInstallationId}");
        }

        public override bool IsSupported ()
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
            SwInfra.CoroutineService.StartCoroutine(SendRequestRoutine(requestJsonString));
        }

        public override void AddServerCallbacks(OnWebResponse callback)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native);
        }

        private IEnumerator SendRequestRoutine(string requestJsonString)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native);
            yield break;
        }

        public override void RemoveServerCallbacks(OnWebResponse callback)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, $"{nameof(SwNativeUnsupportedApi)} | {nameof(RemoveServerCallbacks)}");
        }

        public override bool ToggleBlockingLoader(bool shouldPresent)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, "ToggleBlockingLoader(" + shouldPresent + ") - returning `false`");

            return false;
        }

        public override void UpdateMetadata(SwEventMetadataDto metadata)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, $"Uuid: {metadata.uuid}, CustomInstallationId: {metadata.swInstallationId}");
        }

        public override void UpdateWisdomConfiguration(SwNativeConfig configuration)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, $"configuration: {configuration}");
        }

        public override void RequestRateUsPopup()
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, $"Not supported on platforms other than Android and iOS");
        }

        public override void AddConnectivityCallbacks(OnConnectivityStatusChanged callback)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native);
        }

        public override void RemoveConnectivityCallbacks(OnConnectivityStatusChanged callback)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native);
        }

        #endregion


        #region --- Private Methods ---

        public override void ClearDelegates()
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, "SwNativeUnsupportedApi | clearDelegates");
        }

        public override void RemoveAllSessionCallbacks()
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, "SwNativeUnsupportedApi | RemoveAllSessionCallbacks");
        }

        public override void TrackEvent(string eventName, string customsJson, string extraJson)
        {
            SwInfra.Logger.Log(EWisdomLogType.Native, $"SwNativeUnsupportedApi | TrackEvent | {eventName} | {customsJson} | {extraJson}");
        }

        #endregion
    }
}