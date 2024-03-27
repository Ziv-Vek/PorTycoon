using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace SupersonicWisdomSDK
{
    
    internal class SwUnityWebRequestClient: ISwWebRequestClient
    {
        public IEnumerator Get(string url, SwWebResponse response, int timeout = 0, Dictionary<string, string> headers = null, bool logResponseText = false)
        {
            UnityWebRequest webRequest = TryCreateWebRequest(url, timeout, response);
            if (webRequest == null) yield break;
            
            webRequest.method = UnityWebRequest.kHttpVerbGET;
            SwInfra.Logger.Log(EWisdomLogType.Network, $"{webRequest.method} | url={webRequest.url} | timeout={timeout} | headers = {SwJsonParser.Serialize(headers)}");
            yield return SendWebRequest(webRequest, response, headers, logResponseText);
            webRequest.Dispose();
        }

        public IEnumerator Post(string url, object data, SwWebResponse response, int timeout = 0, Dictionary<string, string> headers = null, bool logResponseText = false)
        {
            UnityWebRequest webRequest = TryCreateWebRequest(url, timeout, response);
            if (webRequest == null) yield break;
            var body = data as string ?? JsonUtility.ToJson(data);
            webRequest.method = UnityWebRequest.kHttpVerbPOST;
            if (!string.IsNullOrEmpty(body))
            {
                webRequest.uploadHandler = new UploadHandlerRaw(new System.Text.UTF8Encoding().GetBytes(body));
                webRequest.SetRequestHeader("Content-Type", "application/json");
            }
            
            SwInfra.Logger.Log(EWisdomLogType.Network, $"{webRequest.method} | url={webRequest.url} | body={body} | timeout={timeout} | headers = {SwJsonParser.Serialize(headers)}");
            yield return SendWebRequest(webRequest, response, headers, logResponseText);
            webRequest.Dispose();
        }

        private UnityWebRequest CreateWebRequest(string url, int timeout)
        {
            UnityWebRequest webRequest = new UnityWebRequest(url);
            webRequest.timeout = timeout;
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            return webRequest;
        }

        private UnityWebRequest TryCreateWebRequest(string url, int timeout, SwWebResponse response)
        {
            try
            {
                return CreateWebRequest(url, timeout);
            }
            catch (UriFormatException)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Network, $"Invalid URL: {url}");
                response.error = SwWebRequestError.InvalidUrl;
                response.isDone = true;
            }

            return null;
        }

        private IEnumerator SendWebRequest(UnityWebRequest webRequest, SwWebResponse response, Dictionary<string, string> headers, bool logResponseText = false)
        {
            if (headers != null)
            {
                foreach (var keyValuePair in headers)
                {
                    webRequest.SetRequestHeader(keyValuePair.Key, keyValuePair.Value);
                }
            }
            response.isPending = true;
            yield return webRequest.SendWebRequest();
            response.isPending = false;
            response.data = webRequest.downloadHandler.data;
            response.code = webRequest.responseCode;
            response.isDone = true;
            if (webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                response.error = SwWebRequestError.Http;
                SwInfra.Logger.LogError(EWisdomLogType.Network, $"{webRequest.method} {webRequest.url} fail | error={SwWebRequestError.Http} | responseCode={webRequest.responseCode}");
            }
            else if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                if (!string.IsNullOrEmpty(webRequest.error) && webRequest.error.EndsWith("timeout"))
                {
                    response.error = SwWebRequestError.Timeout;
                }
                else
                {
                    response.error = SwWebRequestError.Network;
                }
                
                SwInfra.Logger.LogError(EWisdomLogType.Network, $"{webRequest.method} {webRequest.url} fail | error={response.error}");
            }
            else
            {
                var logMessage = $"{webRequest.method} {webRequest.url} success | responseCode={webRequest.responseCode}" +
                                 $" {(logResponseText ? $" | responseText {response.Text}" : string.Empty)}";
                
                SwInfra.Logger.Log(EWisdomLogType.Network, logMessage);
            }
        }
    }
}