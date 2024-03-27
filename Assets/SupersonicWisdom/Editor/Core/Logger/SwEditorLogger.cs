using System;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwEditorLogger
    {
        #region --- Constants ---

        private const string Tag = "SupersonicWisdom";

        #endregion
        
        #region --- Private Methods ---

        internal static void Log(string message)
        {
            if (!SwPreferencesWindow.LogEditorTracker) return;
            
            Debug.Log($"{Tag}: {message}");
        }

        internal static void LogError(string message)
        {
            Debug.LogError($"{Tag}: {message}");
        }

        internal static void LogError(Exception exceptionMessage)
        {
            Debug.LogError($"{Tag}: {exceptionMessage}");
        }

        internal static void LogWarning(string message)
        {
            if (!SwPreferencesWindow.LogEditorTracker) return;
            
            Debug.LogWarning($"{Tag}: {message}");
        }

        #endregion
    }
}