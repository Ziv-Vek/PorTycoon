#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections;

namespace SupersonicWisdomSDK
{
    internal class SwStage10InitThirdParties : ISwAsyncRunnable
    {
        #region --- Members ---

        private readonly SwStage10FacebookAdapter _facebookAdapter;
        private readonly SwStage10GameAnalyticsAdapter _gameAnalyticsAdapter;

        #endregion


        #region --- Construction ---

        public SwStage10InitThirdParties(SwStage10FacebookAdapter facebookAdapter, SwStage10GameAnalyticsAdapter gameAnalyticsAdapter)
        {
            _facebookAdapter = facebookAdapter;
            _gameAnalyticsAdapter = gameAnalyticsAdapter;
        }

        #endregion


        #region --- Public Methods ---

        public IEnumerator Run ()
        {
            _facebookAdapter.Init();
            _gameAnalyticsAdapter.Init();

            yield break;
        }

        #endregion
    }
}
#endif