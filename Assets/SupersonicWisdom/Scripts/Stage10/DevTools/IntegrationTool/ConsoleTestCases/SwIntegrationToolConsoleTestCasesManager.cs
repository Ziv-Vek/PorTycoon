#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    public class SwIntegrationToolConsoleTestCasesManager : SwBaseIntegrationToolTestCasesManager<SwIntegrationToolConsoleBaseTestCase>
    {
        #region --- Members ---

        private readonly Action<string, float, Action, Action> _showToast;
        private readonly SwIntegrationToolTestCasesRawData[] _testCasesRawData;

        #endregion


        #region --- Properties ---

        protected override bool IsValid
        {
            get { return _testCasesRawData != null; }
        }

        #endregion


        #region --- Construction ---

        public SwIntegrationToolConsoleTestCasesManager(SwIntegrationToolTestCasesRawData[] testCasesRawData, Action<Dictionary<string, ISwIntegrationToolTestCaseViewData>> setViewData, Action<string, float, Action, Action> showToast, Action<ISwIntegrationToolTestCase> onSetViewData) : base(setViewData, onSetViewData)
        {
            _testCasesRawData = testCasesRawData;
            _showToast = showToast;
        }

        #endregion


        #region --- Private Methods ---

        protected override void SplitGenericTestCasesToDictionaries()
        {
            if (_testCasesRawData == null) return;

            foreach (var testCaseData in _testCasesRawData)
            {
                SwIntegrationToolConsoleBaseTestCase currentBaseTestCase;

                switch (testCaseData.type)
                {
                    case ETestCaseType.scheduled_log_assertion:
                        currentBaseTestCase = new SwIntegrationToolConsoleScheduledLogAssertionTestCase(testCaseData);

                        break;

                    case ETestCaseType.log_violation:
                        currentBaseTestCase = new SwIntegrationToolConsoleLogViolationTestCase(testCaseData);

                        break;

                    case ETestCaseType.user_acceptance:
                        currentBaseTestCase = new SwIntegrationToolConsoleUserAcceptanceTestCase(testCaseData, _showToast);

                        break;

                    default:
                        continue;
                }

                OnTestCaseCreated(currentBaseTestCase);
            }
        }

        #endregion
    }
}
#endif