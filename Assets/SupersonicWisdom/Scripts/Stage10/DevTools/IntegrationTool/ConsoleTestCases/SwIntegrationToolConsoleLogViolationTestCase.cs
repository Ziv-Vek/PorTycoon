#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    public class SwIntegrationToolConsoleLogViolationTestCase : SwIntegrationToolConsoleBaseTestCase
    {
        #region --- Members ---

        private LinkedListNode<string> _forbidden;

        #endregion


        #region --- Construction ---

        public SwIntegrationToolConsoleLogViolationTestCase(SwIntegrationToolTestCasesRawData testCasesRawData) : base(testCasesRawData)
        {
            _forbidden = _swIntegrationToolSpecRawData.forbidden != null ? new LinkedList<string>(_swIntegrationToolSpecRawData.forbidden).First : null;
            Status = ESwTestCaseStatus.Completed;
        }

        #endregion


        #region --- Private Methods ---

        protected override void OnLog(string log, string stackTrace, LogType type)
        {
            TryFailTestCase(log);
        }

        private void TryFailTestCase(string log)
        {
            if (_swIntegrationToolSpecRawData.forbidden == null || !TryProgress(log, ref _forbidden)) return;
            
            Status = ESwTestCaseStatus.Failed;
            OnDone();
        }

        #endregion
    }
}
#endif