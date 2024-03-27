#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    [Serializable]
    public class SwIntegrationToolSuiteRawData
    {
        [JsonProperty("id")] public string id;
        [JsonProperty("title")] public string title;
        [JsonProperty("description")] public string description;
        [JsonProperty("testCases")] public SwIntegrationToolTestCasesRawData[] testCases;
    }
}
#endif