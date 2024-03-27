using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwDebugCoreSettingsTab : SwCoreSettingsTab
    {
        #region --- Members ---

        private readonly GUIContent _enableDebugLabel = new GUIContent("  Debug Wisdom SDK", "Enable Debug for SDK");
        private readonly GUIContent _enableDevtoolsLabel = new GUIContent("  Enable Devtools", "True by default for debug builds");
        private readonly GUIContent _logViaNetworkLabel = new GUIContent("  Log via network", "Send logs to http://supersonic-wisdom.log");
        private readonly GUIContent _testBlockingApiInvocationLabel = new GUIContent("  Test Blocking API Invocation", "Test blocking api invocations by showing a message upon each invocation.");

        #endregion


        #region --- Construction ---

        public SwDebugCoreSettingsTab(SerializedObject soSettings) : base(soSettings)
        { }

        #endregion


        #region --- Private Methods ---

        protected internal override void DrawContent ()
        {
            GUILayout.Space(SPACE_BETWEEN_FIELDS * 3);
            //Draw Enable Debug checkbox
            GUILayout.BeginHorizontal();
            Settings.enableDebug = GUILayout.Toggle(Settings.enableDebug, _enableDebugLabel);
            GUILayout.EndHorizontal();

            //Draw Test Blocking Api Invocation checkbox
            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            GUILayout.BeginHorizontal();
            Settings.testBlockingApiInvocation = GUILayout.Toggle(Settings.testBlockingApiInvocation, _testBlockingApiInvocationLabel);
            GUILayout.EndHorizontal();


            //Draw Enable Devtools
            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            GUILayout.BeginHorizontal();
            Settings.enableDevtools = GUILayout.Toggle(Settings.enableDevtools, _enableDevtoolsLabel);
            GUILayout.EndHorizontal();

#if SUPERSONIC_WISDOM_TEST
            GUILayout.Space(SPACE_BETWEEN_FIELDS);
            GUILayout.BeginHorizontal();
            Settings.logViaNetwork = GUILayout.Toggle(Settings.logViaNetwork, _logViaNetworkLabel);
            GUILayout.EndHorizontal();
#endif
        }

        protected internal override string Name ()
        {
            return "Debug";
        }

        #endregion
    }
}