#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwFpsMonitorLocalConfig : SwLocalConfig
    {
        #region --- Constants ---

        public const string FPS_MEASUREMENTS_INTERVAL_KEY = "swFpsMeasurementIntervals";
        public const int FPS_MEASUREMENTS_INTERVAL_VALUE = 1;
        public const string FPS_CRITICAL_THRESHOLD_KEY = "swFpsCriticalThreshold";
        public const int FPS_CRITICAL_THRESHOLD_VALUE = 20;
        public const string FPS_SHOULD_REPORT_INVALID_FPS_KEY = "swFpsShouldReportInvalidFps";
        public const bool FPS_SHOULD_REPORT_INVALID_FPS_VALUE = false;

        #endregion


        #region --- Properties ---

        public override Dictionary<string, object> LocalConfigValues
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { FPS_MEASUREMENTS_INTERVAL_KEY, FPS_MEASUREMENTS_INTERVAL_VALUE },
                    { FPS_CRITICAL_THRESHOLD_KEY, FPS_CRITICAL_THRESHOLD_VALUE },
                    { FPS_SHOULD_REPORT_INVALID_FPS_KEY, FPS_SHOULD_REPORT_INVALID_FPS_VALUE },
                };
            }
        }

        #endregion
    }
}
#endif