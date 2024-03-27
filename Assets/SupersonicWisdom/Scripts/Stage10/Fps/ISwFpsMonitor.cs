#if SW_STAGE_STAGE10_OR_ABOVE
namespace SupersonicWisdomSDK
{
    internal interface ISwFpsMonitor
    {
        bool IsCollecting { get; }
        void Start();
        FpsMeasurementSummary Measure();
        void Stop();
    }
}
#endif