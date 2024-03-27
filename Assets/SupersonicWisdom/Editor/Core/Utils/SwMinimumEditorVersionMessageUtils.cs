using System;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwMinimumEditorVersionMessageUtils
    {
        #region --- Constants ---

        private const int MINIMUM_EDITOR_VERSION_MAJOR = 2019;
        private const int HOURS_BETWEEN_SHOWING_MESSAGE = 24;
        private const int CURRENTLY_SUPPORTED_LTS_MAJOR = 2021;

        #endregion


        #region --- Members ---

        private static bool _hasCompletedSetup;

        #endregion


        #region --- Private Methods ---

        private static void ShowMinimumEditorVersionPopup()
        {
            if (EditorUtility.DisplayDialog(SwEditorConstants.UI.SUPERSONIC_WISDOM_SDK, SwEditorConstants.UI.UNITY_MINIMUM_VERSION_MESSAGE.Format(CURRENTLY_SUPPORTED_LTS_MAJOR), SwEditorConstants.UI.ButtonTitle.UPDATE, SwEditorConstants.UI.ButtonTitle.OK))
            {
                Application.OpenURL(SwEditorConstants.UI.MINIMUM_EDITOR_VERSION_URL.Format(CURRENTLY_SUPPORTED_LTS_MAJOR));
            }
        }

        private static bool IsWisdomSupportingThisUnityVersion()
        {
            int.TryParse(Application.unityVersion.Split('.')[0], out var unityVersionMajor);

            return unityVersionMajor > MINIMUM_EDITOR_VERSION_MAJOR;
        }

        private static void TryShowingMinimumEditorMessageOnceEveryFewHours()
        {
            var lastTimeMessageShown = EditorPrefs.GetString(SwEditorConstants.SwKeys.LAST_TIME_MINIMUM_EDITOR_VERSION_MESSAGE_SHOWN, "0");
            var lastTimeMessageShownLong = long.Parse(lastTimeMessageShown);
            var currentTime = DateTime.Now.Ticks;
            var timeDifference = new TimeSpan(currentTime - lastTimeMessageShownLong);

            if (!(timeDifference.TotalHours > HOURS_BETWEEN_SHOWING_MESSAGE)) return;

            ShowMinimumEditorVersionPopup();
            EditorPrefs.SetString(SwEditorConstants.SwKeys.LAST_TIME_MINIMUM_EDITOR_VERSION_MESSAGE_SHOWN, currentTime.ToString());
        }

        [InitializeOnLoadMethod]
        private static void OnEnable()
        {
            if (IsWisdomSupportingThisUnityVersion() || _hasCompletedSetup) return;
            
            EditorPrefs.DeleteKey(SwEditorConstants.SwKeys.LAST_TIME_MINIMUM_EDITOR_VERSION_MESSAGE_SHOWN);
            EditorApplication.update += TryShowingMinimumEditorMessageOnceEveryFewHours;
            _hasCompletedSetup = true;
        }

        #endregion
    }
}