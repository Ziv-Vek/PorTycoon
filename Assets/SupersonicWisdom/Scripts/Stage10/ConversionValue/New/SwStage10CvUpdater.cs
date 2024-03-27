#if SW_STAGE_STAGE10_OR_ABOVE
using Newtonsoft.Json;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwStage10CvUpdater : BaseCvUpdater, ISwStage10ConfigListener, ISwLocalConfigProvider
    {
        #region --- Constants ---
        
        internal const string SKAN_VERSION_COLUMN = "skanVersion";
        internal const string DID_LOCK = "didLock";
        internal const string COARSE_VALUE = "coarseValue";
        internal const string POSTBACK_NUMBER = "postbackNumber";
        
        #endregion
        
        
        #region --- Construction ---

        public SwStage10CvUpdater(MonoBehaviour mono, SwStage10RevenueCalculator revenueCalculator, SwStage10UserData userData, SwStage10Tracker tracker) : base(mono, revenueCalculator, userData, tracker)
        {
        }

        #endregion


        #region --- Public Methods ---

        public void OnConfigResolved(ISwStage10InternalConfig swConfigAccessor, ISwConfigManagerState state)
        {
            SwCvConfig cv = null;

            if (swConfigAccessor.Cv?.model != null)
            {
                cv = swConfigAccessor.Cv;
            }
            else
            {
                var configValue = swConfigAccessor.GetValue(SwStage10CvModelLocalConfig.SKAN_SCHEME_KEY, SwStage10CvModelLocalConfig.SKAN_SCHEME_VALUE);

                if (!configValue.SwIsNullOrEmpty())
                {
                    cv = JsonConvert.DeserializeObject<SwCvConfig>(configValue);
                }
            }
            
            var isEnabled = cv?.model == SKAN4_V1_IAP;

            if (isEnabled)
            {
                if (!_isInitialized)
                {
                    Init(cv, _mono);
                }
                else
                {
                    Configure(cv);
                }
            }
        }

        public SwLocalConfig GetLocalConfig()
        {
            return new SwStage10CvModelLocalConfig();
        }

        #endregion
    }
}

#endif