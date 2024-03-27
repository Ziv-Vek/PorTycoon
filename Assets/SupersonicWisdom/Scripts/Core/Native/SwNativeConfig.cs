namespace SupersonicWisdomSDK
{
    /// <summary>
    ///     Order of fields is crucial
    ///     Should be same as SwWisdomConfigurationDto.h in SupersonicWisdomIOS
    /// </summary>
    internal struct SwNativeConfig
    {
        #region --- Members ---

        public bool IsLoggingEnabled;
        public string Subdomain;
        public int ConnectTimeout;
        public int ReadTimeout;
        public int InitialSyncInterval;
        public string StreamingAssetsFolderPath;
        public string BlockingLoaderResourceRelativePath;
        public int BlockingLoaderViewportPercentage;

        #endregion
    }
}