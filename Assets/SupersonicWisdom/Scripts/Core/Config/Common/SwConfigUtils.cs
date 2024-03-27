using System.Collections.Generic;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    internal static class SwConfigUtils
    {
        #region --- Public Methods ---

        public static Dictionary<string, object> AsDictionary(this Dictionary<string, object> self)
        {
            return new Dictionary<string, object>(self);
        }

        public static int GetValue(this Dictionary<string, object> self, string key, int defaultVal)
        {
            var val = self.GetValue(key);

            if (val is string)
            {
                var ret = 0;

                if (int.TryParse((string)val, out ret))
                {
                    return ret;
                }
            }
            else if (val is int)
            {
                return (int)val;
            }
            else if (val is long)
            {
                var ret = 0;

                if (int.TryParse(val.ToString(), out ret))
                {
                    return ret;
                }
            }

            return defaultVal;
        }

        public static bool GetValue(this Dictionary<string, object> self, string key, bool bDefaultVal)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            var val = self.GetValue(key);

            if (val is string)
            {
                var ret = false;

                if (bool.TryParse((string)val, out ret))
                {
                    return ret;
                }
            }
            else if (val is bool)
            {
                return (bool)val;
            }

            return bDefaultVal;
        }

        public static float GetValue(this Dictionary<string, object> self, string key, float defaultVal)
        {
            var val = self.GetValue(key);

            if (val is string)
            {
                float ret = 0;

                if (float.TryParse((string)val, out ret))
                {
                    return ret;
                }
            }
            else if (val is float f)
            {
                return f;
            }
            else if (val is double)
            {
                float ret = 0;

                if (float.TryParse(val.ToString(), out ret))
                {
                    return ret;
                }
            }

            return defaultVal;
        }

        public static string GetValue(this Dictionary<string, object> self, string key, string defaultVal)
        {
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            var val = self.GetValue(key);

            if (val is string valueString)
            {
                return valueString;
            }

            return defaultVal;
        }

        public static bool HasConfigKey(this Dictionary<string, object> self, string key)
        {
            var hasKey = self.ContainsKey(key);
            SwInfra.Logger.Log(EWisdomLogType.Config, $"key={key} | {hasKey}");

            return hasKey;
        }

        #endregion


        #region --- Private Methods ---

        internal static Dictionary<string, object> ResolveAbConfig([NotNull] SwAbConfig ab)
        {
            var configFromAb = new Dictionary<string, object>();

            if (ab.IsValid && !ab.IsExpired)
            {
                if (ab.key.Equals(SwConfigConstants.SwDictionaryKey))
                {
                    configFromAb.SwMerge(true, ab.value.SwToJsonDictionary());
                    SwInfra.Logger.Log(EWisdomLogType.Config, $"Init | Got partial config via AB: {ab.value}");
                }
                else
                {
                    configFromAb[ab.key] = ab.value;
                }
            }

            return configFromAb;
        }

        internal static Dictionary<string, object> ResolveDeepLinkConfig([NotNull] Dictionary<string, string> deepLinkParams)
        {
            var configFromDeepLink = new Dictionary<string, object>();
            var partialConfigDeepLinkRaw = deepLinkParams.SwSafelyGet(SwConfigConstants.SwDictionaryKey, null);
            
            if (string.IsNullOrEmpty(partialConfigDeepLinkRaw)) return configFromDeepLink;
            
            configFromDeepLink.SwMerge(true, SwJsonParser.DeserializeToDictionary(partialConfigDeepLinkRaw));
            SwInfra.Logger.Log(EWisdomLogType.Config, $"Init | Got partial config via Deep Link: {partialConfigDeepLinkRaw}");

            return configFromDeepLink;
        }

        /// <summary>
        ///     Get value per key
        ///     It favors data in the AB config over the accessible config
        /// </summary>
        /// <param name="key">key for value</param>
        /// <returns></returns>
        private static object GetValue(this Dictionary<string, object> self, string key)
        {
            var value = self.SwSafelyGet(key, null);
            
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            SwInfra.Logger.Log(EWisdomLogType.Config, $"key={key} | {value}");

            return value;
        }

        #endregion
    }
}