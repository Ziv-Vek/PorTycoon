using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class SwCoreConfig : ISwCoreInternalConfig
    {
        #region --- Constants ---

        private const string CONFIG_KEY = "config";

        #endregion
        

        #region --- Properties ---
        
        /// <summary>
        ///     The dictionary under "config" key.
        ///     This cannot be simply deserialized since it's a dynamic dictionary
        /// </summary>
        [JsonProperty("config")]
        [JsonConverter(typeof(DictionaryConverter<object>))]
        [NotNull]
        public Dictionary<string, object> DynamicConfig { get; set; }

        [JsonProperty("ab")]
        public SwAbConfig Ab { get; set; }

        #endregion


        #region --- Construction ---

        public SwCoreConfig(Dictionary<string, object> defaultDynamicConfig)
        {
            DynamicConfig = defaultDynamicConfig;
        }

        #endregion


        #region --- Public Methods ---
        
        public int GetValue(int defaultVal, params string[] keys)
        {
            if (keys == null || keys.Length == 0) return defaultVal;
            
            foreach (var key in keys)
            {
                if (DynamicConfig.ContainsKey(key))
                {
                    return DynamicConfig.GetValue(key, defaultVal);
                }
            }

            return defaultVal;
        }

        public int GetValue(string key, int defaultVal)
        {
            return DynamicConfig.GetValue(key, defaultVal);
        }

        public float GetValue(string key, float defaultVal)
        {
            return DynamicConfig.GetValue(key, defaultVal);
        }

        public bool GetValue(string key, bool defaultVal)
        {
            return DynamicConfig.GetValue(key, defaultVal);
        }
        
        public bool GetValue(bool defaultVal, params string[] keys)
        {
            if (keys == null || keys.Length == 0) return defaultVal;
            
            foreach (var key in keys)
            {
                if (DynamicConfig.ContainsKey(key))
                {
                    return DynamicConfig.GetValue(key, defaultVal);
                }
            }

            return defaultVal;
        }

        public string GetValue(string key, string defaultVal)
        {
            return DynamicConfig.GetValue(key, defaultVal);
        }

        public Dictionary<string, object> AsDictionary()
        {
            return DynamicConfig.AsDictionary();
        }

        public bool HasConfigKey(string key)
        {
            return DynamicConfig.HasConfigKey(key);
        }

        #endregion
    }
}