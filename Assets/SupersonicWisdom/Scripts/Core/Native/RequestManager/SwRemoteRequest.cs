using System;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal struct SwRemoteRequest
    {
        #region --- Members ---

        public int connectionTimeout;
        public int readTimeout;
        public int cap;
        public string key;
        public string url;
        public string body;
        public string headers;

        #endregion
    }
}