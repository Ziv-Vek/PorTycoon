using System;

namespace SupersonicWisdomSDK
{
    internal interface ISwLogger
    {
        #region --- Properties ---

        bool LogViaNetwork { get; set; }

        #endregion


        #region --- Public Methods ---

        public void Setup(Func<bool> isDebugEnabledFunc);
        bool IsEnabled ();
        void Log(EWisdomLogType wisdomLogType, string message = "");
        void LogError(EWisdomLogType wisdomLogType, Exception message);
        void LogError(EWisdomLogType wisdomLogType, string message);
        void LogWarning(EWisdomLogType wisdomLogType, string message);

        #endregion
    }
}