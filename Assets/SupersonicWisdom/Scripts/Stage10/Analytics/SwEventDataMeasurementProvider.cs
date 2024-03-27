#if SW_STAGE_STAGE10_OR_ABOVE
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal class SwEventDataMeasurementProvider
    {
        #region --- Members ---

        private readonly SwCoreFpsMeasureWrapper _fpsMeasureWrapper;

        #endregion


        #region --- Properties ---

        private Dictionary<string, object> Data
        {
            get
            {
                return Summary?.fpsMeasurementDuration <= 0 
                    ? new Dictionary<string, object>() 
                    : new SwJsonDictionary(Summary);
            }
        }

        private FpsMeasurementSummary Summary
        {
            get
            {
                return _fpsMeasureWrapper.Summary;
            }
        }

        #endregion


        #region --- Construction ---

        public SwEventDataMeasurementProvider(SwCoreFpsMeasurementManager fpsMeasurementManager)
        {
            _fpsMeasureWrapper = new SwCoreFpsMeasureWrapper(fpsMeasurementManager);
        }

        #endregion


        #region --- Public Methods ---

        public void StartMeasure()
        {
            _fpsMeasureWrapper.StartMeasure();
        }

        public Dictionary<string, object> StopMeasure()
        {
            var data = Data; 
            _fpsMeasureWrapper.StopMeasure();
            
            return data;
        }

        public Dictionary<string, object> GetDataAndRestart()
        {
            var data = StopMeasure();

            StartMeasure();
            
            return data;
        }

        #endregion
    }
}
#endif