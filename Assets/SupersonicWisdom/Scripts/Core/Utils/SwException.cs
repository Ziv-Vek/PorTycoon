using System;

namespace SupersonicWisdomSDK
{
    public class SwException : Exception
    {
        #region --- Construction ---

        public SwException(string exceptionMessage) : base($"SupersonicWisdom: An exception has occurred. {exceptionMessage}")
        { }

        #endregion
    }
}