namespace SupersonicWisdomSDK
{
    internal enum SwUserStateChangeReason
    {
        None = -1,
        GameStart,
        SessionStart,
        UpdateConversionValue,
        UpdateImpConversionValue,
        LevelCompleted,
        LevelSkipped,
        LevelRevived,
        LevelFailed,
        RvImpression,
        Revenue,
        InterstitialImpression,
        PayingUser,
        NoAdsStateChange,
    }
}