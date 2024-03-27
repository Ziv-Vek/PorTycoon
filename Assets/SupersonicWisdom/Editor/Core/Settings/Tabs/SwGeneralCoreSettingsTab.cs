using System.Linq;
using UnityEditor;
using UnityEngine;
using static System.String;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwGeneralCoreSettingsTab : SwCoreSettingsTab
    {
        #region --- Constants ---

        private const string _accountEmailLabel = "Email";
        private const string _accountLoginLabel = "Login";
        private const string _accountLogoutLabel = "Logout";
        private const string _accountPasswordLabel = "Password";
        private const string _accountRefreshGameListLabel = "Refresh Game list";
        private const string _accountRetrieveConfiguration = "Retrieve configuration";
        private const string _accountStatusLabel = "Status";
        private const string _accountStatusText = "Logged in";

        private const string _accountTitleLabel = "Account";

        #endregion


        #region --- Members ---

        private static string _accountPassword;

        #endregion


        #region --- Construction ---

        public SwGeneralCoreSettingsTab(SerializedObject soSettings) : base(soSettings)
        { }

        #endregion


        #region --- Private Methods ---

        protected internal override void DrawContent ()
        {
            GUILayout.Space(SPACE_BETWEEN_FIELDS * 5);
            DrawAccountFields();
            GUILayout.Space(SPACE_BETWEEN_FIELDS * 2);
        }

        protected internal override string Name ()
        {
            return "General";
        }

        private void SaveToSettings()
        {
            if (SwAccountUtils.IsSelectedTitleDummy)
            {
                SwEditorAlerts.AlertError(SwEditorConstants.UI.PLEASE_CHOOSE_TITLE, (int)SwErrors.ESettings.ChooseTitle, SwEditorConstants.UI.ButtonTitle.OK);

                return;
            }

            SwAccountUtils.SaveSelectedTitleGamesToSettings();
            SwEditorAlerts.TitleSavedToSettingsSuccess();
        }

        protected virtual void OnTitleSelected()
        {
            // Should be handled on subclasses
        }

        private void DrawAccountFields ()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(_accountTitleLabel, EditorStyles.largeLabel);
            GUILayout.EndHorizontal();

            if (SwAccountUtils.IsLoggedIn)
            {
                DrawAccountTools();
            }
            else
            {
                DrawAccountLogin();
            }
        }

        private void DrawAccountLogin ()
        {
            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            GUILayout.BeginHorizontal();
            GUILayout.Label(_accountEmailLabel, GUILayout.Width(LABEL_WIDTH));
            Settings.accountEmail = TextFieldWithoutSpaces(Settings.accountEmail, alwaysEnable: true);
            GUILayout.EndHorizontal();

            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            GUILayout.BeginHorizontal();
            GUILayout.Label(_accountPasswordLabel, GUILayout.Width(LABEL_WIDTH));
            _accountPassword = EditorGUILayout.PasswordField("", _accountPassword);
            GUILayout.EndHorizontal();

            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(_accountLoginLabel, GUILayout.Width(80)))
            {
                if (IsNullOrEmpty(_accountPassword))
                {
                    SwEditorAlerts.AlertError(SwEditorConstants.UI.MISSING_PASSWORD, (int)SwErrors.ESettings.MissingPassword, SwEditorConstants.UI.ButtonTitle.CLOSE);

                    return;
                }

                if (!Settings.accountEmail.SwIsValidEmailAddress())
                {
                    SwEditorAlerts.AlertError(SwEditorConstants.UI.INVALID_EMAIL_ADDRESS, (int)SwErrors.ESettings.InvalidEmail, SwEditorConstants.UI.ButtonTitle.CLOSE);

                    return;
                }

                SwAccountUtils.Login(Settings.accountEmail, _accountPassword);
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(SPACE_BETWEEN_FIELDS * 2);

            DrawHorizontalLine();
        }

        private void DrawAccountTools ()
        {
            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            GUILayout.BeginHorizontal();
            GUILayout.Label(_accountEmailLabel, GUILayout.Width(LABEL_WIDTH));
            GUILayout.Label(Settings.accountEmail);
            GUILayout.EndHorizontal();

            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            GUILayout.BeginHorizontal();
            GUILayout.Label(_accountStatusLabel, GUILayout.Width(LABEL_WIDTH));
            GUILayout.Label(_accountStatusText);
            GUILayout.EndHorizontal();

            GUILayout.Space(SPACE_BETWEEN_FIELDS * 3);
            GUILayout.BeginHorizontal();
            var isTitleSelectedAutomatically = Settings.isTitleSelectedAutomatically;

            if (isTitleSelectedAutomatically)
            {
                GUI.enabled = false;
            }

            var names = SwAccountUtils.TitlesList?.Select(game => game.name).ToArray();
            Settings.selectedGameIndex = EditorGUILayout.Popup(Settings.selectedGameIndex, names, GUILayout.Width(LABEL_WIDTH));

            if (isTitleSelectedAutomatically)
            {
                GUI.enabled = true;

                if (GUILayout.Button(_accountRetrieveConfiguration, GUILayout.Width(LABEL_WIDTH)))
                {
                    SwAccountUtils.FetchTitles();
                }

                GUILayout.EndHorizontal();
            }
            else
            {
                if (GUILayout.Button(_accountRetrieveConfiguration, GUILayout.Width(LABEL_WIDTH)))
                {
                    if (SwAccountUtils.IsSelectedTitleDummy)
                    {
                        SwEditorAlerts.AlertError(SwEditorConstants.UI.PLEASE_CHOOSE_TITLE, (int)SwErrors.ESettings.ChooseTitle, SwEditorConstants.UI.ButtonTitle.OK);

                        return;
                    }
                    
                    OnTitleSelected();
                    SaveToSettings();
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(_accountRefreshGameListLabel, GUILayout.Width(LABEL_WIDTH)))
                {
                    SwAccountUtils.FetchTitles();
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(SPACE_BETWEEN_FIELDS);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(_accountLogoutLabel, GUILayout.Width(80)))
            {
                _accountPassword = Empty;

                SwAccountUtils.Logout(Settings);
                DrawContent();
            }

            GUILayout.EndHorizontal();

            GUILayout.Space(SPACE_BETWEEN_FIELDS * 2);

            DrawHorizontalLine();
        }

        private void OnLoginFail(SwEditorError error)
        {
            SwEditorAlerts.AlertError(error.ErrorMessage, (int)SwErrors.ESettings.LoginEndpoint, SwEditorConstants.UI.ButtonTitle.CLOSE);
        }

        #endregion
    }
}