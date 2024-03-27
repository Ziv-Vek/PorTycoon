#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwStage10CvModelLocalConfig : SwLocalConfig
    {
        #region --- Constants ---

        public const string SKAN_SCHEME_KEY = "swSkanScheme";
        public const string SKAN_SCHEME_VALUE = null;

        #endregion


        #region --- Properties ---

        public override Dictionary<string, object> LocalConfigValues
        {
            get
            {
                return new Dictionary<string, object>
                {
                    { SKAN_SCHEME_KEY, SKAN_SCHEME_VALUE },
                };
            }
        }

        #endregion
    }
}
#endif 