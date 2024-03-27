using UnityEditor;
using UnityEngine;
using System.Collections;

namespace SupersonicWisdomSDK.Editor
{
    public static class WelcomeMessageUtils
    {
        #region --- Members ---

        private static bool _isWelcomeMessageShowed;

        #endregion


        #region --- Public Methods ---

        public static void TryShowWelcomeMessage()
        {
            if (SwEditorUtils.SwSettings != null || _isWelcomeMessageShowed) return;

            _isWelcomeMessageShowed = true;

            SwEditorCoroutines.StartEditorCoroutine(SwEditorUtils.WaitUntilEndOfCompilation(), false, SwEditorAlerts.AlertWelcomeMessage);
        }

        #endregion
    }
}