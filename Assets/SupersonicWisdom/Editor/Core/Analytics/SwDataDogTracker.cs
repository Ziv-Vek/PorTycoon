using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwDataDogTracker : ISwExternalAnalyticsService
    {
        #region --- Constants ---
        
        private const string URL = "https://api.datadoghq.com/api/v1/events";
        private const string API_KEY = "a6beda4bfd82ff4073407113b11d4b37";
        private const string CUSTOM_HEADER_KEY = "DD-API-KEY";
        
        #endregion
        
        
        #region --- Public Methods ---
        
        public async Task<bool> TrackEvent(Dictionary<string, object> customEventParams)
        {
            var headers = new Dictionary<string, string>
            {
                { CUSTOM_HEADER_KEY, API_KEY },
            };

            var result = await SwNetworkHelper.PerformRequest(URL, customEventParams, headers);

            if (result.Item2.ErrorMessage == null)
            {
                return true;
            }

            SwEditorLogger.LogError($"{nameof(SwDataDogTracker)}: Failed to track event: {result.Item2.ErrorMessage} ({(int)result.Item3.StatusCode})");
            return false;
        } 
        
        #endregion
    }
}