using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupersonicWisdomSDK.Editor
{
    internal interface ISwExternalAnalyticsService
    {
        Task<bool> TrackEvent(Dictionary<string, object> customEventParams);
    }
}