using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace SupersonicWisdomSDK.Editor
{
    public class SwSelfUpdateWindow : EditorWindow
    {
        #region --- Constants ---

        private const float WINDOW_WIDTH = 320;
        internal const string APP_NAME_PLACEHOLDER = "{appName}";
        internal const string CHECK_INTERVAL_HOURS_PLACEHOLDER = "{selfCheckIntervalHours}";
        internal const string CURRENT_VERSION_PLACEHOLDER = "{currentVersion}";
        internal const string UPDATED_VERSION_PLACEHOLDER = "{updatedVersion}";
        internal const string CHANGE_LOGS_LINK_PLACEHOLDER = "{releaseNotesLink}";
        private const string BUTTON_TITLE_PLACEHOLDER = "{buttonTitle}";

        #endregion


        #region --- Members ---

        internal bool IsUpdateButtonEnabled;

        private Action<SwSelfUpdateWindow, bool> _onUpdateButtonSelected;
        private GUIContent _downloadButtonContent;

        private GUIContent _integrationGuideButtonContent;
        private string _additionalDescription;
        private string _integrationGuideUrl;

        private GUIContent _remindMeLaterButtonContent;
        private string _changeLogsUrl;
        private string _releaseNotesLinkTitle;
        private string _messageBody;
        private Vector2 _scrollPosition = Vector2.up;
        private static bool _isPresented;

        #endregion


        #region --- Mono Override ---

        private void OnDestroy()
        {
            _isPresented = false;
            var onDismiss = _onUpdateButtonSelected;
            _onUpdateButtonSelected = null;
            onDismiss?.Invoke(this, false);
        }

        private void OnGUI()
        {
            _isPresented = true;

            DrawLogoAndBodySideBySide();
            DrawDescriptionAndActionButtons();
        }

        private void DrawDescriptionAndActionButtons()
        {
            GUILayout.BeginVertical();

            if (!string.IsNullOrEmpty(_additionalDescription))
            {
                EditorGUILayout.Space();
                GUILayout.Label(_additionalDescription, EditorStyles.wordWrappedMiniLabel, GUILayout.MaxWidth(WINDOW_WIDTH));
                EditorGUILayout.Space();
            }

            DrawActionButtons();

            GUILayout.EndVertical();
        }

        private void DrawActionButtons()
        {
            GUI.enabled = IsUpdateButtonEnabled;

            if (GUILayout.Button(_downloadButtonContent))
            {
                _onUpdateButtonSelected?.Invoke(this, true);
            }

            if (!IsUpdateButtonEnabled)
            {
                GUI.enabled = true;
            }

            if (!string.IsNullOrEmpty(_integrationGuideUrl))
            {
                if (GUILayout.Button(_integrationGuideButtonContent))
                {
                    Application.OpenURL(_integrationGuideUrl);
                }
            }
            else if (!string.IsNullOrEmpty(_changeLogsUrl))
            {
                // Only when it's only version update
                if (GUILayout.Button(_remindMeLaterButtonContent))
                {
                    _isPresented = false;
                    Close();
                }
            }
        }

        private void DrawLogoAndBodySideBySide()
        {
            GUILayout.BeginHorizontal();
            SwEditorUtils.DrawGuiLayoutSwLogoLabel(Constants.UI.SW_LOGO_DIMENSIONS);
            DrawBody();
            GUILayout.EndHorizontal();
        }

        private void DrawBody()
        {
            GUILayout.BeginVertical();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false, GUILayout.MinWidth(0));
            var bodyComponents = _messageBody.SwSplit(CHANGE_LOGS_LINK_PLACEHOLDER);

            if (bodyComponents.Length > 1 && !string.IsNullOrEmpty(_changeLogsUrl))
            {
                DrawClickableBody(bodyComponents, _releaseNotesLinkTitle, _changeLogsUrl);
            }
            else
            {
                GUILayout.Label(_messageBody, EditorStyles.wordWrappedLabel);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            GUILayout.EndVertical();
        }

        private void DrawClickableBody(string[] bodyComponents, string linkTitle, string linkUrl)
        {
            var bodyComponentsList = bodyComponents.ToList();
            GUILayout.Label(bodyComponents[0], EditorStyles.wordWrappedLabel);
            bodyComponentsList.RemoveAt(0);
            SwEditorUtils.DrawClickableButtonLink(linkTitle, linkUrl, () => Application.OpenURL(linkUrl));
            GUILayout.Label(string.Join("\n", bodyComponentsList), EditorStyles.wordWrappedLabel);
        }

        #endregion


        #region --- Public Methods ---

        public static void CloseWindow()
        {
            SwEditorUtils.RunOnMainThread(() =>
            {
                try
                {
                    GetIfPresented()?.Close();
                    _isPresented = false;
                }
                catch (Exception e)
                {
                    SwEditorLogger.LogError(e);
                }
            });
        }

        public static SwSelfUpdateWindow ShowNew(string updateButtonTitle, string updateButtonTip, string messageTitle, string messageBody, string integrationGuideButtonTitle, string integrationGuideButtonTip, string additionalDescription, string integrationGuideUrl, string changeLogUrl, string releaseNotesLinkTitle, string remindLaterButtonTitle, string remindLaterButtonTip, string currentVersion, string updatedVersion, Action<SwSelfUpdateWindow, bool> onUpdateButtonSelected)
        {
            var appName = SwEditorUtils.AppName;
            var buttonTitle = updateButtonTitle ?? Constants.UI.UPGRADE_BUTTON_TITLE;

            var updateWindow = (SwSelfUpdateWindow)GetWindow(typeof(SwSelfUpdateWindow), true);

            updateWindow.position = new Rect(150, 150, 450, 200);
            updateWindow.titleContent = new GUIContent((messageTitle ?? Constants.UI.WINDOW_TITLE).Replace(APP_NAME_PLACEHOLDER, appName).Replace(BUTTON_TITLE_PLACEHOLDER, buttonTitle));

            updateWindow.IsUpdateButtonEnabled = onUpdateButtonSelected != null;
            updateWindow._onUpdateButtonSelected = onUpdateButtonSelected;
            updateWindow._downloadButtonContent = new GUIContent(buttonTitle, updateButtonTip ?? Constants.UI.DOWNLOAD_BUTTON_TITLE);
            updateWindow._messageBody = messageBody.Replace(APP_NAME_PLACEHOLDER, appName).Replace(BUTTON_TITLE_PLACEHOLDER, buttonTitle).Replace(CURRENT_VERSION_PLACEHOLDER, currentVersion).Replace(UPDATED_VERSION_PLACEHOLDER, updatedVersion);

            updateWindow._additionalDescription = (additionalDescription ?? "").Replace(APP_NAME_PLACEHOLDER, appName).Replace(BUTTON_TITLE_PLACEHOLDER, buttonTitle);

            updateWindow._integrationGuideUrl = integrationGuideUrl;
            updateWindow._integrationGuideButtonContent = new GUIContent(integrationGuideButtonTitle, integrationGuideButtonTip);

            updateWindow._changeLogsUrl = changeLogUrl;
            updateWindow._releaseNotesLinkTitle = releaseNotesLinkTitle ?? Constants.UI.RELEASE_NOTES_BUTTON_TITLE;

            var hours = SwSelfUpdate.RECURRING_CHECK_UPDATES_DURATION_SECONDS / SwSelfUpdate.TOTAL_SECONDS_IN_ONE_HOUR;
            updateWindow._remindMeLaterButtonContent = new GUIContent(remindLaterButtonTitle.Replace(CHECK_INTERVAL_HOURS_PLACEHOLDER, $"{SwEditorUtils.Plural(hours, "hour")}"), remindLaterButtonTip.Replace(CHECK_INTERVAL_HOURS_PLACEHOLDER, $"{SwEditorUtils.Plural(hours, "hour")}"));

            updateWindow.Show();
            
            SwEditorTracker.TrackEditorEvent(nameof(ShowNew), ESwEditorWisdomLogType.SelfUpdate, ESwEventSeverity.Info, "Self update window presented");

            return updateWindow;
        }

        private struct Constants
        {
            public struct UI
            {
                public const string RELEASE_NOTES_BUTTON_TITLE = "Release notes";
                public const string UPGRADE_BUTTON_TITLE = "Upgrade";
                public const float SW_LOGO_DIMENSIONS = 64;
                public const string WINDOW_TITLE = "Supersonic Wisdom Upgrade Available";
                public const string DOWNLOAD_BUTTON_TITLE = "click to start downloading and importing the new package";
            }
        }

        public static SwSelfUpdateWindow GetIfPresented()
        {
            return _isPresented ? GetWindow<SwSelfUpdateWindow>() : null;
        }

        #endregion
    }
}