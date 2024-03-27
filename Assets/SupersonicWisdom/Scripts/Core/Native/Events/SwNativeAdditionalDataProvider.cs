using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    internal class SwNativeAdditionalDataProvider
    {
        #region --- Fields ---
        
        private readonly ISwNativeApi _wisdomNativeApi;
        private List<ISwTrackerDataProvider>  _trackerDataProviders;
        
        #endregion
        
        
        #region --- Construction ---
        
        internal SwNativeAdditionalDataProvider(ISwNativeApi wisdomNativeApi)
        {
            _wisdomNativeApi = wisdomNativeApi;
        }
        
        #endregion
        
        
        #region --- Public Methods ---

        public void SetTrackerDataProviders(List<ISwTrackerDataProvider> trackerDataProviders)
        {
            _trackerDataProviders = trackerDataProviders;
        }

        public string GetAdditionalDataJson()
        {
            var extraData = new SwJsonDictionary();

            if (_trackerDataProviders == null)
            {
                return string.Empty;
            }
            
            foreach (var provider in _trackerDataProviders)
            {
                try
                {
                    var (providerData, keysToEncrypt) = provider.AddExtraDataToTrackEvent();
                    
                    foreach (var key in keysToEncrypt)
                    {
                        if (providerData.TryGetValue(key, out var value))
                        {
                            providerData[key] = EncryptData(value.ToString());
                        }
                    }
                    
                    extraData.SwMerge(true, providerData);
                }
                catch (Exception e)
                {
                    SwInfra.Logger.Log(EWisdomLogType.Analytics,
                        $"An error occured while trying to merge {provider} \n message: {e.Message}");
                }
            }

            return JsonConvert.SerializeObject(extraData);
        }

        private static string EncryptData(string payload)
        {
            return SwCoreTracker.EncryptCustom(payload);
        }

        #endregion
    }
}