using System.Collections.Generic;
using UnityEditorInternal;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwSelfUpdateUiManager
    {
        #region --- Messages ---

        public static readonly UiAlertOperation UpdateInProgressOperation = new UiAlertOperation
        {
            Message = SwEditorConstants.UI.UPDATE_IN_PROGRESS,
            ButtonTitle = SwEditorConstants.UI.ButtonTitle.OK,
        };

        public static readonly UiAlertOperation UpdateCheckFailedOperation = new UiAlertOperation
        {
            Message = SwEditorConstants.UI.WISDOM_UPDATE_CHECK_FAILED,
            ButtonTitle = SwEditorConstants.UI.ButtonTitle.CLOSE,
            ErrorCode = (int)SwErrors.ESelfUpdate.CheckUpdatesFailed,
        };

        public static readonly UiAlertOperation UpdateSuccessful = new UiAlertOperation
        {
            Message = SwEditorConstants.UI.WISDOM_UPDATED_SUCCESSFULLY,
            ButtonTitle = SwEditorConstants.UI.ButtonTitle.AWESOME,
        };

        public static readonly UiAlertOperation NoUpdateAvailable = new UiAlertOperation
        {
            Message = SwEditorConstants.UI.NO_NEED_TO_UPDATE,
            ButtonTitle = SwEditorConstants.UI.ButtonTitle.THANKS,
        };

        public static readonly UiAlertOperation UpdateCheckFailedDueToEmptyGamesList = new UiAlertOperation
        {
            Message = SwEditorConstants.UI.CANT_CHECK_UPDATES_EMPTY_GAMES_LIST,
            ButtonTitle = SwEditorConstants.UI.ButtonTitle.OK,
            ErrorCode = (int)SwErrors.ESelfUpdate.CheckFailedDueToEmptyGamesList,
        };

        public static readonly UiAlertOperation ChecksumMismatch = new UiAlertOperation
        {
            Message = SwEditorConstants.UI.FAILED_TO_DOWNLOAD_UNITY_PACKAGE,
            ButtonTitle = SwEditorConstants.UI.ButtonTitle.CLOSE,
            ErrorCode = (int)SwErrors.ESelfUpdate.DownloadChecksumMismatch,
        };

        #endregion


        #region --- Public Methods ---

        internal static void ShowUpdateConfirmation()
        {
            var currentVersionId = SwSelfUpdateConfiguration.CurrentVersionId;
            var currentStage = SwSelfUpdateConfiguration.CurrentStage;
            var updatedVersionId = SwSelfUpdate.SelfUpdateConfiguration.UpdatedVersionId;
            var updatedStage = SwSelfUpdate.SelfUpdateConfiguration.UpdatedStage;

            var shouldUpdateStage = SwSelfUpdate.SelfUpdateConfiguration.ShouldUpdateStage;
            var shouldUpdateVersion = SwSelfUpdate.SelfUpdateConfiguration.ShouldUpdateVersion;

            const string integrationGuideUrlKey = "integrationGuideUrl";
            const string changeLogUrlKey = "changeLogsUrl";

            var stageKey = $"{currentStage}-{updatedStage}";
            var stageUpdateConfiguration = SwSelfUpdate.SelfUpdateConfiguration.StageUpdate;
            stageUpdateConfiguration = new SwJsonDictionary(SwJsonParser.Deserialize(stageUpdateConfiguration.SwSafelyGet(stageKey, null)?.ToString()) as Dictionary<string, object>);
            var bothConfiguration = SwSelfUpdate.SelfUpdateConfiguration.StageAndVersionUpdate;
            var versionUpdateConfiguration = SwSelfUpdate.SelfUpdateConfiguration.VersionUpdate;

            var messageTitle = (string)stageUpdateConfiguration.SwSafelyGet("messageTitle", null);
            var updateButtonTitle = (string)stageUpdateConfiguration.SwSafelyGet("updateButtonTitle", null);
            var updateButtonTip = (string)stageUpdateConfiguration.SwSafelyGet("updateButtonTip", null);
            var changeLogUrl = (string)versionUpdateConfiguration.SwSafelyGet(changeLogUrlKey, SwEditorConstants.DEFAULT_CHANGE_LOG_URL);
            var releaseNotesLinkTitle = (string)versionUpdateConfiguration.SwSafelyGet("releaseNotesLinkTitle", "Release notes");

            // In addition, we noticed there is a new and updated SDK for you
            var integrationGuideButtonTitle = (string)stageUpdateConfiguration.SwSafelyGet("integrationGuideButtonTitle", "Integration Guide");
            var integrationGuideButtonTip = (string)stageUpdateConfiguration.SwSafelyGet("integrationGuideButtonTip", "Link to Wisdom integration guide");
            var integrationGuideUrl = (string)stageUpdateConfiguration.SwSafelyGet(integrationGuideUrlKey, SwEditorConstants.DEFAULT_STAGE_INTEGRATION_GUIDE_URL);
            SwEditorUtils.SwAccountData.IntegrationGuideUrl = integrationGuideUrl;

            var additionalDescription = (string)bothConfiguration.SwSafelyGet("additionalDescription", string.Empty);
            string defaultMessageBody;
            SwJsonDictionary messageBodyConfig;

            if (shouldUpdateStage && shouldUpdateVersion) // Both
            {
                defaultMessageBody = $"Good News!\nYour game, {SwSelfUpdateWindow.APP_NAME_PLACEHOLDER}, has advanced to the next level.\nIn addition, we noticed there is a new and updated SDK for you.\n\nUpgrade your Wisdom package from version {SwSelfUpdateWindow.CURRENT_VERSION_PLACEHOLDER} to {SwSelfUpdateWindow.UPDATED_VERSION_PLACEHOLDER}.{SwSelfUpdateWindow.CHANGE_LOGS_LINK_PLACEHOLDER}\nAdd functionalities needed to keep progression using the integration guide.";
                messageBodyConfig = bothConfiguration;
            }
            else if (shouldUpdateVersion) // `shouldUpdateVersion` only
            {
                integrationGuideUrl = string.Empty;
                defaultMessageBody = $"We noticed there is a new and updated SDK for you.\nUpgrade your Wisdom package from version {SwSelfUpdateWindow.CURRENT_VERSION_PLACEHOLDER} to {SwSelfUpdateWindow.UPDATED_VERSION_PLACEHOLDER}.{SwSelfUpdateWindow.CHANGE_LOGS_LINK_PLACEHOLDER}";
                messageBodyConfig = versionUpdateConfiguration;
            }
            else // `shouldUpdateStage` only
            {
                changeLogUrl = string.Empty; // clear "change logs" link
                defaultMessageBody = $"Good News!\nYour game, {SwSelfUpdateWindow.APP_NAME_PLACEHOLDER}, has advanced to the next level.\nUpgrade your Wisdom package to add functionalities needed to keep progressing.";
                messageBodyConfig = stageUpdateConfiguration;
            }

            var messageBody = messageBodyConfig.SwSafelyGet("messageBody", defaultMessageBody) as string ?? string.Empty;

            var remindLaterButtonTitle = (string)versionUpdateConfiguration.SwSafelyGet("remindLaterButtonTitle", "Remind me later");
            var remindLaterButtonTip = (string)versionUpdateConfiguration.SwSafelyGet("remindLaterButtonTip", "We will let you know again after " + SwSelfUpdateWindow.CHECK_INTERVAL_HOURS_PLACEHOLDER);

            var currentVersion = SwUtils.System.ComputeVersionString(currentVersionId);
            var updatedVersion = SwUtils.System.ComputeVersionString(updatedVersionId);

            SwSelfUpdateWindow.ShowNew(updateButtonTitle,
                updateButtonTip,
                messageTitle,
                messageBody,
                integrationGuideButtonTitle,
                integrationGuideButtonTip,
                additionalDescription,
                integrationGuideUrl,
                changeLogUrl,
                releaseNotesLinkTitle,
                remindLaterButtonTitle,
                remindLaterButtonTip,
                currentVersion,
                updatedVersion,
                (selfUpdateWindow, didUserAgree) =>
                {
                    if (didUserAgree)
                    {
                        SwSelfUpdate.OnUserClickedUpdate(selfUpdateWindow);
                    }
                });
        }

        internal static void ShowUpdateFailed(string errorMessage)
        {
            if (!Alert(SwEditorConstants.UI.FAILED_TO_UPDATE_WISDOM_PACKAGE(errorMessage), SwEditorConstants.UI.ButtonTitle.CLOSE, SwErrors.ESelfUpdate.ImportFailed))
            {
                return;
            }

            SwSelfUpdateWindow.GetIfPresented()?.Focus();
            InternalEditorUtility.RepaintAllViews();
        }

        internal static bool Alert(UiAlertOperation operation)
        {
            return operation.ErrorCode.HasValue ? SwEditorAlerts.AlertError(operation.Message, operation.ErrorCode.Value, operation.ButtonTitle) : SwEditorAlerts.Alert(operation.Message, operation.ButtonTitle);
        }

        internal static bool Alert(string customMessage, string buttonTitle, SwErrors.ESelfUpdate errorCode)
        {
            var uiAlertOperation = new UiAlertOperation
            {
                Message = customMessage,
                ButtonTitle = buttonTitle,
                ErrorCode = (int)errorCode,
            };

            return Alert(uiAlertOperation);
        }

        #endregion


        #region --- Internal Class ---

        internal static class UiMessages
        {


            #region --- Constants ---

            public const string SW_UPDATE_BACKUP_STEP = "backing up files";
            public const string SW_UPDATE_DELETION_STEP = "deleting current version files";
            public const string SW_UPDATE_IMPORT_STEP = "importing...";

            #endregion


        }

        internal class UiAlertOperation
        {


            #region --- Members ---

            public string Message { get; set; }
            public string ButtonTitle { get; set; }
            public int? ErrorCode { get; set; }

            #endregion


        }

        #endregion
    }
}