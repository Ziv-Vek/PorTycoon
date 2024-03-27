using System;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwAgentConfig
    {
        #region --- Members ---

        [JsonProperty("country")]
        public string country;

        #endregion
    }
}