#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections;

namespace SupersonicWisdomSDK
{
    internal class SwStage10InitAppsflyer : ISwAsyncRunnable
    {
        #region --- Members ---

        private readonly SwStage10AppsFlyerAdapter _appsFlyerAdapter;

        #endregion


        #region --- Construction ---

        public SwStage10InitAppsflyer(SwStage10AppsFlyerAdapter appsFlyerAdapter)
        {
            _appsFlyerAdapter = appsFlyerAdapter;
        }

        #endregion


        #region --- Public Methods ---

        public IEnumerator Run ()
        {
            _appsFlyerAdapter.Init();

            yield break;
        }

        #endregion
    }
}
#endif