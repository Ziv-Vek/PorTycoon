using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    /// <summary>
    /// This class is used to bridge between Wisdom's internally collected data and the user.
    /// </summary>
    internal abstract class SwCoreDataBridge
    {
        #region --- Properties ---

        protected Dictionary<string, object> DataDictionary { get; } = new Dictionary<string, object>();
        internal Dictionary<string, object> CustomAndExternalDataDictionary { get; } = new Dictionary<string, object>();

        #endregion
        
        
        #region --- Public Methods ---
        
        internal Dictionary<string, object> GetAllDataAsDictionary()
        {
            var getDataFlags = Enum.GetValues(typeof(ESwGetDataFlag)) as ESwGetDataFlag[];

            if (getDataFlags != null)
            {
                return ConstructGetDataDictionary(getDataFlags, true);
            }

            SwInfra.Logger.LogError(EWisdomLogType.DataBridge, $"{nameof(GetAllDataAsDictionary)} | Could not convert Enum values to ESwGetDataFlag array");
            return null;
        }

        internal Dictionary<string, object> GetDataBasedOnFlagsAsDictionary(params ESwGetDataFlag[] getDataFlags)
        {
            if (getDataFlags != null && !getDataFlags.SwIsEmpty())
            {
                return ConstructGetDataDictionary(getDataFlags, false);
            }

            SwInfra.Logger.LogError(EWisdomLogType.DataBridge, $"{nameof(GetDataBasedOnFlagsAsDictionary)} | getDataFlags cannot be null or empty");
            return null;
        }
        
        internal string GetAllDataAsJsonString()
        {
            var data = GetAllDataAsDictionary();
            return JsonConvert.SerializeObject(data);
        }

        internal string GetDataBasedOnFlagsAsJsonString(params ESwGetDataFlag[] getDataFlags)
        {
            var data = GetDataBasedOnFlagsAsDictionary(getDataFlags);
            return JsonConvert.SerializeObject(data);
        }
        
        public void SetData(string customKey, string value)
        {
            if (customKey.SwIsNullOrEmpty() || value.SwIsNullOrEmpty())
            {
                SwInfra.Logger.LogError(EWisdomLogType.DataBridge, $"{nameof(SetData)} | customKey and value cannot be null or empty");
                return;
            }
            
            SwInfra.KeyValueStore.SetString(customKey, value);
            DataDictionary.SwAddOrReplace(customKey, value);
            CustomAndExternalDataDictionary.SwAddOrReplace(customKey, value);
        }
        
        #endregion


        #region --- Private Methods ---
        
        protected Dictionary<string, object> ConstructGetDataDictionary(IEnumerable<ESwGetDataFlag> getDataFlags, bool shouldReturnAll = false)
        {
            var data = new Dictionary<string, object>();
            
            if (shouldReturnAll)
            {
                data = DataDictionary;
            }
            
            foreach (var flag in getDataFlags)
            {
                AddDataToDictionary(data, flag);
            }
            
            SwInfra.Logger.Log(EWisdomLogType.DataBridge, $"{nameof(ConstructGetDataDictionary)} | Constructed data dictionary: {data.SwToString()}");

            return data;
        }

        protected abstract void AddDataToDictionary(Dictionary<string, object> data, ESwGetDataFlag flag);

        #endregion
    }
}