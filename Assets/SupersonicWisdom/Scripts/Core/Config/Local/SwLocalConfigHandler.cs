using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SupersonicWisdomSDK
{
    internal class SwLocalConfigHandler
    {
        #region --- Members ---

        public readonly Dictionary<string, object> ConfigValues;

        public SwLocalConfigHandler(ISwLocalConfigProvider[] localConfigProviders)
        {
            ConfigValues = ResolveLocalConfigFromProviders(localConfigProviders);
        }

        #endregion


        #region --- Private Methods ---

        private Dictionary<string, object> ResolveLocalConfigFromProviders(ISwLocalConfigProvider[] configProviders)
        {
            var arrayLength = configProviders.Length;

            if (arrayLength == 0) return new Dictionary<string, object>();

            var localConfigValues = new Dictionary<string, object>[arrayLength];

            for (var i = 0; i < arrayLength; i++)
            {
                var config = configProviders[i].GetLocalConfig();
                localConfigValues[i] = config.LocalConfigValues;
            }

            var configToMerge = new Dictionary<string, object>().SwMerge(true, localConfigValues);
            SwInfra.Logger.Log(EWisdomLogType.Config, "SwLocalConfigHandler | ResolveLocalConfigFromSetters | " + $"Resolved {configToMerge.Count} pairs");

            return configToMerge;
        }

        #endregion
    }
}