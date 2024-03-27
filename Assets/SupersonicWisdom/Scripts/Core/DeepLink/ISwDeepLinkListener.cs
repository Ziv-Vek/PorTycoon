using System.Collections;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal interface ISwDeepLinkListener
    {
        IEnumerator OnDeepLinkResolved(Dictionary<string, string> deepLinkParams);
    }
}