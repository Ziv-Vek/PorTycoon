#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwStage10DevTools : SwDevTools, ISwDeepLinkListener
    {
        #region --- Members ---

        protected internal SwStage10IntegrationManager SwStage10IntegrationManager;
        private Dictionary<string, string> _deepLinkParams;

        #endregion


        #region --- Construction ---

        internal SwStage10DevTools(SwFilesCacheManager filesCacheManager) : base(filesCacheManager)
        {
        }

        #endregion


        #region --- Private Methods ---

        public IEnumerator OnDeepLinkResolved(Dictionary<string, string> deepLinkParams)
        {
            SwInfra.Logger.Log(EWisdomLogType.IntegrationTool, "on resolved");
            _deepLinkParams = deepLinkParams;
            
            if (SwStage10IntegrationManager != null)
            {
                SwInfra.Logger.Log(EWisdomLogType.IntegrationTool, "yes manager");
                yield return SwStage10IntegrationManager?.Init(deepLinkParams);
            }
            else
            {
                SwInfra.Logger.Log(EWisdomLogType.IntegrationTool, "no manager");
            }
        }

        protected override IEnumerator SetUpDevToolsCanvas()
        {
            yield return base.SetUpDevToolsCanvas();
            yield return CreateIntegrationManager();
        }

        protected virtual IEnumerator CreateIntegrationManager()
        {
            SwStage10IntegrationManager = new SwStage10IntegrationManager(DevToolsCanvas);

            if (_deepLinkParams != null)
            {
                yield return SwStage10IntegrationManager.Init(_deepLinkParams);
            }
        }

        #endregion
    }
}
#endif