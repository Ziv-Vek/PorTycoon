#if SW_STAGE_STAGE10_OR_ABOVE
using UnityEditor.Android;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwFacebookAdvertiserIdCollectionAndroidInjector : IPostGenerateGradleAndroidProject
    {
        #region --- Properties ---

        public int callbackOrder
        {
            get { return 1; }
        }

        #endregion


        #region --- Public Methods ---

        public void OnPostGenerateGradleAndroidProject(string basePath)
        {
            var swAndroidManifest = new SwAndroidManifest(basePath);

            if (SwStageUtils.CurrentStageNumber == SwEditorConstants.FACEBOOK_ADVERTISER_ID_COLLECTION_STAGE_NUMBER_REQUIREMENT)
            {
                swAndroidManifest.SetMetadataElement("com.facebook.sdk.AdvertiserIDCollectionEnabled", "true");
            }

            swAndroidManifest.Save();
            SwEditorTracker.TrackEditorEvent(nameof(OnPostGenerateGradleAndroidProject), ESwEditorWisdomLogType.Android, ESwEventSeverity.Info, "Facebook Advertiser ID collection injected.");
        }

        #endregion
    }
}

#endif