using System;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK.Editor
{
    [Serializable]
    internal class SwEditorConfigData
    {
        #region --- Serialized Members ---

        [JsonProperty(nameof(config))]
        public SwEditorConfig config;
        
        #endregion
    }

    [Serializable]
    internal class SwEditorConfig
    {
        #region --- Serialized Members ---

        [JsonProperty(nameof(analyticsConfig))]
        public SwEditorAnalyticsConfig analyticsConfig;

        [JsonProperty(nameof(selfUpdateConfig))]
        public SwSelfUpdateRawConfiguration selfUpdateConfig;

        #endregion
    }
}