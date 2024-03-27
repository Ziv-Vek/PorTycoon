#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwAliveSenderLocalConfig : SwLocalConfig
    {
        #region --- Constants ---

        public const string ALIVE_EVENT_INTERVALS_CONFIG_KEY = "swAliveEventIntervals";
        public const int ALIVE_EVENT_INTERVALS_CONFIG_VALUE = -1;

        #endregion


        #region --- Properties ---

        public override Dictionary<string, object> LocalConfigValues
        {
            get
            {
                return new Dictionary<string, object>
                {
                    {
                        // configuration is string since can be also a json
                        ALIVE_EVENT_INTERVALS_CONFIG_KEY, ALIVE_EVENT_INTERVALS_CONFIG_VALUE
                    },
                };
            }
        }

        #endregion
    }
}
#endif