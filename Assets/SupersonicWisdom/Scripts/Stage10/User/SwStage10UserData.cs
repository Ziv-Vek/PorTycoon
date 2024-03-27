#if SW_STAGE_STAGE10_OR_ABOVE

namespace SupersonicWisdomSDK
{
    internal class SwStage10UserData : SwCoreUserData, ISwStage10ConfigListener
    {
        #region --- Properties ---

        public string AppsFlyerId { get; set; }

        #endregion


        #region --- Construction ---

        public SwStage10UserData(ISwSettings settings, ISwAdvertisingIdsGetter idsGetter, SwUserActiveDay activeDay) : base(settings, idsGetter, activeDay) { }

        #endregion


        public void OnConfigResolved(ISwStage10InternalConfig swConfigAccessor, ISwConfigManagerState state)
        {
            if (swConfigAccessor?.Agent != null)
            {
                Country = swConfigAccessor.Agent.country;
            }
        }
    }
}

#endif