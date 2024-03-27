#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwStage10Config : SwCoreConfig, ISwStage10InternalConfig
    {
        #region --- Members ---

        [JsonProperty("unavailable")]
        public bool Unavailable { get; set; }
        
        [JsonProperty("agent")]
        public SwAgentConfig Agent { get; set; }
        
        [JsonProperty("events")]
        public SwNativeEventsConfig Events { get; set; }
        
        [JsonProperty("cv")]
        public SwCvConfig Cv { get; set; }

        #endregion
        

        #region --- Construction ---

        public SwStage10Config(Dictionary<string, object> defaultDynamicConfig) : base(defaultDynamicConfig)
        {
            Events = new SwNativeEventsConfig();
        }

        #endregion
    }
}
#endif