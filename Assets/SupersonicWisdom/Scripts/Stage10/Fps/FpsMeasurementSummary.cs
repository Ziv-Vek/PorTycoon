#if SW_STAGE_STAGE10_OR_ABOVE
using System;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal class FpsMeasurementSummary
    {
        public int averageFps;
        public int samplesCount;
        public int medianFps;
        public int criticalMinFps;
        public float fpsMeasurementDuration;
        public float criticalFpsDuration;
    }
}
#endif