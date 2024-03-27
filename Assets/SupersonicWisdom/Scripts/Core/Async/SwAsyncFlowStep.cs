using System;

namespace SupersonicWisdomSDK
{
    internal sealed class SwAsyncFlowStep
    {
        #region --- Properties ---

        public int ExecutionIndex { get; }
        public int? MaxExecutionIndex { get; }
        public ISwAsyncRunnable Runnable { get; }

        #endregion


        #region --- Construction ---

        internal SwAsyncFlowStep(ISwAsyncRunnable runnable, int executionIndex, int? maxExecutionIndex = null)
        {
            Runnable = runnable;
            ExecutionIndex = executionIndex;
            MaxExecutionIndex = maxExecutionIndex;
        }

        #endregion
    }
}