#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class PostbackWindow
    {
        [JsonProperty] public EPostbackWindow type;
        [JsonProperty] public DateTime startTime;
        [JsonProperty] public DateTime endTime;
        [JsonProperty] public bool isLocked;
    }
}
#endif