namespace SupersonicWisdomSDK
{
    internal static class SwTrackerConstants
    {
        #region --- Inner Enumerations ---

        private enum EMinimumSdkVersion
        {
            // ReSharper disable once InconsistentNaming
            Sdk_7_7_18 = 7071899,
        }
        
        #endregion
        
        
        #region --- Public Methods ---
        
        public static bool ShouldTrackPastMinimumSdkVersion(SwCoreUserData coreUserData)
        {
            // In use due to - https://supersonicstudio.monday.com/boards/883112163/pulses/6277235145
            return SwUtils.System.IsSdkDevelopmentBuild 
                   || coreUserData.InstallSdkVersionId >= (long)EMinimumSdkVersion.Sdk_7_7_18;
        }
        
        #endregion
    }
}