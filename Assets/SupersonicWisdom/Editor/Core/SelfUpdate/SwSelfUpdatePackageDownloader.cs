using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEditor;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwSelfUpdatePackageDownloader
    {
        #region --- Properties ---

        [CanBeNull]
        internal static string GameId
        {
            get
            {
                if (SwEditorUtils.SwSettings == null)
                {
                    // Might happen if the user deleted the settings file/git refresh didn't happen yet on import.
                    SwEditorLogger.LogWarning("SwSettings is null.");
                    return null;
                }
                
                var iosGameId = SwEditorUtils.SwSettings.iosGameId;
                var androidGameId = SwEditorUtils.SwSettings.androidGameId;
                string gameId;

                if (!string.IsNullOrEmpty(iosGameId) && !string.IsNullOrEmpty(androidGameId))
                {
                    // Both not empty - choose according to the current target.
                    gameId = SwSystemUtils.IsIosTarget() ? iosGameId : androidGameId;
                }
                else
                {
                    // At least one of them is empty - try to see the other available
                    gameId = !string.IsNullOrEmpty(iosGameId) ? iosGameId : androidGameId;
                }

                return gameId;
            }
        }

        internal static bool IsDownloadProcessInProgress { get; private set; }

        #endregion


        #region --- Public Methods ---

        internal static string GenerateUnityPackageRemoteUrl(int stageToDownload, long requestedVersionId)
        {
            var apiPath = string.Empty;

            if (requestedVersionId <= 0)
            {
                SwEditorLogger.LogError("The param 'requestedVersionId' doesn't exist, using current running version as fallback.");
                SwEditorTracker.TrackEditorEvent(nameof(GenerateUnityPackageRemoteUrl), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, "The param 'requestedVersionId' doesn't exist, using current running version as fallback.");
                requestedVersionId = SwConstants.SdkVersionId;
            }

            try
            {
                var versionToDownload = SwUtils.System.ComputeVersionString(requestedVersionId);
                var fileSuffix = string.Empty;

                if (SwStageUtils.MAX_STAGE_NUMBER != stageToDownload)
                {
                    fileSuffix = "_Stage" + stageToDownload;
                }

                var path = versionToDownload + "/SupersonicWisdomSDK_" + versionToDownload + fileSuffix + SwFileUtils.UNITY_PACKAGE_FILE_EXTENSION;
                apiPath = SwPlatformCommunication.URLs.DOWNLOAD_WISDOM_PACKAGE + $"?{QueryParamKeys.ID}={GameId}&path={path}&{QueryParamKeys.SDK_VERSION_ID}={requestedVersionId}";
            }
            catch (Exception e)
            {
                SwEditorLogger.LogError(e);
            }

            return apiPath;
        }

        internal static async Task<(string, int, string)> DownloadUpdatePackageWithCheckSum(string remoteUrl, string localDestinationFilePath)
        {
            string downloadedFilePath = null;
            var downloadUpdatePackageErrorCode = 0;
            string checksum = null;

            try
            {
                // Promise style...
                await Task.WhenAll(new List<Task>
                {
                    Task.Run(async () =>
                    {
                        var (downloadedFile, errorCode) = await DownloadUpdatePackage(remoteUrl, localDestinationFilePath);
                        downloadedFilePath = downloadedFile;
                        downloadUpdatePackageErrorCode = errorCode;
                    }),
                    Task.Run(async () =>
                    {
                        var wisdomPackageApi = remoteUrl.Replace(SwPlatformCommunication.URLs.DOWNLOAD_WISDOM_PACKAGE, SwPlatformCommunication.URLs.WISDOM_PACKAGE_MANIFEST);
                        var (response, error, httpResponseMessage) = await SwNetworkHelper.PerformRequest(wisdomPackageApi, null, SwPlatformCommunication.CreateAuthorizationHeadersDictionary());
                        var wisdomPackageManifest = response.SwToJsonDictionary();
                        checksum = wisdomPackageManifest.SwSafelyGet(SwEditorUtils.Keys.CHECKSUM, string.Empty).ToString();
                    }),
                });
            }
            catch
            {
                //
            }

            return (downloadedFilePath, downloadUpdatePackageErrorCode, checksum);
        }

        #endregion


        #region --- Private Methods ---

        private static async Task<(string, int)> DownloadUpdatePackage(string fromRemoteUrl, string toLocalFilePath)
        {
            var (_, error, httpResponseMessage) = await SwNetworkHelper.PerformRequest(fromRemoteUrl, null, SwPlatformCommunication.CreateAuthorizationHeadersDictionary());

            var fileTempUrl = string.Empty;

            if (error.IsValid && 302 == error.ResponseCode)
            {
                // Got a redirect response: https://learn.microsoft.com/en-us/dotnet/api/system.net.http.headers.httpresponseheaders.location?view=net-6.0#system-net-http-headers-httpresponseheaders-location
                fileTempUrl = httpResponseMessage.Headers.Location.ToString();
            }

            if (string.IsNullOrEmpty(fileTempUrl)) return (null, error.ResponseCode);

            IsDownloadProcessInProgress = true;

            var (downloadedFilePath, errorCode) = await SwNetworkHelper.DownloadFileAsync(new Uri(fileTempUrl),
                toLocalFilePath,
                (percentagesDownloaded, totalBytesDownloaded, onCancel) =>
                {
                    if (!IsDownloadProcessInProgress)
                    {
                        return;
                    }

                    if (!EditorUtility.DisplayCancelableProgressBar(SwEditorConstants.UI.SUPERSONIC_WISDOM_SDK, SwEditorConstants.UI.DownloadingUpdatePackage(SwUtils.File.GenerateFileSizeString(totalBytesDownloaded), (percentagesDownloaded * 100).ToString("0")), percentagesDownloaded))
                    {
                        return;
                    }

                    IsDownloadProcessInProgress = false;

                    SwEditorUtils.RunOnMainThread(() =>
                    {
                        onCancel?.Invoke();
                        SwSelfUpdateWindow.CloseWindow();
                        EditorUtility.ClearProgressBar();
                    });
                });

            IsDownloadProcessInProgress = false;

            SwEditorUtils.RunOnMainThread(EditorUtility.ClearProgressBar, 100);

            return (downloadedFilePath, errorCode);
        }

        #endregion


        #region --- Internal Classes ---

        internal static class QueryParamKeys
        {
            #region --- Constants ---

            internal const string ID = "id";
            internal const string SDK_VERSION_ID = "sdkVersionId";

            #endregion
        }
        #endregion
    }
}