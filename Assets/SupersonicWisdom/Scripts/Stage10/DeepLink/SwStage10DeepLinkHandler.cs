#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections;

namespace SupersonicWisdomSDK
{
    internal class SwStage10DeepLinkHandler : SwDeepLinkHandler
    {
        #region --- Constants ---

        private const string IN_GAME_CONSOLE_DEEP_LINK_PARAM_NAME = "inGameConsole";

        #endregion


        #region --- Members ---

        private readonly SwSettings _settings;

        #endregion


        #region --- Construction ---

        public SwStage10DeepLinkHandler(SwSettings settings, ISwWebRequestClient webRequestClient) : base(settings, webRequestClient)
        {
            _settings = settings;
        }

        #endregion


        #region --- Private Methods ---

        protected override IEnumerator OnDeepLinkParamsResolve()
        {
            yield return base.OnDeepLinkParamsResolve();
            SwInfra.Logger.LogViaNetwork = _settings.logViaNetwork;
            HandleDeepLinkInGameConsole();
        }

        private void HandleDeepLinkInGameConsole()
        {
            if (DeepLinkParams.TryGetValue(IN_GAME_CONSOLE_DEEP_LINK_PARAM_NAME, out var debugLevel))
            {
                SwInfra.Logger.Log(EWisdomLogType.DeepLink, $"In Game Console enabled with debug level:{debugLevel}");
                SwGameConsole.InitConsole(debugLevel);
            }
        }

        #endregion
    }
}
#endif