using System;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwIosCoreSettingsTab : SwCoreSettingsTab
    {
        #region --- Members ---

        protected readonly GUIContent IosAppIdLabel = new GUIContent("Apple App ID", "Your iOS App ID");
        protected readonly GUIContent IosGameIdLabel = new GUIContent("Supersonic ID", "Your iOS Game ID");

        #endregion


        #region --- Construction ---

        public SwIosCoreSettingsTab(SerializedObject soSettings) : base(soSettings)
        { }

        #endregion


        #region --- Private Methods ---

        protected internal override void DrawContent()
        {
            GUILayout.Space(SPACE_BETWEEN_FIELDS * 3);
            //Draw Supersonic iOS Game ID
            GUILayout.BeginHorizontal();
            GUILayout.Label(IosGameIdLabel, GUILayout.Width(LABEL_WIDTH));
            Settings.iosGameId = TextFieldWithoutSpaces(Settings.iosGameId);
            GUILayout.EndHorizontal();
        }

        protected internal override string Name ()
        {
            return "iOS";
        }

        #endregion
    }
}