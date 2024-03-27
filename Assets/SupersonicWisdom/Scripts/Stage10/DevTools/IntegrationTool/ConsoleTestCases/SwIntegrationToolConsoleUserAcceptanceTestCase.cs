#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    public class SwIntegrationToolConsoleUserAcceptanceTestCase : SwIntegrationToolConsoleBaseTestCase
    {
        #region --- Members ---

        private readonly Action<string, float, Action, Action> _onLogDetected;

        #endregion


        #region --- Properties ---

        private string Message { get; }
        private string LogTrigger { get; }

        #endregion


        #region --- Construction ---

        public SwIntegrationToolConsoleUserAcceptanceTestCase(SwIntegrationToolTestCasesRawData testCasesRawData, Action<string, float, Action, Action> showToast) : base(testCasesRawData)
        {
            _onLogDetected = showToast;
            LogTrigger = testCasesRawData.spec.logTrigger;
            Message = testCasesRawData.spec.message;
            Status = ESwTestCaseStatus.Running;
        }

        #endregion


        #region --- Private Methods ---

        protected internal override void Finish()
        {
            base.Finish();
            
            if (Status == ESwTestCaseStatus.Completed) return;
            
            Status = ESwTestCaseStatus.Failed;
        }

        protected override void OnLog(string log, string stackTrace, LogType type)
        {
            if (IsMatch(LogTrigger, log))
            {
                _onLogDetected?.Invoke(Message, 10f, OnAccept, OnDecline);
            }
        }

        private void OnDecline()
        {
            Status = ESwTestCaseStatus.Failed;
            OnDone();
        }
        
        private void OnAccept()
        {
            Status = ESwTestCaseStatus.Completed;
        }

        #endregion
    }
}
#endif