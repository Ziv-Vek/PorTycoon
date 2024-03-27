#if UNITY_ANDROID
using System.Xml;
using UnityEditor.Android;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwDeepLinkAndroidInjector : IPostGenerateGradleAndroidProject
    {
        #region --- Members ---

        private SwAndroidManifest _swAndroidManifest;

        #endregion


        #region --- Properties ---

        public int callbackOrder
        {
            get { return 2; }
        }

        #endregion


        #region --- Public Methods ---

        public void OnPostGenerateGradleAndroidProject(string basePath)
        {
            _swAndroidManifest = new SwAndroidManifest(basePath);

            InjectDeepLink();

            _swAndroidManifest.Save();
        }

        #endregion


        #region --- Private Methods ---

        private void InjectDeepLink ()
        {
            var activityWithLaunchIntent = _swAndroidManifest.GetActivityWithLaunchIntent();
            var deepLinkIntent = _swAndroidManifest.CreateElement("intent-filter");
            var viewAction = _swAndroidManifest.CreateElement("action");
            _swAndroidManifest.SetAndroidAttribute(viewAction, "name", "android.intent.action.VIEW");
            deepLinkIntent.AppendChild(viewAction);
            var defaultCategory = _swAndroidManifest.CreateElement("category");
            _swAndroidManifest.SetAndroidAttribute(defaultCategory, "name", "android.intent.category.DEFAULT");
            deepLinkIntent.AppendChild(defaultCategory);
            var browsableCategory = _swAndroidManifest.CreateElement("category");
            _swAndroidManifest.SetAndroidAttribute(browsableCategory, "name", "android.intent.category.BROWSABLE");
            deepLinkIntent.AppendChild(browsableCategory);
            var data = _swAndroidManifest.CreateElement("data");
            _swAndroidManifest.SetAndroidAttribute(data, "scheme", SwDeepLinkHandler.GetDeepLinkScheme(SwEditorUtils.SwSettings.GetGameId()));
            _swAndroidManifest.SetAndroidAttribute(data, "host", SwDeepLinkConstants.DEEP_LINK_HOST);
            deepLinkIntent.AppendChild(data);
            activityWithLaunchIntent.AppendChild(deepLinkIntent);
            SwEditorTracker.TrackEditorEvent(nameof(InjectDeepLink), ESwEditorWisdomLogType.Android, ESwEventSeverity.Info, "Deep link injected");
        }

        #endregion
    }
}
#endif