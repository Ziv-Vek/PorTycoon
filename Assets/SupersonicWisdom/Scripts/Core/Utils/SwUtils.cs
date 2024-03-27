namespace SupersonicWisdomSDK
{
    internal static class SwUtils
    {
        #region --- Properties ---

        internal static SwLangAndCountryUtils LangAndCountry { get; } = new SwLangAndCountryUtils();
        internal static SwDateAndTimeUtils DateAndTime { get; } = new SwDateAndTimeUtils();
        internal static SwSystemUtils System { get; } = new SwSystemUtils();
        internal static SwCoreFileUtils File { get; } = new SwCoreFileUtils();
        internal static SwParsingUtils Parsing { get; } = new SwParsingUtils();
        internal static SwNativeUtils Native { get; } = new SwNativeUtils();
        internal static SwUiUtils Ui { get; } = new SwUiUtils();
        internal static SwReflectionUtils Reflection { get; } = new SwReflectionUtils();
        

        #endregion
    }
}