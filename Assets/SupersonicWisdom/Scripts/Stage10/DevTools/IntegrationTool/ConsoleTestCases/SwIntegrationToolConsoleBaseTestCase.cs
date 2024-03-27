#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SupersonicWisdomSDK
{
    public abstract class SwIntegrationToolConsoleBaseTestCase : SwBaseIntegrationToolTestCase
    {
        #region --- Members ---

        protected readonly SwIntegrationToolSpecRawData _swIntegrationToolSpecRawData;

        #endregion


        #region --- Construction ---

        protected SwIntegrationToolConsoleBaseTestCase(SwIntegrationToolTestCasesRawData testCasesRawData)
        {
            ID = testCasesRawData.idWithParams ?? testCasesRawData.id;
            Status = ESwTestCaseStatus.Pending;
            Description = testCasesRawData.description;
            Title = testCasesRawData.title;
            IsRegExp = testCasesRawData.isRegExp;

            _swIntegrationToolSpecRawData = testCasesRawData.spec;
            Application.logMessageReceivedThreaded += OnLog;
        }

        #endregion


        #region --- Private Methods ---

        protected bool TryProgress(string log, ref LinkedListNode<string> linkedListNode)
        {
            if (linkedListNode == null)
            {
                return true;
            }

            var isDone = false;

            if (IsMatch(linkedListNode.Value, log))
            {
                linkedListNode = linkedListNode.Next;

                if (linkedListNode == null)
                {
                    isDone = true;
                }
            }

            return isDone;
        }

        protected bool IsMatch(string format, string log)
        {
            if (format == null || log == null) return false;
            
            return IsRegExp ? IsRegexMatch(format, log) : log.Contains(format);
        }
        
        private bool IsRegexMatch(string regex, string str)
        {
            try
            {
                var pattern = new Regex(regex);
                return pattern.IsMatch(str);
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion


        #region --- Event Handler ---

        protected abstract void OnLog(string log, string stackTrace, LogType type);

        protected internal override void Finish()
        {
            OnDone();
        }
        
        protected override void OnDone()
        {
            Application.logMessageReceivedThreaded -= OnLog;
        }

        #endregion
    }
}
#endif