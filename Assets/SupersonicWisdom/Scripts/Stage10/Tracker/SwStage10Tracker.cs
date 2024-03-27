#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;
using System.Globalization;

namespace SupersonicWisdomSDK
{
    internal class SwStage10Tracker : SwCoreTracker
    {
        #region --- Constants ---
        
        protected const string UPDATE_CONVERSION_VALUE_EVENT_TYPE = "UpdateCV";

        #endregion


        #region --- Construction ---

        public SwStage10Tracker(SwCoreNativeAdapter wisdomCoreNativeAdapter, SwCoreUserData coreUserData, ISwWebRequestClient webRequestClient, SwTimerManager timerManager, SwCoreDataBridge dataBridge) : base(wisdomCoreNativeAdapter, coreUserData, webRequestClient, timerManager, dataBridge)
        { }

        #endregion


        #region --- Public Methods ---

        public void TrackConversionValueEvent(int conversionValue, string authorizationStatusString = "", string payload = "", string coarseValue = "", string postback = "")
        { 
            TrackEventInternal(UPDATE_CONVERSION_VALUE_EVENT_TYPE, EncryptData($"{conversionValue}"), EncryptData($"{payload}"), authorizationStatusString, coarseValue, postback);
        }
        
        public void TrackConversionValueEvent(int skanVersion, int conversionValue, ECVCoarseValue coarseValue, string payload = "", int postback = 0, double revenue = 0, bool didLock = false)
        {
            var encryptedPayload = EncryptData(payload);
            
            var dict = new Dictionary<string, object>()
            {
                { SwStage10RevenueCalculator.TOTAL_REVENUE, SwEncryptor.EncryptAesBase64(revenue.ToString(CultureInfo.InvariantCulture), TRACKER_ENCRYPTION_KEY, TRACKER_ENCRYPTION_IV) },
                { SwStage10CvUpdater.SKAN_VERSION_COLUMN, skanVersion },
            };
            
            if (skanVersion == 4)
            {
                dict[SwStage10CvUpdater.COARSE_VALUE] = coarseValue.ToString();
                dict[SwStage10CvUpdater.POSTBACK_NUMBER] = postback;
                dict[SwStage10CvUpdater.DID_LOCK] = didLock ? "1" : "0";
            }

            TrackEventWithParams(UPDATE_CONVERSION_VALUE_EVENT_TYPE, dict, ClientCategory.Infra, SwEncryptor.EncryptAesBase64($"{conversionValue}", TRACKER_ENCRYPTION_KEY, TRACKER_ENCRYPTION_IV), encryptedPayload);
        }

        #endregion
    }
}

#endif