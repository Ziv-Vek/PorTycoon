#if SW_STAGE_STAGE10_OR_ABOVE
using UnityEditor;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwStage10Postprocessor : AssetPostprocessor
    {
        #region --- Public Methods ---

        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            // isHumanControllingUs is mandatory since GA settings property is using Resources.Load function
            // which is not available on CI builds and cause GA Settings to be override
            if (UnityEditorInternal.InternalEditorUtility.isHumanControllingUs &&
                GameAnalyticsSDK.GameAnalytics.SettingsGA != null && SwEditorUtils.SwSettings != null && SwEditorUtils.SwSettings.wasLoggedIn)
            {
                SwGameAnalyticsUtils.VerifyMandatoryFlags();
            }
        }

        #endregion
    }
}
#endif