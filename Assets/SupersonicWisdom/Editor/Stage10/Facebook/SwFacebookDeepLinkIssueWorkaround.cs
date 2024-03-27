#if SW_STAGE_STAGE10_OR_ABOVE
#if UNITY_IOS
using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;

namespace SupersonicWisdomSDK.Editor
{
    /// <summary>
    ///     Fixing FB deep link issue on iOS.
    ///     FB issue waiting to be fixed: https://github.com/facebook/facebook-sdk-for-unity/issues/712
    ///     Unity documentations:
    ///     https://issuetracker.unity3d.com/issues/ios-crash-when-opening-app-using-a-deep-link-while-facebook-sdk-is-installed
    /// </summary>
    internal static class SwFacebookDeepLinkIssueWorkaround
    {
        #region --- Constants ---

        private const string DID_FINISH_LAUNCHING_WITH_OPTIONS = @"(?x)                                  # Verbose mode
  (\(BOOL\)isBackgroundLaunchOptions.+      # Find this function...
    (?:.*\n)+?                          # Match as few lines as possible until...
    \s*return\ )YES(\;\n                 #   return YES;
  \})                                   # }";

        #endregion


        #region --- Public Methods ---

        [PostProcessBuild(201)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
#if ((UNITY_2021_3_OR_NEWER && !UNITY_2022_3_OR_NEWER && !UNITY_2021_3_1 && !UNITY_2021_3_2 && !UNITY_2021_3_3 && !UNITY_2021_3_4 && !UNITY_2021_3_5 && !UNITY_2021_3_6 && !UNITY_2021_3_7 && !UNITY_2021_3_8 && !UNITY_2021_3_9 && !UNITY_2021_3_10 && !UNITY_2021_3_11 && !UNITY_2021_3_12 && !UNITY_2021_3_13 && !UNITY_2021_3_14 && !UNITY_2021_3_15 && !UNITY_2021_3_16 && !UNITY_2021_3_17 && !UNITY_2021_3_18 && !UNITY_2021_3_19 && !UNITY_2021_3_20 && !UNITY_2021_3_21 && !UNITY_2021_3_22 && !UNITY_2021_3_23 && !UNITY_2021_3_24 && !UNITY_2021_3_25 && !UNITY_2021_3_26 && !UNITY_2021_3_27 && !UNITY_2021_3_28 && !UNITY_2021_3_29 && !UNITY_2021_3_30) || (UNITY_2022_3_OR_NEWER && !UNITY_2022_3_1 && !UNITY_2022_3_2 && !UNITY_2022_3_3 && !UNITY_2022_3_4 && !UNITY_2022_3_5 && !UNITY_2022_3_6 && !UNITY_2022_3_7 && !UNITY_2022_3_8 && !UNITY_2022_3_9 && !UNITY_2022_3_10))
            if (buildTarget == BuildTarget.iOS)
            {
                FixColdStart(path);
            }
#endif
        }

        #endregion


        #region --- Private Methods ---

        private static void FixColdStart(string path)
        {
            try
            {
                var fullPath = Path.Combine(path, Path.Combine("Classes", "UnityAppController.mm"));
                var data = File.ReadAllText(fullPath);
                data = Regex.Replace(data, DID_FINISH_LAUNCHING_WITH_OPTIONS, "$1NO$2");
                File.WriteAllText(fullPath, data);
            }
            catch (Exception e)
            {
                SwEditorLogger.Log(e.Message);
            }
        }

        #endregion
    }
}
#endif
#endif