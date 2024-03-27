#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwIosStage10SettingsTab : SwIosCoreSettingsTab
    {
        #region --- Members ---

        protected readonly GUIContent GameAnalyticsIosGameKeyLabel = new GUIContent("GameAnalytics Game Key", "Your GameAnalytics iOS Game Key");
        protected readonly GUIContent GameAnalyticsIosSecretKeyLabel = new GUIContent("GameAnalytics Secret Key", "Your GameAnalytics iOS Secret Key");

        #endregion


        #region --- Construction ---

        public SwIosStage10SettingsTab(SerializedObject soSettings) : base(soSettings)
        { }

        #endregion


        #region --- Private Methods ---

        protected internal override void DrawContent ()
        {
            base.DrawContent();

            //Draw Apple App ID
            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            GUILayout.BeginHorizontal();
            GUILayout.Label(IosAppIdLabel, GUILayout.Width(LABEL_WIDTH));
            Settings.iosAppId = TextFieldWithoutSpaces(Settings.iosAppId);
            GUILayout.EndHorizontal();

            DrawGameAnalytics();
        }

        private void DrawGameAnalytics ()
        {
            //Draw GA iOS Game Key
            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            GUILayout.BeginHorizontal();
            var (iosGameAnalyticsGameKey, iosGameAnalyticsSecretKey) = SwStage10EditorUtils.GetGameAnalyticsKeys(RuntimePlatform.IPhonePlayer);
            GUILayout.Label(GameAnalyticsIosGameKeyLabel, GUILayout.Width(LABEL_WIDTH));

            if (!Settings.iosChinaBuildEnabled)
            {
                if (string.IsNullOrEmpty(Settings.iosGameAnalyticsGameKey))
                {
                    Settings.iosGameAnalyticsGameKey = iosGameAnalyticsGameKey;
                }

                if (string.IsNullOrEmpty(Settings.iosGameAnalyticsSecretKey))
                {
                    Settings.iosGameAnalyticsSecretKey = iosGameAnalyticsSecretKey;
                }
            }

            Settings.iosGameAnalyticsGameKey = TextFieldWithoutSpaces(Settings.iosGameAnalyticsGameKey);
            GUILayout.EndHorizontal();

            //Draw GA iOS Game Secret
            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            GUILayout.BeginHorizontal();
            GUILayout.Label(GameAnalyticsIosSecretKeyLabel, GUILayout.Width(LABEL_WIDTH));
            Settings.iosGameAnalyticsSecretKey = TextFieldWithoutSpaces(Settings.iosGameAnalyticsSecretKey);
            GUILayout.EndHorizontal();

            if (!Settings.iosChinaBuildEnabled && (!Settings.iosGameAnalyticsGameKey.Equals(iosGameAnalyticsGameKey) || !Settings.iosGameAnalyticsSecretKey.Equals(iosGameAnalyticsSecretKey)))
            {
                SwStage10EditorUtils.SetGameAnalyticsKeys(RuntimePlatform.IPhonePlayer, Settings.iosGameAnalyticsGameKey, Settings.iosGameAnalyticsSecretKey);
            }
        }

        #endregion
    }
}
#endif