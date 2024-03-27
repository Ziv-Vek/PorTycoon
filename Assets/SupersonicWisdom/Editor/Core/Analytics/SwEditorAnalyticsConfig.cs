using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    [Serializable]
    public class SwEditorAnalyticsConfig
    {
        #region --- Serialized Members ---

        [JsonProperty(nameof(blacklistedUnityVersions))]
        public List<string> blacklistedUnityVersions;
        [JsonProperty(nameof(blacklistedSdkVersions))]
        public List<string> blacklistedSdkVersions;

        #endregion
        
        
        #region --- Properties ---

        public bool ShouldTrackEditorEvents
        {
            get
            {
                return !IsUnityVersionBlacklisted(Application.unityVersion) && !IsSdkVersionBlacklisted(SwConstants.SDK_VERSION);
            }
        }

        #endregion
    
        
        #region --- Private Methods ---
        
        private bool IsUnityVersionBlacklisted(string version)
        {
#if SUPERSONIC_WISDOM_TEST
            return false;
#endif
            return SwEditorConfigUtils.IsVersionBlacklisted(version, blacklistedUnityVersions);
        }

        private bool IsSdkVersionBlacklisted(string version)
        {
#if SUPERSONIC_WISDOM_TEST
            return false;
#endif
            return SwEditorConfigUtils.IsVersionBlacklisted(version, blacklistedSdkVersions);
        }
        
        #endregion
    }
}