using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwSelfUpdateEventsHandler
    {
        #region --- Public Methods ---

        public static void OnPreCompilation()
        {
            EditorPrefs.SetString(SwEditorConstants.SwKeys.TEMP_AUTH_TOKEN, SwAccountUtils.AccountToken);

            if (!SwSelfUpdatePackageDownloader.IsDownloadProcessInProgress) return;

            SwSelfUpdateWindow.CloseWindow();
            SwEditorAlerts.AlertError(SwEditorConstants.UI.WISDOM_UPDATE_FAILED_DUE_TO_COMPILATION, (long)SwErrors.ESelfUpdate.CompilationInterference, SwEditorConstants.UI.ButtonTitle.OK);
        }

        public static void OnPostCompilation()
        {
            TryAlertWisdomUpdateFinished();
            
            // This is due to check in OnPostCompilation stage, so we need to check if it's the first run so that alert doesn't show in codebuild
            var isFirstRun = EditorPrefs.GetBool(SwSelfUpdate.CHECK_FOR_UPDATE_GRACEFUL_FIRST_RUN_KEY, true);
            
            if (!isFirstRun)
            {
                SwSelfUpdate.CheckForUpdates();
            }
        }

        #endregion


        #region --- Private Methods ---

        private static void TryAlertWisdomUpdateFinished()
        {
            var updateInfoJsonString = EditorPrefs.GetString(SwEditorConstants.SwKeys.IMPORTED_WISDOM_UPDATE_INFO, SwEditorConstants.EMPTY_STRINGIFIED_JSON);
            
            if(updateInfoJsonString.Equals(SwEditorConstants.EMPTY_STRINGIFIED_JSON)) return;

            var updateInfoJsonDictionary = updateInfoJsonString.SwToJsonDictionary();
            var backupFolderPath = SwSelfUpdateFileManagement.EnsurePath(updateInfoJsonDictionary.SwSafelyGet(SwEditorConstants.SwKeys.IMPORTED_WISDOM_BACKUP_FOLDER_PATH, "").ToString(), 
                SwSelfUpdateFileManagement.WisdomBackUpFolderPath, SwSelfUpdateFileManagement.FileOrFolderPath.Folder);
            var unityPackageFilePath = SwSelfUpdateFileManagement.EnsurePath(updateInfoJsonDictionary.SwSafelyGet(SwEditorConstants.SwKeys.IMPORTED_UNITY_PACKAGE_FILE_PATH, "").ToString(), 
                SwSelfUpdateFileManagement.PendingUnityPackagesToImportFolderPath, SwSelfUpdateFileManagement.FileOrFolderPath.File);
            
            int.TryParse(updateInfoJsonDictionary.SwSafelyGet(SwEditorConstants.SwKeys.UPDATED_SDK_STAGE_NUMBER, SwSelfUpdate.NONE_CODE.ToString()).ToString(), out var updatedStageNumber);
            int.TryParse(updateInfoJsonDictionary.SwSafelyGet(SwEditorConstants.SwKeys.UPDATED_SDK_VERSION_ID, SwSelfUpdate.NONE_CODE.ToString()).ToString(), out var updatedVersionId);
            bool.TryParse(updateInfoJsonDictionary.SwSafelyGet(SwEditorConstants.SwKeys.SHOULD_VERIFY_UPDATE, true.ToString()).ToString(), out var shouldVerifyUpdate);

            OnVerificationRequested(unityPackageFilePath, backupFolderPath, shouldVerifyUpdate);

            var didCompleteUpdate = updatedStageNumber == SwStageUtils.CurrentStage.sdkStage || updatedVersionId == SwConstants.SdkVersionId;

            if (didCompleteUpdate)
            {
                SwEditorCoroutines.StartEditorCoroutine(OnPostUpdate());
            }
        }

        private static IEnumerator OnPostUpdate()
        {
            // Important! Do not close the update window, this window is left in order to provide a link for opening the integration guide.
            yield return new WaitForSeconds(2);

            SwSelfUpdateFileManagement.DeletePendingUnityPackagesToImportFolder();

            EditorPrefs.DeleteKey(SwEditorConstants.SwKeys.IMPORTED_WISDOM_UPDATE_INFO);

            if (SwSelfUpdateUiManager.Alert(SwSelfUpdateUiManager.UpdateSuccessful))
            {
                SwEditorUtils.OpenSettings();
            }
        }

        private static void OnVerificationRequested(string unityPackageFilePath, string backupFolderPath, bool shouldVerifyUpdate)
        {
            if (!shouldVerifyUpdate)
            {
                SwEditorLogger.Log("Skipping update verification.");
                SwEditorTracker.TrackEditorEvent(nameof(OnVerificationRequested), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Info, "Skipping update verification.");

                return;
            }
            
            if (SwSelfUpdate.VerifySuccessfulUpdate())
            {
                HandlePostImport(true, backupFolderPath, unityPackageFilePath);

                return;
            }

            SwEditorLogger.LogWarning("Update verification failed.");
            SwEditorTracker.TrackEditorEvent(nameof(OnVerificationRequested), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Warning, SwEditorConstants.ErrorMessages.FAILED_TO_VERIFY_SUCCESSFUL_UPDATE);
            SwSelfUpdateUiManager.ShowUpdateFailed(SwEditorConstants.ErrorMessages.FAILED_TO_VERIFY_SUCCESSFUL_UPDATE);
            HandlePostImport(false, backupFolderPath, unityPackageFilePath);
        }

        private static void HandlePostImport(bool didImport, string backUpFolderPath, string unityPackageFilePath)
        {
            try
            {
                if (didImport)
                {
                    SwSelfUpdateFileManagement.DeleteWisdomBackUpFolder();
                    EditorPrefs.DeleteKey(SwSelfUpdate.CHECK_FOR_UPDATE_GRACEFUL_FIRST_RUN_KEY);
                    return;
                }

                RestorePreviousVersion(backUpFolderPath);
            }
            
            catch (Exception ex)
            {
                SwEditorLogger.LogError($"Exception occurred while handling post import: {ex}");
            }
            finally
            {
                FindAndDeleteUnityPackage(unityPackageFilePath);
            }
        }

        private static void RestorePreviousVersion(string backUpFolderPath)
        {
            if (string.IsNullOrEmpty(backUpFolderPath))
            {
                if (!SwSelfUpdateFileManagement.TryFindingBackupFolder(out backUpFolderPath))
                {
                    SwEditorLogger.LogError("Failed to find backup folder");
                    return;
                }
            }

            SwEditorLogger.Log("Restoring due to Wisdom update failure...");
            SwEditorTracker.TrackEditorEvent(nameof(HandlePostImport), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Error, "Restoring due to Wisdom update failure...");

            if (SwSelfUpdateFileManagement.RestorePackageFromFolder(backUpFolderPath))
            {
                SwEditorLogger.Log("... restored.");
            }
            else
            {
                SwEditorLogger.LogError("... failed restore!");
            }
        }

        private static void FindAndDeleteUnityPackage(string unityPackageFilePath)
        {
            if (!unityPackageFilePath.SwIsNullOrEmpty()) return;
            
            SwEditorLogger.LogWarning("Unity Package File Path is null or empty");
            SwSelfUpdateFileManagement.TryFindingUnityPathFile(out unityPackageFilePath);
                
            if (!unityPackageFilePath.SwIsNullOrEmpty())
            {
                SwFileUtils.DeleteFileAtPath(unityPackageFilePath);
            }
        }

        #endregion
    }
}