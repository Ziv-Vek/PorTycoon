using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwNetworkHelper
    {
        #region --- Constants ---

        private const string MEDIA_TYPE_JSON = "application/json";
        private const int HTTP_REQUEST_TIMEOUT = 60;

        #endregion


        #region --- Public Methods ---

        public static async Task<Tuple<string, SwEditorError, HttpResponseMessage>> PerformRequest(string urlString, Dictionary<string, object> jsonDictionary, Dictionary<string, string> headers)
        {
            Tuple<string, SwEditorError, HttpResponseMessage> result = null;
            var didFinish = false;

            PerformRequest(urlString, jsonDictionary, headers, (resultString, error, httpResponseMessage) =>
            {
                result = new Tuple<string, SwEditorError, HttpResponseMessage>(resultString, error, httpResponseMessage);
                didFinish = true;
            });

            while (!didFinish)
            {
                await Task.Delay(300);
            }

            return result ?? new Tuple<string, SwEditorError, HttpResponseMessage>(null, null, null);
        }

        public static void PerformRequest(string urlString, Dictionary<string, object> jsonDictionary, Dictionary<string, string> headers, Action<string, SwEditorError, HttpResponseMessage> callback)
        {
            var error = new SwEditorError();
            Task<HttpResponseMessage> task = null;
            
            try
            {
                task = ExecuteRequestTask(urlString, jsonDictionary, headers);
                task.ContinueWith(sameTask =>
                {
                    var responseString = OnRequestTaskCompleted(sameTask, error);
                    
                    // This `completed` operation is running on a background thread, in most cases we would like to get a result on the main thread.
                    SwEditorUtils.RunOnMainThread(() => { callback.Invoke(responseString, error, sameTask.Result); });
                });
            }
            catch (Exception e)
            {
                error.ConsumeException(e, task);
                
                SwEditorUtils.RunOnMainThread(() => { callback.Invoke(null, error, null); });
            }
        }

        private static Task<HttpResponseMessage> ExecuteRequestTask(string urlString, ICollection jsonDictionary, Dictionary<string, string> headers)
        {
            Task<HttpResponseMessage> task;
            
            var handler = new HttpClientHandler { AllowAutoRedirect = false };
            var client = new HttpClient(handler) { Timeout = new TimeSpan(0, 0, HTTP_REQUEST_TIMEOUT) };
            headers?.Keys.ToList().ForEach(key => { client.DefaultRequestHeaders.Add(key, headers[key]); });
            
            if (jsonDictionary?.SwIsEmpty() ?? true)
            {
                task = client.GetAsync(urlString);
            }
            else
            {
                var jsonString = SwJsonParser.Serialize(jsonDictionary);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MEDIA_TYPE_JSON));
                var content = new StringContent(jsonString, Encoding.UTF8, MEDIA_TYPE_JSON);
                task = client.PostAsync(urlString, content);
            }

            return task;
        }

        private static string OnRequestTaskCompleted(Task<HttpResponseMessage> task, SwEditorError error)
        {
            string responseString = null;

            try
            {
                var result = task.Result;

                if (!result.IsSuccessStatusCode)
                {
                    error.ErrorMessage = result.ReasonPhrase;

                    if (string.IsNullOrEmpty(error.ErrorMessage))
                    {
                        error.ErrorMessage = result.Content.ReadAsStringAsync().Result;
                    }

                    error.ResponseCode = (int) result.StatusCode;
                }
                else
                {
                    responseString = result.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception e)
            {
                error.ConsumeException(e, task);
            }

            return responseString;
        }

        #endregion

        
        #region --- Extension Methods ---

        private static void ConsumeException(this SwEditorError self, Exception exception, Task<HttpResponseMessage> task)
        {
            if (string.IsNullOrEmpty(self.ErrorMessage))
            {
                self.ErrorMessage = task?.Result.Content.ReadAsStringAsync().Result ?? "";
            }

            if (string.IsNullOrEmpty(self.ErrorMessage))
            {
                self.ErrorMessage = exception.ToString();
            }
            else
            {
                self.ErrorMessage += $"\nException: {exception}";
            }
                        
            self.ResponseCode = (int) SwErrors.ECommunication.RequestFailed;
            
            SwEditorTracker.TrackEditorEvent(nameof(ConsumeException), ESwEditorWisdomLogType.Network, ESwEventSeverity.Error, self.ToString());
        }
        
        #endregion
        

        #region --- Private Methods ---

        internal static async Task<Tuple<string, int>> DownloadFileAsync(Uri uri, string destinationFilePath, Action<float, ulong, Action> downloadProgressionHandler = null)
        {
            var errorCode = 0;
            var didFinish = false;

            SwEditorCoroutines.StartEditorCoroutine(DownloadFile(uri.ToString(), destinationFilePath, (percentage, bytes, cancelAction) =>
            {
                if (didFinish) return;
            
                downloadProgressionHandler?.Invoke(percentage, bytes, () =>
                {
                    cancelAction?.Invoke();
                    errorCode = (int)SwErrors.ESelfUpdate.DownloadUpdatePackageCanceled;
                    didFinish = true;
                });
            }), callback: () => { didFinish = true; });

            while (!didFinish)
            {
                await Task.Delay(300);
            }

            var resultPath = File.Exists(destinationFilePath) ? destinationFilePath : "";

            return Tuple.Create(resultPath, errorCode);
        }

        private static IEnumerator DownloadFile(string downloadFileUrl, string destinationFilePath, Action<float, ulong, Action> progressionHandler = null, Action<int> onDone = null)
        {
            var errorCode = 0;
            var isCancelled = false;
            var downloadWebClient = new UnityWebRequest(downloadFileUrl) { downloadHandler = new DownloadHandlerFile(destinationFilePath) };
            downloadWebClient.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (downloadWebClient.result != UnityWebRequest.Result.ProtocolError)
#else
            if (!downloadWebClient.isHttpError && !downloadWebClient.isNetworkError)
#endif
            {
                while (!downloadWebClient.isDone && !isCancelled)
                {
                    yield return new WaitForSeconds(0.1f);

                    if (progressionHandler == null) continue;

                    var downloadProgress = downloadWebClient.downloadProgress;
                    var downloadedBytes = downloadWebClient.downloadedBytes;

                    SwEditorUtils.RunOnMainThread(() =>
                    {
                        void OnCancelClicked()
                        {
                            // User clicked "cancel" button
                            if (downloadWebClient != null && !downloadWebClient.isDone)
                            {
                                downloadWebClient.Abort();
                            }

                            isCancelled = true;
                            File.Delete(destinationFilePath);
                        }

                        progressionHandler(downloadProgress, downloadedBytes, OnCancelClicked);
                    });
                }
            }
            else
            {
                SwEditorLogger.LogError("Error Downloading from '" + downloadFileUrl + "' : " + downloadWebClient.error);
                File.Delete(destinationFilePath);

                errorCode = (int)SwErrors.ESelfUpdate.DownloadUpdatePackageFailed;
            }

            downloadWebClient.Dispose();

            onDone?.Invoke(errorCode);
        }

        #endregion
    }
}
