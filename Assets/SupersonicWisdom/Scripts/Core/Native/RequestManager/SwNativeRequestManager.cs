using System;
using System.Collections.Generic;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwNativeRequestManager
    {
        private const int IOS_HTTP_ERROR_STATUS_CODE = 400;
        private const int IOS_NETWORK_ERROR_STATUS_CODE = -6;
        private const int IOS_TIMEOUT_ERROR_STATUS_CODE = -1001;
        private const int IOS_INVALID_URL_ERROR_STATUS_CODE = -1;
        
        private const int ANDROID_UNSUPPORTED_ENCODING_ERROR_STATUS_CODE = 3;
        private const int ANDROID_BAD_REQUEST_ERROR_STATUS_CODE = 400;
        private const int ANDROID_PROTOCOL_ERROR_STATUS_CODE_3 = -4;
        private const int ANDROID_INTERNAL_IO_ERROR_STATUS_CODE_4 = -5;
        private const int ANDROID_NETWORK_ERROR_STATUS_CODE = -6;
        private const int ANDROID_TIMEOUT_ERROR_STATUS_CODE = -1;
        private const int ANDROID_INVALID_URL_ERROR_STATUS_CODE = -2;


        #region --- Members ---

        private readonly Dictionary<string, ISwNativeRequestListener> _requestsDict;

        private readonly ISwNativeApi _wisdomNativeApi;

        #endregion


        #region --- Construction ---

        public SwNativeRequestManager(ISwNativeApi nativeApi)
        {
            _wisdomNativeApi = nativeApi;
            _requestsDict = new Dictionary<string, ISwNativeRequestListener>();
        }

        #endregion


        #region --- Public Methods ---

        public void Init()
        {
            _wisdomNativeApi.AddServerCallbacks(OnResponse);
        }

        public void SendRequest(string url, string headers, string body, ISwNativeRequestListener listener, int connectionTimeout, int readTimeout, int cap)
        {
            var key = Guid.NewGuid().ToString();
            _requestsDict[key] = listener;

            var requestJsonString = JsonUtility.ToJson(new SwRemoteRequest
            {
                key = key,
                url = url,
                headers = headers,
                body = body,
                connectionTimeout = connectionTimeout,
                readTimeout = readTimeout,
                cap = cap,
            });

            SwInfra.Logger.Log(EWisdomLogType.Native, $"Send request with key - {key}");
            _wisdomNativeApi.SendRequest(requestJsonString);
        }

        #endregion


        #region --- Private Methods ---
    
        private void OnResponse(string response)
        {
            // This code must run on main thread since this flow will eventually send event
            // and the extra data values are filled only when running on main thread
            SwInfra.MainThreadRunner.RunOnMainThread(() =>
            {
                var swWebResponse = JsonUtility.FromJson<SwWebResponse>(response);
                swWebResponse.error = StatusCodeToEnum(swWebResponse.code);
                var key = swWebResponse.key;
                var request = _requestsDict[key];

                SwInfra.Logger.Log(EWisdomLogType.Native, $"Response with key - {key} | DidSuccess = {swWebResponse.DidSucceed} | Error = {swWebResponse.error} | iteration = {swWebResponse.iteration} | cap = {swWebResponse.cap} | time = {swWebResponse.time}");

                if (!_requestsDict.ContainsKey(key))
                {
                    SwInfra.Logger.LogError(EWisdomLogType.Native, $"Could not find listener for - {key}");

                    return;
                }

                if (swWebResponse.DidSucceed)
                {
                    request?.OnSuccess(swWebResponse);
                    _requestsDict.Remove(key);
                }
                else
                {
                    if (swWebResponse.isReachedCap)
                    {
                        request?.OnFail(swWebResponse);
                        _requestsDict.Remove(key);
                    }
                    else
                    {
                        request?.OnIteration(swWebResponse);
                    }
                }
            });
        }

        private SwWebRequestError StatusCodeToEnum(long code)
        {
            if (code >= 200 && code <= 299)
            {
                return SwWebRequestError.None;
            }

#if UNITY_ANDROID

            return AndroidCodeToEnum(code);

#elif UNITY_IOS
            return IosCodeToEnum(code);

#else

            return SwWebRequestError.None;
#endif
        }

        private SwWebRequestError IosCodeToEnum(long code)
        {
            switch (code)
            {
                case IOS_NETWORK_ERROR_STATUS_CODE:
                    return SwWebRequestError.Network;

                case IOS_HTTP_ERROR_STATUS_CODE:
                    return SwWebRequestError.Http;

                case IOS_INVALID_URL_ERROR_STATUS_CODE:
                    return SwWebRequestError.InvalidUrl;

                case IOS_TIMEOUT_ERROR_STATUS_CODE:
                    return SwWebRequestError.Timeout;
            }

            return SwWebRequestError.Unknown;
        }

        private SwWebRequestError AndroidCodeToEnum(long code)
        {
            switch (code)
            {
                case ANDROID_NETWORK_ERROR_STATUS_CODE:
                    return SwWebRequestError.Network;

                case ANDROID_UNSUPPORTED_ENCODING_ERROR_STATUS_CODE:
                case ANDROID_BAD_REQUEST_ERROR_STATUS_CODE:
                case ANDROID_PROTOCOL_ERROR_STATUS_CODE_3:
                case ANDROID_INTERNAL_IO_ERROR_STATUS_CODE_4:
                    return SwWebRequestError.Http;

                case ANDROID_INVALID_URL_ERROR_STATUS_CODE:
                    return SwWebRequestError.InvalidUrl;

                case ANDROID_TIMEOUT_ERROR_STATUS_CODE:
                    return SwWebRequestError.Timeout;
            }

            return SwWebRequestError.Unknown;
        }

        #endregion
    }
}