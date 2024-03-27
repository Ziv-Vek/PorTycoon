#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    [Serializable]
    public class SwIntegrationToolSpecRawData
    {
        [JsonProperty("start")] public string[] start;
        [JsonProperty("expected")] public string[] expected;
        [JsonProperty("forbidden")] public string[] forbidden;
        [JsonProperty("end")] public string[] end;
        [JsonProperty("logTrigger")] public string logTrigger;
        [JsonProperty("message")] public string message;
    }
}
#endif