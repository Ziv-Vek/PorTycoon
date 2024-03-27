#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    public class SwIntegrationToolManualTestCasesManager : SwBaseIntegrationToolTestCasesManager<SwIntegrationToolManualBaseTestCase>
    {
        #region --- Members ---

        private SwIntegrationToolSearchInHierarchyTestCase<AppsFlyerObjectScript> _searchAppsFlyer;

        #endregion


        #region --- Properties ---

        protected override bool IsValid
        {
            get { return true; }
        }

        #endregion


        #region --- Construction ---

        public SwIntegrationToolManualTestCasesManager(Action<Dictionary<string, ISwIntegrationToolTestCaseViewData>> setViewData, Action<ISwIntegrationToolTestCase> onIntegrationToolTestCaseManagerStatusChanged) : base(setViewData, onIntegrationToolTestCaseManagerStatusChanged) { }

        #endregion


        #region --- Private Methods ---

        protected override void SplitGenericTestCasesToDictionaries()
        {
            _searchAppsFlyer = new SwIntegrationToolSearchInHierarchyTestCase<AppsFlyerObjectScript>();
            OnTestCaseCreated(_searchAppsFlyer);
        }

        #endregion
    }
}
#endif