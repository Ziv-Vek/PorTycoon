#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SupersonicWisdomSDK
{
    [Serializable]
    public class SwIntegrationToolTestCasesRawData
    {
        [JsonProperty("idWithParams")] 
        public string idWithParams;
        
        [JsonProperty("id")] 
        public string id;
        
        [JsonProperty("title")]
        public string title;
        
        [JsonProperty("description")] 
        public string description;
        
        [JsonProperty("type"), JsonConverter(typeof(StringEnumConverter))]
        public ETestCaseType type;
        
        [JsonProperty("isRegExp")]
        public bool isRegExp;
        
        [JsonProperty("spec")] 
        public SwIntegrationToolSpecRawData spec;
    }
}
#endif