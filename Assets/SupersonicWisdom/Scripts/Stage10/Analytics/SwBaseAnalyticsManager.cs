#if SW_STAGE_STAGE10_OR_ABOVE
using System;

namespace SupersonicWisdomSDK
{
    internal abstract class SwBaseAnalyticsManager
    {
        #region --- Members ---

        protected readonly ISwTracker _tracker;

        #endregion


        #region --- Construction ---

        protected SwBaseAnalyticsManager(ISwTracker tracker)
        {
            _tracker = tracker;
        }

        #endregion
    }
}
#endif