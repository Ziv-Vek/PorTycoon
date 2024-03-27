using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace SupersonicWisdomSDK.Editor
{
    public class SwAndroidPreBuildValidator : IPreprocessBuildWithReport
    {
    #region --- Constants ---

        private const int MIN_ANDROID_TARGET_API_VERSION = 33;

    #endregion


    #region --- Public Fields ---

        public int callbackOrder { get { return 0; } }

    #endregion


    #region --- Public Methods ---

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform != BuildTarget.Android)
            {
                return;
            }

            CheckAndroidTargetApiVersion();
        }

    #endregion


    #region --- Private Methods ---

        private void CheckAndroidTargetApiVersion()
        {
            var androidTargetVersion = (int)PlayerSettings.Android.targetSdkVersion;

            if (androidTargetVersion >= MIN_ANDROID_TARGET_API_VERSION)
            {
                return;
            }
            
            if (EditorUtility.DisplayDialog("Target API Version Error", GetAndroidTargetApiWarningMessage(androidTargetVersion), SwEditorConstants.UI.ButtonTitle.OK, SwEditorConstants.UI.ButtonTitle.CLOSE))
            {
                EditorApplication.ExecuteMenuItem("Edit/Project Settings...");
            }

            SwEditorTracker.TrackEditorEvent(nameof(SwAndroidPreBuildValidator), ESwEditorWisdomLogType.Android, ESwEventSeverity.Error, GetAndroidTargetApiBuildErrorMessage(androidTargetVersion));
            SwEditorUtils.FailBuildWithMessage(nameof(SwAndroidPreBuildValidator), GetAndroidTargetApiBuildErrorMessage(androidTargetVersion));
        }

        private string GetAndroidTargetApiWarningMessage(int androidTargetVersion)
        {
            //In case the user has set the Android target API version to Automatic the settings return 0 as the API version, we cannot confirm the version until build time, so we show a different message before starting the build.
            return androidTargetVersion == 0 ? $"The Android target API version is set to Automatic, as we cannot confirm the version until build time, please manually set it to {MIN_ANDROID_TARGET_API_VERSION} or higher in the Player Settings." : $"The Android target API version is set to {androidTargetVersion}, please set it to {MIN_ANDROID_TARGET_API_VERSION} or higher in the Player Settings.";
        }

        private string GetAndroidTargetApiBuildErrorMessage(int androidTargetVersion)
        {
            return androidTargetVersion == 0 ? "Build failed because the Android target API version was set to Automatic" : "Build failed because the Android target API version is too low.";
        }

    #endregion
    }
}