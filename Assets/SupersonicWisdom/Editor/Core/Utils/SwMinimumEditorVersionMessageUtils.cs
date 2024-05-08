using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwMinimumEditorVersionMessageUtils
    {
        #region --- Constants ---

        private const int LTS_MAJOR_2021 = 2021;
        private const int LTS_MAJOR_2022 = 2022;
        private const int HOURS_BETWEEN_SHOWING_MESSAGE = 24;
        private const string MINIMUM_PATCH_2021 = "2021.3.35";
        private const string MINIMUM_PATCH_2022 = "2022.3.18";

        #endregion


        #region --- Members ---

        private static readonly Version MinimumSupported2021Lts = new Version(LTS_MAJOR_2021, 3, 35);
        private static readonly Version MinimumSupported2022Lts = new Version(LTS_MAJOR_2022, 3, 18);
        private static long _lastTimeMessageShownLong;
        private static Version _currentUnityVersion;

        #endregion


        #region --- Properties ---

        private static bool IsWisdomSupportingThisUnityVersion
        {
            get { return _currentUnityVersion == null || _currentUnityVersion.Major == LTS_MAJOR_2021 && _currentUnityVersion >= MinimumSupported2021Lts || _currentUnityVersion.Major == LTS_MAJOR_2022 && _currentUnityVersion >= MinimumSupported2022Lts || _currentUnityVersion.Major > LTS_MAJOR_2022; }
        }

        #endregion


        #region --- Mono Override ---

        [InitializeOnLoadMethod]
        private static void OnEnable()
        {
            _currentUnityVersion = GetCurrentUnityVersion();

            if (IsWisdomSupportingThisUnityVersion) return;

            var lastTimeMessageShown = EditorPrefs.GetString(SwEditorConstants.SwKeys.LAST_TIME_MINIMUM_EDITOR_VERSION_MESSAGE_SHOWN, "0");
            _lastTimeMessageShownLong = long.Parse(lastTimeMessageShown);

            EditorApplication.update -= TryShowingMinimumEditorMessageOnceEveryFewHours;
            EditorApplication.update += TryShowingMinimumEditorMessageOnceEveryFewHours;
        }

        #endregion


        #region --- Private Methods ---

        private static void ShowMinimumEditorVersionPopup()
        {
            _lastTimeMessageShownLong = DateTime.Now.Ticks;
            
            var updateUrl = string.Empty;
            var message = string.Empty;

            if (_currentUnityVersion.Major < LTS_MAJOR_2021 || _currentUnityVersion.Major == 2022 && _currentUnityVersion < MinimumSupported2022Lts)
            {
                updateUrl = SwEditorConstants.UI.MINIMUM_EDITOR_VERSION_URL.Format(LTS_MAJOR_2022);
                message = SwEditorConstants.UI.UNITY_MINIMUM_VERSION_MESSAGE.Format(MINIMUM_PATCH_2022);
            }

            if (_currentUnityVersion < MinimumSupported2021Lts)
            {
                updateUrl = SwEditorConstants.UI.MINIMUM_EDITOR_VERSION_URL.Format(LTS_MAJOR_2021);
                message = SwEditorConstants.UI.UNITY_MINIMUM_VERSION_MESSAGE.Format(MINIMUM_PATCH_2021);
            }

            if (EditorUtility.DisplayDialog(SwEditorConstants.UI.SUPERSONIC_WISDOM_SDK, message, SwEditorConstants.UI.ButtonTitle.UPDATE, SwEditorConstants.UI.ButtonTitle.OK))
            {
                Application.OpenURL(updateUrl);
            }
        }

        private static void TryShowingMinimumEditorMessageOnceEveryFewHours()
        {
            if (!DidTheTimePassed()) return;

            ShowMinimumEditorVersionPopup();
            
            EditorPrefs.SetString(SwEditorConstants.SwKeys.LAST_TIME_MINIMUM_EDITOR_VERSION_MESSAGE_SHOWN, _lastTimeMessageShownLong.SwToString());
        }

        private static bool DidTheTimePassed()
        {
            var timeDifference = new TimeSpan(DateTime.Now.Ticks - _lastTimeMessageShownLong);

            return timeDifference.TotalHours >= HOURS_BETWEEN_SHOWING_MESSAGE;
        }

        private static Version GetCurrentUnityVersion()
        {
            var unityVersion = Application.unityVersion;
            var versionMatch = Regex.Match(unityVersion, @"^(\d+\.\d+\.\d+)");

            if (!versionMatch.Success) return null;

            var numericVersionString = versionMatch.Value;

            return Version.TryParse(numericVersionString, out var numericVersion) ? numericVersion : null;
        }

        #endregion
    }
}