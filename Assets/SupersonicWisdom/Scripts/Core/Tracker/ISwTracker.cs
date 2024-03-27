using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal interface ISwTracker
    {
        void TrackEventWithParams(string eventName, Dictionary<string, object> customs = null, ClientCategory? clientCategory = null);
        void TrackGameProgressEvent(Dictionary<string, object> customs = null);
    }
}