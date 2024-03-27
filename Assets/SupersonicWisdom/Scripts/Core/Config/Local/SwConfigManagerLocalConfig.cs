using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwConfigManagerLocalConfig : SwLocalConfig
    {
        internal const string SHOULD_REPORT_CONFIG_ITERATION_KEY = "swShouldReportConfigIterationEvent";
        internal const bool SHOULD_REPORT_CONFIG_ITERATION_VALUE = true;
        
        public override Dictionary<string, object> LocalConfigValues
        {
            get
            {
                return new Dictionary<string, object>()
                {
                    { SHOULD_REPORT_CONFIG_ITERATION_KEY, SHOULD_REPORT_CONFIG_ITERATION_VALUE },
                };
            }
        }
    }
}