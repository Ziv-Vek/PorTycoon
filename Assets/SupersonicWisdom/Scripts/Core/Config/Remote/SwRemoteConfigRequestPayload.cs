using System;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwRemoteConfigRequestPayload
    {
        [JsonProperty("bundle")]
        public string bundle;
        [JsonProperty("gameId")]
        public string gameId;
        [JsonProperty("sysLang")]
        public string sysLang;
        [JsonProperty("os")]
        public string os;
        [JsonProperty("osver")]
        public string osver;
        [JsonProperty("version")]
        public string version;
        [JsonProperty("device")]
        public string device;
        [JsonProperty("session")]
        public string session;
        [JsonProperty("uuid")]
        public string uuid;
        [JsonProperty("abid")]
        public string abid;
        [JsonProperty("isNew")]
        public string isNew;
        [JsonProperty("apiVersion")]
        public string apiVersion;
        [JsonProperty("sdkVersion")]
        public string sdkVersion;
        [JsonProperty("sdkVersionId")]
        public long sdkVersionId;
        [JsonProperty("stage")]
        public long stage;
        [JsonProperty("abTestVariant", NullValueHandling = NullValueHandling.Ignore)]
        public string abTestVariant;
        [JsonProperty("installSdkVersion", NullValueHandling = NullValueHandling.Ignore)]
        public string installSdkVersion;
        [JsonProperty("installSdkVersionId", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long installSdkVersionId;

#if UNITY_IOS
        public string idfv;
#endif
#if UNITY_ANDROID
        public string appSetId;
#endif
    }
}