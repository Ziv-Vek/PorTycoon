using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwNativeEventsConfig
    {
        #region --- Constants ---

        private const bool DefaultEnabled = true;
        private const int DefaultConnectTimeout = 15;
        private const int DefaultInitialSyncInterval = 8;
        private const int DefaultReadTimeout = 10;

        #endregion


        #region --- Members ---

        [JsonProperty("enabled", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(DefaultEnabled)]
        public bool enabled;
        
        [JsonProperty("connectTimeout", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(DefaultConnectTimeout)]
        public int connectTimeout;
        
        [JsonProperty("initialSyncInterval", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(DefaultInitialSyncInterval)]
        public int initialSyncInterval;
        
        [JsonProperty("readTimeout", DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(DefaultReadTimeout)]
        public int readTimeout;

        #endregion


        #region --- Construction ---

        public SwNativeEventsConfig()
        {
            enabled = DefaultEnabled;
            connectTimeout = DefaultConnectTimeout;
            initialSyncInterval = DefaultInitialSyncInterval;
            readTimeout = DefaultReadTimeout;
        }

        #endregion
    }
}