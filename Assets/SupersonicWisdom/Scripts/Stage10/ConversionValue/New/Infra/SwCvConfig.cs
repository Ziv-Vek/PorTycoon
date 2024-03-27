#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwCvConfig
    {
        [JsonProperty("model")]
        public string model;
        
        [JsonProperty("postbackZeroScheme")]
        public SwPostbackScheme postbackZeroScheme;
        
        [JsonProperty("postbackOneScheme")]
        public SwPostbackScheme postbackOneScheme;
        
        [JsonProperty("postbackTwoScheme")]
        public SwPostbackScheme postbackTwoScheme;
    }
    
    [Serializable]
    internal class SwPostbackScheme
    {
        [JsonProperty("cv")]
        public float[] cv;
        
        [JsonProperty("coarse")]
        public float[] coarse;
        
        [JsonProperty("lockWindow")]
        public LockWindow lockWindow;
    }
    
    [Serializable]
    internal class LockWindow
    {
        [JsonProperty("lockFromTime")]
        public float lockFromTime = BaseCvUpdater.LOCK_FROM_TIME_DEFAULT_VALUE;
        
        [JsonProperty("lockFromCoarse")]
        public int lockFromCoarse = BaseCvUpdater.LOCK_FROM_TIME_DEFAULT_VALUE;
    }
}
#endif
