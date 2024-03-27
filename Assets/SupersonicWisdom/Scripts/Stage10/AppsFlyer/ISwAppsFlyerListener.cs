#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using AppsFlyerSDK;

namespace SupersonicWisdomSDK
{
    public interface ISwAppsFlyerListener : IAppsFlyerConversionData
    {
        #region --- Event Handler ---

        /// <summary>
        ///     This is the missing interface in apps flyer for request-reponse callback
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnAppsFlyerRequestResponse(object sender, EventArgs args);

        #endregion
    }
}
#endif