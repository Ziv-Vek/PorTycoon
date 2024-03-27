#if SW_STAGE_STAGE10_OR_ABOVE
using UnityEngine;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    public class SwIntegrationToolConsoleScheduledLogAssertionTestCase : SwIntegrationToolConsoleBaseTestCase
    {
        #region --- Members ---

        private LinkedListNode<string> _start;
        private LinkedListNode<string> _expected;
        private LinkedListNode<string> _forbidden;
        private LinkedListNode<string> _end;

        #endregion


        #region --- Construction ---

        public SwIntegrationToolConsoleScheduledLogAssertionTestCase(SwIntegrationToolTestCasesRawData testCasesRawData) : base(testCasesRawData)
        {
            _start = _swIntegrationToolSpecRawData.start != null ? new LinkedList<string>(_swIntegrationToolSpecRawData.start).First : null;
            _expected = _swIntegrationToolSpecRawData.expected != null ? new LinkedList<string>(_swIntegrationToolSpecRawData.expected).First : null;
            _forbidden = _swIntegrationToolSpecRawData.forbidden != null ? new LinkedList<string>(_swIntegrationToolSpecRawData.forbidden).First : null;
            _end = _swIntegrationToolSpecRawData.end != null ? new LinkedList<string>(_swIntegrationToolSpecRawData.end).First : null;
        }

        #endregion


        #region --- Private Methods ---

        protected internal override void Finish()
        {
            base.Finish();
            
            if (Status == ESwTestCaseStatus.Completed) return;

            Status = ESwTestCaseStatus.Failed;
        }

        protected override void OnLog(string log, string stacktrace, LogType type)
        {
            EnablePendingTasks(log);

            if (Status == ESwTestCaseStatus.Running)
            {
                TryCompleteTestCase(log);
                TryFailTestCase(log);
                TryEndTestCase(log);    
            }
        }

        private void EnablePendingTasks(string log)
        {
            if (!TryProgress(log, ref _start)) return;

            Status = ESwTestCaseStatus.Running;
        }

        private void TryCompleteTestCase(string log)
        {
            if (_swIntegrationToolSpecRawData.expected == null || !TryProgress(log, ref _expected)) return;

            Status = ESwTestCaseStatus.Completed;
            OnDone();
        }

        private void TryFailTestCase(string log)
        {
            if ((_swIntegrationToolSpecRawData.forbidden == null || !TryProgress(log, ref _forbidden))) return;

            Status = ESwTestCaseStatus.Failed;
            OnDone();
        }
        
        private void TryEndTestCase(string log)
        {
            if ((_swIntegrationToolSpecRawData.end == null || !TryProgress(log, ref _end))) return;

            Status = _swIntegrationToolSpecRawData.expected != null ? ESwTestCaseStatus.Failed : ESwTestCaseStatus.Completed;
            
            OnDone();
        }

        #endregion
    }
}
#endif