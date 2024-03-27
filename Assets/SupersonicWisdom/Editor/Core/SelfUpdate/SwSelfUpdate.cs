using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwSelfUpdate
    {
        #region --- Constants ---

        internal const string CHECK_FOR_UPDATE_GRACEFUL_FIRST_RUN_KEY = nameof(CheckForUpdates);
        internal const int NONE_CODE = -1;
        internal const long TOTAL_SECONDS_IN_ONE_HOUR = 60 * 60;
        internal const long LOGIN_TO_CHECK_UPDATES_ALERT_DURATION_SECONDS = TOTAL_SECONDS_IN_ONE_DAY;
        internal const long RECURRING_CHECK_UPDATES_DURATION_SECONDS = TOTAL_SECONDS_IN_ONE_HOUR;
        private const long TOTAL_SECONDS_IN_ONE_DAY = 24 * TOTAL_SECONDS_IN_ONE_HOUR;

        #endregion


        #region --- Properties ---

        static SwSelfUpdate()
        {
        }

        internal static SwSelfUpdateConfiguration SelfUpdateConfiguration { get; private set; }

        #endregion


        #region --- Public Methods ---

        /// <summary>
        ///     This method initiates the self-update process, if needed.
        ///     In case weh updated is needed, the flows divided into two cases:
        ///     A. Stage update - an easy update, Wisdom will simply download the package and will import it.
        ///     This update should be easier because there's no version changes, Wisdom won't expect any folder structure changes,
        ///     components removal, refactors, etc.
        ///     B. Version update (or version + stage update) - a larger update, Wisdom will (a) restore its current version files
        ///     (b) delete all these files (c) import.
        ///     In case (a) fails - it won't continue to (b). In case (b) / (c) fails - Wisdom will restore itself and revert to
        ///     previous working version.
        /// </summary>
        /// <param name="isInitiatedByUser"></param>
        public static async void CheckForUpdates(bool isInitiatedByUser = false)
        {
            var (updatedStage, updatedVersionId) = await SwSelfUpdatePackageVersionManager.FetchVersionDetailsForTitle(isInitiatedByUser);
            var rawUpdateConfiguration = await SwSelfUpdatePackageVersionManager.FetchUpdateConfig();
            CreateSelfUpdateConfiguration(rawUpdateConfiguration, updatedStage, updatedVersionId);

            var shouldUpdate = await SwSelfUpdatePackageVersionManager.ShouldUpdate(isInitiatedByUser);

            if (!shouldUpdate || ShouldAbortUpdate(isInitiatedByUser, SelfUpdateConfiguration.UpdatedVersionId))
            {
                return;
            }

            SwEditorLogger.Log("Wisdom should be updated");
            SwEditorTracker.TrackEditorEvent(nameof(CheckForUpdates), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Info, "Wisdom should be updated");
            SwSelfUpdateUiManager.ShowUpdateConfirmation();
        }

        internal static async void OnUserClickedUpdate(SwSelfUpdateWindow updateWindow)
        {
            var updatedStage = SelfUpdateConfiguration.UpdatedStage;
            var updatedVersionId = SelfUpdateConfiguration.UpdatedVersionId;
            updateWindow.IsUpdateButtonEnabled = false;
            var didUpdate = await BeginSelfUpdate(updatedStage, updatedVersionId);

            if (didUpdate)
            {
                // This block should never run as it will be interrupted at some point.
                // The replaced and compiled code (from the imported package) will take it from there.
            }

            updateWindow.IsUpdateButtonEnabled = !didUpdate;
        }

        internal static bool VerifySuccessfulUpdate()
        {
            SwEditorLogger.Log($"{nameof(SwSelfUpdate)} | {nameof(VerifySuccessfulUpdate)} | Verifying...");
            SwEditorTracker.TrackEditorEvent(nameof(VerifySuccessfulUpdate), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Info, "Verifying SW self-update succeeded");

            var expectedFiles = SwSelfUpdateFileManagement.GetCurrentPackageManifestContent();

            if (expectedFiles.Count == 0)
            {
                SwEditorLogger.LogError("SW self-update verification failed, expected files list is empty");
                SwEditorTracker.TrackEditorEvent(nameof(VerifySuccessfulUpdate), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, "SW self-update verification failed, expected files list is empty");

                return false;
            }

            var allProjectFiles = SwFileUtils.GetFolderContent(Application.dataPath, true).ToList().ConvertAll(e => e.Replace(Application.dataPath, SwEditorConstants.ASSETS)).SwToHashSet();

            // Cleanup all
            expectedFiles.RemoveWhere(e => Path.GetFileName(e).StartsWith(SwFileUtils.HIDDEN_FILE_PREFIX));
            allProjectFiles.RemoveWhere(e => Path.GetFileName(e).StartsWith(SwFileUtils.HIDDEN_FILE_PREFIX));

            // Sometimes the list may contain "Unity hidden files and folder names"
            expectedFiles.RemoveWhere(e => e.EndsWith(SwFileUtils.UNITY_IGNORED_FILE_SUFFIX));
            allProjectFiles.RemoveWhere(e => e.EndsWith(SwFileUtils.UNITY_IGNORED_FILE_SUFFIX));

            // Sometimes there are expected meta files that still were not created at this pont
            expectedFiles.RemoveWhere(e => e.EndsWith(SwFileUtils.META_FILE_EXTENSION));
            allProjectFiles.RemoveWhere(e => e.EndsWith(SwFileUtils.META_FILE_EXTENSION));

            // Turns out this could also happen
            expectedFiles.RemoveWhere(string.IsNullOrEmpty);
            allProjectFiles.RemoveWhere(string.IsNullOrEmpty);

            expectedFiles.RemoveWhere(e => e.Contains(SwEditorUtils.SW_UPDATE_METADATA_FOLDER_NAME));

            SwInfra.Logger.Log(EWisdomLogType.SelfUpdate, $"expectedFiles: {expectedFiles.SwToString()}");
            SwInfra.Logger.Log(EWisdomLogType.SelfUpdate, $"allProjectFiles: {allProjectFiles.SwToString()}");
            expectedFiles.RemoveWhere(e => allProjectFiles.Contains(e));

            var isValid = 0 == expectedFiles.Count;

            if (!isValid)
            {
                SwEditorLogger.LogError($"SW self-update verification failed, {expectedFiles.Count} expected files not found: {expectedFiles.SwToString()}");
                SwEditorTracker.TrackEditorEvent(nameof(VerifySuccessfulUpdate), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, $"SW self-update verification failed");
            }
            else
            {
                CleanupLeftoverFiles();
            }

            SwEditorLogger.Log($"{nameof(SwSelfUpdate)} | {nameof(VerifySuccessfulUpdate)} | Is Wisdom package valid? => {isValid}");
            SwEditorTracker.TrackEditorEvent(nameof(VerifySuccessfulUpdate), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Info, $"Is Wisdom package valid? => {isValid}");
            
            
            return isValid;
        }

        internal static SwSelfUpdateConfiguration GetSelfUpdateConfiguration()
        {
            return SelfUpdateConfiguration;
        }

        #endregion


        #region --- Private Methods ---

        private static void CreateSelfUpdateConfiguration(SwSelfUpdateRawConfiguration rawUpdateConfiguration, int updatedStage, long updatedVersionId)
        {
            SelfUpdateConfiguration = new SwSelfUpdateConfiguration(rawUpdateConfiguration, updatedStage, updatedVersionId);
        }

        private static async Task<bool> BeginSelfUpdate(int updatedStageNumber, long requestedVersionId)
        {
            var remoteUrl = SwSelfUpdatePackageDownloader.GenerateUnityPackageRemoteUrl(updatedStageNumber, requestedVersionId);

            if ((remoteUrl?.Length ?? 0) == 0)
            {
                SwEditorAlerts.AlertError(SwEditorConstants.UI.FAILED_TO_GENERATE_REMOTE_URL, (int)SwErrors.ESelfUpdate.InvalidUnityPackageRemoteUrl, SwEditorConstants.UI.ButtonTitle.OK);

                return false;
            }

            var localDestinationFilePath = SwSelfUpdateFileManagement.GenerateLocalFilePath(updatedStageNumber, SwUtils.System.ComputeVersionString(requestedVersionId));
            var (downloadedFilePath, downloadUpdatePackageErrorCode, checksum) = await SwSelfUpdatePackageDownloader.DownloadUpdatePackageWithCheckSum(remoteUrl, localDestinationFilePath);

            var didUpdate = false;

            if ((downloadedFilePath?.Length ?? 0) > 0)
            {
                var md5 = SwEditorUtils.Md5(downloadedFilePath);

                if (md5 == checksum)
                {
                    Selection.activeObject = null; // Deselecting our settings object in the inspector before import. Motivation: The settings inspector is not refreshed properly in case of stage update while our settings is focused in the inspector.
                    var errorMessage = await SwSelfUpdatePackageVersionManager.UpdateWisdomFromPackage(localDestinationFilePath, updatedStageNumber, requestedVersionId);

                    didUpdate = errorMessage == null;

                    if (!didUpdate)
                    {
                        SwSelfUpdateUiManager.ShowUpdateFailed(errorMessage);
                    }
                }
                else
                {
                    SwEditorLogger.LogError($"Failed to download: Checksum doesn't match (expected: {checksum}, actual: {md5})");
                    SwEditorTracker.TrackEditorEvent(nameof(BeginSelfUpdate), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, $"Failed to download: Checksum doesn't match (expected: {checksum}, actual: {md5})");
                    SwSelfUpdateUiManager.Alert(SwSelfUpdateUiManager.ChecksumMismatch);
                }
            }
            else
            {
                if (downloadUpdatePackageErrorCode != 0 && downloadUpdatePackageErrorCode != (int)SwErrors.ESelfUpdate.DownloadUpdatePackageCanceled)
                {
                    SwEditorLogger.LogError($"Failed to download stage update Unity package file to path: {localDestinationFilePath}. Error code: {downloadUpdatePackageErrorCode}");
                    SwEditorTracker.TrackEditorEvent(nameof(BeginSelfUpdate), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, $"Failed to download stage update Unity package file to path: {localDestinationFilePath}. Error code: {downloadUpdatePackageErrorCode}");

                    SwSelfUpdateUiManager.Alert(SwEditorConstants.UI.FAILED_TO_DOWNLOAD_UNITY_PACKAGE, SwEditorConstants.UI.ButtonTitle.CLOSE, (SwErrors.ESelfUpdate)downloadUpdatePackageErrorCode);
                }
            }

            return didUpdate;
        }

        private static bool ShouldAbortUpdate(bool isInitiatedByUser, long updatedVersionId)
        {
            // If version was blacklisted in the remote config, abort the process.
            if (SelfUpdateConfiguration.VersionBlacklisted)
            {
                if (isInitiatedByUser)
                {
                    SwSelfUpdateUiManager.Alert(
                        SwEditorConstants.ErrorMessages.FAILED_TO_UPDATE_DUE_TO_BLACKLISTING.Format(SwConstants.SDK_VERSION),
                        SwEditorConstants.UI.ButtonTitle.CLOSE,
                        SwErrors.ESelfUpdate.VersionBlacklisted);
                }
                
                SwEditorLogger.Log("Blacklisted version detected, skipping update");

                return true;
            }

            // If there is a downgrade, we shouldn't update stage
            var isDowngrade = SwConstants.SdkVersionId > updatedVersionId;
            
            if (isDowngrade)
            { 
                if (isInitiatedByUser)
                {
                    SwSelfUpdateUiManager.Alert(
                        SwEditorConstants.ErrorMessages.FAILED_TO_UPDATE_DUE_TO_DOWNGRADE.Format(SwConstants.SDK_VERSION,
                            SwUtils.System.ComputeVersionString(updatedVersionId)),
                        SwEditorConstants.UI.ButtonTitle.CLOSE,
                        SwErrors.ESelfUpdate.DowngradeDetected);
                }

                SwEditorLogger.Log("Downgrade detected, skipping update");

                return true;
            }

            if (!SelfUpdateConfiguration.ShouldUpdate)
            {
                SwEditorLogger.Log("No need to update, skipping update");
                
                return true;
            }
            
			SwEditorLogger.Log("Downgrade detected, skipping stage update");
            SwEditorTracker.TrackEditorEvent(nameof(ShouldAbortUpdate), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Info, "Downgrade detected, skipping stage update");
            
            return false;
        }

        // This method is called after a successful update, it cleans up all the files that were left from the update process.
        // The files were leftover due to an issue in the SwFileUtils.DeleteFilesAtPaths method, which is used in the update process.
        private static void CleanupLeftoverFiles()
        {
            var baseFolder = $"{SwEditorConstants.ASSETS}/{SwEditorConstants.BASE_FOLDER_NAME}";
            string[] subfoldersToCheck = { "DemoScenes", "Editor", "Plugins", "Resources", "Scripts" };
            string[] stagesToCheck = { "Stage1", "Stage2", "Stage3" };

            foreach (string subfolder in subfoldersToCheck)
            {
                foreach (string stage in stagesToCheck)
                {
                    var fullPath = Path.Combine(baseFolder, subfolder, stage);

                    if(!Directory.Exists(fullPath)) continue;

                    var metaFilePath = fullPath + SwFileUtils.META_FILE_EXTENSION;

                    try
                    {
                        Directory.Delete(fullPath, true);
                    }
                    catch (Exception e) 
                    {
                        SwEditorLogger.LogWarning("Failed to delete directory: " + fullPath + " Error: " + e);
                    }

                    try
                    {
                        File.Delete(metaFilePath);
                    }
                    catch (Exception e)
                    {
                        SwEditorLogger.LogWarning("Failed to delete file: " + metaFilePath + " Error: " + e);
                    }
                }
            }

            AssetDatabase.Refresh();
        }

        #endregion
    }
}