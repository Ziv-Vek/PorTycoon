using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwSelfUpdatePackageVersionManager
    {
        #region --- Properties ---

        private static (int, int) ErroneousUpdateDetails
        {
            get
            {
                return (SwSelfUpdate.NONE_CODE, SwSelfUpdate.NONE_CODE);
            }
        }

        private static bool IsUpdateCheckInProgress { get; set; }

        #endregion

        
        #region --- Members ---
        
        private static SwSelfUpdateRawConfiguration _rawSelfUpdateConfig;
        private static SwSelfUpdateConfigFetcher _selfUpdateConfigFetcher;
        
        #endregion
        
        
        #region --- Constructor ---
        
        static SwSelfUpdatePackageVersionManager()
        { 
            _selfUpdateConfigFetcher = new SwSelfUpdateConfigFetcher();
        }
        
        #endregion
        
        
        #region --- Public Methods ---

        internal static async Task<bool> ShouldUpdate(bool isInitiatedByUser)
        {
            var currentTimestampSeconds = DateTime.Now.SwTimestampSeconds();

            if (ShouldQuitUpdateLocalChecksAndTryAlert(isInitiatedByUser, currentTimestampSeconds))
            {
                IsUpdateCheckInProgress = false;

                return false;
            }

            if (await ShouldQuitUpdateRemoteChecksAndTryAlert(isInitiatedByUser, currentTimestampSeconds))
            {
                IsUpdateCheckInProgress = false;

                return false;
            }

            IsUpdateCheckInProgress = false;
            
            // This is due to check in OnPostCompilation stage, so we need to check if it's the first run so that alert doesn't show in codebuild
            EditorPrefs.SetBool(SwSelfUpdate.CHECK_FOR_UPDATE_GRACEFUL_FIRST_RUN_KEY, false);

            return true;
        }

        internal static async Task<string> UpdateWisdomFromPackage(string unityPackageFilePath, int updatedStageNumber, long updatedVersionId)
        {
            var currentStep = 0;
            var didImport = false;

            try
            {
                var backUpFolderPath = HandleVersionUpdate(UpdateSteps);

                if (string.IsNullOrEmpty(backUpFolderPath))
                {
                    return SwEditorConstants.ErrorMessages.FAILED_TO_BACKUP;
                }

                didImport = await ImportPackage(unityPackageFilePath, backUpFolderPath, updatedStageNumber, updatedVersionId, UpdateSteps);
            }
            catch (Exception e)
            {
                SwEditorLogger.LogError("Importing SW update package failed! Exception:\n" + e);
                SwEditorTracker.TrackEditorEvent(nameof(UpdateWisdomFromPackage), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, e.ToString());
            }

            EditorUtility.ClearProgressBar();

            return didImport ? SwEditorConstants.ErrorMessages.FAILED_TO_IMPORT_PACKAGE : null;

            void UpdateSteps()
            {
                var progressDescriptions = new[]
                {
                    SwSelfUpdateUiManager.UiMessages.SW_UPDATE_BACKUP_STEP,
                    SwSelfUpdateUiManager.UiMessages.SW_UPDATE_DELETION_STEP,
                    SwSelfUpdateUiManager.UiMessages.SW_UPDATE_IMPORT_STEP,
                };

                var totalSteps = progressDescriptions.Length;
                var progressStep = currentStep / (float)totalSteps;
                var safeStep = Math.Min(currentStep, totalSteps - 1);

                var progressDescription = progressDescriptions[safeStep];
                currentStep++;

                SwEditorUtils.RunOnMainThread(() =>
                {
                    EditorUtility.DisplayProgressBar(SwEditorConstants.UI.SUPERSONIC_WISDOM_SDK, progressDescription, progressStep);
                    Thread.Sleep(1000);
                });
            }
        }

        internal static async Task<(int, long)> FetchVersionDetailsForTitle(bool isInitiatedByUser)
        {
            var currentStageUrl = CurrentStageUrl();

            if (string.IsNullOrEmpty(currentStageUrl))
            {
                return ErroneousUpdateDetails;
            }

            var (responseString, error, httpResponseMessage) = await SwNetworkHelper.PerformRequest(currentStageUrl, null, SwPlatformCommunication.CreateAuthorizationHeadersDictionary());

            if (error.IsValid)
            {
                if (isInitiatedByUser)
                {
                    SwEditorLogger.LogError($"Error: {error},\nHTTP response: {httpResponseMessage}");
                    SwEditorTracker.TrackEditorEvent(nameof(FetchVersionDetailsForTitle), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, error.ErrorMessage);
                    SwSelfUpdateUiManager.Alert(error.ErrorMessage, SwEditorConstants.UI.ButtonTitle.CLOSE, SwErrors.ESelfUpdate.CheckCurrentStage);
                }

                return ErroneousUpdateDetails;
            }

            var responseDictionary = SwJsonParser.DeserializeToDictionary(responseString);
            var errorCode = (int)(responseDictionary.SwSafelyGet("errorCode", null) ?? SwSelfUpdate.NONE_CODE);

            if (errorCode >= 0)
            {
                var errorMessage = responseDictionary.SwSafelyGet("errorMessage", string.Empty).ToString();

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    SwEditorLogger.LogError(errorMessage);
                }

                return ErroneousUpdateDetails;
            }

            var stageNumberString = responseDictionary.SwSafelyGet("stage", SwSelfUpdate.NONE_CODE.ToString()).ToString();
            var latestVersionString = responseDictionary.SwSafelyGet("latest-version", SwSelfUpdate.NONE_CODE.ToString()).ToString();
            var latestStableVersionId = SwUtils.System.ComputeVersionId(latestVersionString);

            if (latestStableVersionId == 0)
            {
                long.TryParse(latestVersionString, NumberStyles.Any, CultureInfo.InvariantCulture, out latestStableVersionId);
            }

            if (int.TryParse(stageNumberString, NumberStyles.Any, CultureInfo.InvariantCulture, out var stageNumber))
            {
                return (stageNumber, latestStableVersionId);
            }

            stageNumber = SwSelfUpdate.NONE_CODE;
            SwEditorLogger.LogError("Failed to parse an integer from stage number string: " + stageNumberString);
            SwEditorTracker.TrackEditorEvent(nameof(FetchVersionDetailsForTitle), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, "Failed to parse an integer from stage number string: " + stageNumberString);

            return (stageNumber, latestStableVersionId);
        }

        internal static async Task<SwSelfUpdateRawConfiguration> FetchUpdateConfig()
        {
            if (_rawSelfUpdateConfig != null)
            {
                SwEditorTracker.TrackEditorEvent(nameof(FetchUpdateConfig), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Info, "Using cached self update config.");
                return _rawSelfUpdateConfig;
            }

            try
            {
                var config = await SwEditorConfigManager.ManuallyFetchConfig();
                _rawSelfUpdateConfig = config.config?.selfUpdateConfig;
            }
            catch (Exception e)
            {
                SwEditorLogger.LogError(e);
                return JsonConvert.DeserializeObject<SwSelfUpdateRawConfiguration>(SwEditorConstants.EMPTY_STRINGIFIED_JSON);
            }
            
            return _rawSelfUpdateConfig;
        }

        #endregion


        #region --- Private Methods ---

        private static async Task<bool> ShouldQuitUpdateRemoteChecksAndTryAlert(bool isInitiatedByUser, long currentTimestampSeconds)
        {
            var lastCheckupTimestampString = EditorPrefs.GetString(SwEditorConstants.SwKeys.LAST_WISDOM_UPDATE_CHECKUP_TIMESTAMP, "0");
            long.TryParse(lastCheckupTimestampString, out var lastCheckupTimestamp);

            if (!isInitiatedByUser && currentTimestampSeconds - lastCheckupTimestamp < SwSelfUpdate.RECURRING_CHECK_UPDATES_DURATION_SECONDS) return true;

            EditorPrefs.SetString(SwEditorConstants.SwKeys.LAST_WISDOM_UPDATE_CHECKUP_TIMESTAMP, currentTimestampSeconds.ToString());

            SwEditorLogger.Log("Checking for updates...");
            var (updatedStage, updatedVersionId) = await FetchVersionDetailsForTitle(isInitiatedByUser);
            var selfUpdateConfiguration = SwSelfUpdate.GetSelfUpdateConfiguration();
            var shouldUpdate = selfUpdateConfiguration.ShouldUpdate;

            if (!shouldUpdate)
            {
                SwEditorLogger.Log("Can't / no need to update.");
                SwEditorTracker.TrackEditorEvent(nameof(ShouldQuitUpdateRemoteChecksAndTryAlert), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Info, "Can't / no need to update.");

                if (isInitiatedByUser)
                {
                    if (SwSelfUpdate.NONE_CODE == updatedStage || SwSelfUpdate.NONE_CODE == updatedVersionId)
                    {
                        SwSelfUpdateUiManager.Alert(SwSelfUpdateUiManager.UpdateCheckFailedOperation);
                    }
                    else
                    {
                        SwSelfUpdateUiManager.Alert(SwSelfUpdateUiManager.NoUpdateAvailable);
                    }
                }
            }

            return !shouldUpdate;
        }

        private static bool ShouldQuitUpdateLocalChecksAndTryAlert(bool isInitiatedByUser, long currentTimestampSeconds)
        {
            if (IsUpdateCheckInProgress || SwSelfUpdateWindow.GetIfPresented() != null)
            {
                HandleUpdateInProgress(isInitiatedByUser);

                return true;
            }

            IsUpdateCheckInProgress = true;

            if (SwEditorUtils.SwSettings == null)
            {
                return true;
            }

            if (!SwAccountUtils.IsLoggedIn)
            {
                HandleNotLoggedInAct(isInitiatedByUser, currentTimestampSeconds);

                return true;
            }

            if ((SwAccountUtils.TitlesList?.Count ?? 0) == 0)
            {
                HandleNoTitles(isInitiatedByUser);

                return true;
            }

            return false;
        }

        private static string CurrentStageUrl()
        {
            if (string.IsNullOrEmpty(SwSelfUpdatePackageDownloader.GameId)) return string.Empty;

            var queryString = SwUtils.System.SerializeToQueryString(new SwJsonDictionary
            {
                [SwSelfUpdatePackageDownloader.QueryParamKeys.ID] = SwSelfUpdatePackageDownloader.GameId,
            });

            return $"{SwPlatformCommunication.URLs.CURRENT_STAGE_API}?{queryString}";
        }

        private static async Task<bool> ImportPackage(string unityPackageFilePath, string backupFolderPath, int updatedStageNumber, long updatedVersionId, Action updateSteps)
        {
            PersistUpdateDetailsBeforeUpdate(unityPackageFilePath, backupFolderPath, updatedStageNumber, updatedVersionId, SwSelfUpdate.SelfUpdateConfiguration.ShouldVerifyUpdate);
            SwEditorLogger.Log($"Importing Unity package file from path: {unityPackageFilePath}");
            var didImport = await SwEditorUtils.ImportPackage(unityPackageFilePath, true);
            updateSteps?.Invoke();

            return didImport;
        }

        private static string HandleVersionUpdate(Action updateSteps)
        {
            updateSteps?.Invoke();
            var backUpFolderPath = SwSelfUpdateFileManagement.BackupCurrentVersionFiles();

            if (string.IsNullOrEmpty(backUpFolderPath))
            {
                SwEditorLogger.LogError(SwEditorConstants.ErrorMessages.FAILED_TO_BACKUP);
                SwEditorTracker.TrackEditorEvent(nameof(HandleVersionUpdate), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, SwEditorConstants.ErrorMessages.FAILED_TO_BACKUP);

                return null;
            }

            updateSteps?.Invoke();

            if (!SwSelfUpdateFileManagement.DeleteCurrentVersionFiles())
            {
                SwEditorLogger.LogError(SwEditorConstants.ErrorMessages.FAILED_TO_DELETE_CURRENT_VERSION);
                SwEditorTracker.TrackEditorEvent(nameof(HandleVersionUpdate), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, SwEditorConstants.ErrorMessages.FAILED_TO_DELETE_CURRENT_VERSION);

                var restorePath = SwSelfUpdateFileManagement.TryFindingBackupFolder(out var backupFolderPath)
                    ? backupFolderPath
                    : string.Empty;
                
                if (!SwSelfUpdateFileManagement.RestorePackageFromFolder(restorePath))
                {
                    SwEditorLogger.LogError("Failed to restore previous version files! please contact support.");
                }
                
                return null;
            }

            updateSteps?.Invoke();
            EditorUtility.ClearProgressBar();

            return backUpFolderPath;
        }

        private static void HandleUpdateInProgress(bool isInitiatedByUser)
        {
            if (isInitiatedByUser)
                SwEditorUtils.RunOnMainThread(() =>
                {
                    if (SwSelfUpdateUiManager.Alert(SwSelfUpdateUiManager.UpdateInProgressOperation))
                    {
                        SwSelfUpdateWindow.GetIfPresented()?.Focus();
                    }
                });
        }

        private static void HandleNotLoggedInAct(bool isInitiatedByUser, long currentTimestampSeconds)
        {
            var shouldShowPopup = isInitiatedByUser;

            if (!shouldShowPopup)
            {
                var lastLoginAlertTimestampString = EditorPrefs.GetString(SwEditorConstants.SwKeys.LAST_LOGIN_ALERT_TIMESTAMP, "0");
                long.TryParse(lastLoginAlertTimestampString, out var lastLoginAlertTimestampSeconds);

                shouldShowPopup = currentTimestampSeconds - lastLoginAlertTimestampSeconds > SwSelfUpdate.LOGIN_TO_CHECK_UPDATES_ALERT_DURATION_SECONDS;

                if (shouldShowPopup)
                {
                    EditorPrefs.SetString(SwEditorConstants.SwKeys.LAST_LOGIN_ALERT_TIMESTAMP, currentTimestampSeconds.ToString());
                }
            }

            if (!shouldShowPopup) return;

            if (SwEditorAlerts.Alert(SwEditorConstants.UI.LOGIN_REQUIRED_TO_CHECK_UPDATES, SwEditorConstants.UI.ButtonTitle.LOGIN_NOW, SwEditorConstants.UI.ButtonTitle.CANCEL))
            {
                SwEditorTracker.TrackEditorEvent(nameof(HandleNotLoggedInAct), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Info, "User logged in to check updates.");
                SwAccountUtils.GoToLoginTab();
            }
        }

        private static void HandleNoTitles(bool isInitiatedByUser)
        {
            if (!isInitiatedByUser)
            {
                return;
            }

            if (SwSelfUpdateUiManager.Alert(SwSelfUpdateUiManager.UpdateCheckFailedDueToEmptyGamesList))
            {
                SwAccountUtils.GoToLoginTab();
            }
        }

        private static void PersistUpdateDetailsBeforeUpdate(string unityPackageFilePath, string backupFolderPath, int updatedStageNumber, long updatedVersionId, bool shouldUpdateVersion)
        {
            var updateInfoJsonString = new SwJsonDictionary
            {
                [SwEditorConstants.SwKeys.IMPORTED_UNITY_PACKAGE_FILE_PATH] = unityPackageFilePath,
                [SwEditorConstants.SwKeys.IMPORTED_WISDOM_BACKUP_FOLDER_PATH] = backupFolderPath,
                [SwEditorConstants.SwKeys.UPDATED_SDK_STAGE_NUMBER] = updatedStageNumber,
                [SwEditorConstants.SwKeys.UPDATED_SDK_VERSION_ID] = updatedVersionId,
                [SwEditorConstants.SwKeys.SHOULD_VERIFY_UPDATE] = shouldUpdateVersion,
            }.SwToJsonString();
            
            SwEditorLogger.Log($"Persisting update info: {updateInfoJsonString.SwToString()}");
            EditorPrefs.SetString(SwEditorConstants.SwKeys.IMPORTED_WISDOM_UPDATE_INFO, updateInfoJsonString);
        }

        #endregion
        
        
        #region --- Internal Class ---
        
        private class SwSelfUpdateConfigFetcher : ISwEditorConfigListener
        {
            #region --- Constructor ---
            
            public SwSelfUpdateConfigFetcher()
            {
                SwEditorConfigManager.RegisterListener(this);
            }
            
            #endregion
            
            
            #region --- Public Methods ---
            
            public void OnConfigUpdate(SwEditorConfigData newConfigData)
            {
                _rawSelfUpdateConfig = newConfigData.config?.selfUpdateConfig;
            }
            
            #endregion
        }
        
        #endregion
    }
}