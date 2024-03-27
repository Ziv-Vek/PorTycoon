using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwAndroidCoreSettingsTab : SwCoreSettingsTab
    {
        #region --- Members ---

        private readonly GUIContent _androidGameIdLabel = new GUIContent("Supersonic ID", "Your Android Game  ID");

        #endregion


        #region --- Construction ---

        public SwAndroidCoreSettingsTab(SerializedObject soSettings) : base(soSettings)
        { }

        #endregion


        #region --- Private Methods ---

        protected internal override void DrawContent ()
        {
            //Draw Supersonic Android Game ID
            GUILayout.Space(SPACE_BETWEEN_FIELDS * 3);
            GUILayout.BeginHorizontal();
            GUILayout.Label(_androidGameIdLabel, GUILayout.Width(LABEL_WIDTH));
            Settings.AndroidGameId = TextFieldWithoutSpaces(Settings.androidGameId);
            GUILayout.EndHorizontal();
        }

        protected internal override string Name ()
        {
            return "Android";
        }

        #endregion
    }
}