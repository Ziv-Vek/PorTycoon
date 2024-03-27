using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    public class SwPreferencesWindow : SettingsProvider
    {
        #region --- Constants ---
        
        private const string LOG_EDITOR_TRACKER_KEY = "SwLogEditor";
        private const float LABEL_WIDTH = 200f;
        private const float TOGGLE_WIDTH = 50f;
        private const float SECTION_SPACE = 10f;
        
        #endregion
        
      
        #region --- Members ---
        
        private static bool _logEditorTracker;
        
        #endregion
        
        
        #region --- Constructor ---
        
        private SwPreferencesWindow(string path, SettingsScope scope = SettingsScope.User) : base(path, scope) 
        {
            // Initialize LogEditorTracker on construction.
            if (!EditorPrefs.HasKey(LOG_EDITOR_TRACKER_KEY))
            {
                EditorPrefs.SetBool(LOG_EDITOR_TRACKER_KEY, false);
            }
            _logEditorTracker = EditorPrefs.GetBool(LOG_EDITOR_TRACKER_KEY);
        }

        #endregion
        
        
        #region --- Properties ---
        
        public static bool LogEditorTracker => _logEditorTracker;
        
        #endregion
        
        
        #region --- Public Methods ---
        
        [SettingsProvider]
        public static SettingsProvider Create()
        {
            return new SwPreferencesWindow("Preferences/Supersonic Wisdom");
        }

        public override void OnGUI(string searchContext)
        {
            DrawEditorLogging();
            GUILayout.Space(SECTION_SPACE);
        }
        
        #endregion
        
        
        #region --- Private Methods ---

        private static void DrawEditorLogging()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.LabelField("Editor Logging", EditorStyles.boldLabel);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Log Editor Events", GUILayout.Width(LABEL_WIDTH));
                
                var loggingEditorTracker = EditorGUILayout.Toggle(_logEditorTracker, GUILayout.Width(TOGGLE_WIDTH));
                EditorGUILayout.EndHorizontal();
                DrawSmallHelperLabel(
                    "Disabling this prevents from info and warning logs from Wisdom Editor to appear.");
                if (EditorGUI.EndChangeCheck())
                {
                    EditorPrefs.SetBool(LOG_EDITOR_TRACKER_KEY, loggingEditorTracker);
                    // Update local cache whenever preferences update.
                    _logEditorTracker = loggingEditorTracker;
                }
            }
        }
        
        private static void DrawSmallHelperLabel(string text) 
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField(text, EditorStyles.miniLabel);
            EditorGUI.indentLevel--;
        }
        
        #endregion
    }
}