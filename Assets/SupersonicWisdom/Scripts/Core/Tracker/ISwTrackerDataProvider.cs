using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal interface ISwTrackerDataProvider
    {
        // Since this function called from SW native plugin - this code will run on BACKGROUND thread =>
        // The class that implements must not call Unity Api since they are available only on MAIN thread
        (SwJsonDictionary dataDictionary, IEnumerable<string> keysToEncrypt) ConditionallyAddExtraDataToTrackEvent(SwCoreUserData coreUserData);
    }
}