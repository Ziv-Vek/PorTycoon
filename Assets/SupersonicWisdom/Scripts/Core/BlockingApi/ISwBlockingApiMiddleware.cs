using System.Collections;

namespace SupersonicWisdomSDK
{
    internal interface ISwBlockingApiMiddleware
    {
        #region --- Public Methods ---

        IEnumerator PrepareForGameStarted();
        IEnumerator ProcessTimeBasedGameStarted();
        IEnumerator ProcessLevelCompleted();
        IEnumerator ProcessLevelFailed();
        IEnumerator ProcessLevelRevived();
        IEnumerator ProcessLevelSkipped();
        IEnumerator ProcessLevelStarted();
        IEnumerator ProcessRewardedVideoOpportunityMissed();
        IEnumerator ProcessMetaStarted();
        IEnumerator ProcessMetaEnded();

        #endregion
    }
}