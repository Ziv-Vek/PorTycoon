using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwEditorTrackerDefaultValues : Dictionary<string, object>
    {
        #region --- Constructor ---

        public SwEditorTrackerDefaultValues()
        {
            Add("os", SwUtils.System.GetOperatingSystemName());
            Add("osVersion", SwUtils.System.GetOperatingSystemVersion());
            Add("sdkVersion", SwConstants.SDK_VERSION);
            Add("sdkVersionId", SwConstants.SdkVersionId);
            Add("unityVersion", Application.unityVersion);
            Add("unityVersionId", SwUtils.System.ComputeUnityVersionId(Application.unityVersion));
            Add("buildTarget", EditorUserBuildSettings.activeBuildTarget.ToString());
            Add("bundle", Application.identifier);
            Add("appVersion", Application.version);
            Add("appleAppId", SwEditorUtils.SwSettings.IosAppId);
        }

        #endregion
    }
}