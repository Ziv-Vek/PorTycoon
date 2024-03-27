#if SW_STAGE_STAGE10_OR_ABOVE
namespace SupersonicWisdomSDK
{
    internal readonly struct FpsSample
    {
        #region --- Properties ---

        public float Duration { get; }
        public float SessionTimestamp { get; }
        public int FpsValue { get; }
        public int Frames { get; }

        public float StartTime
        {
            get
            {
                return SessionTimestamp - Duration;
            }
        }

        #endregion


        #region --- Construction ---

        public FpsSample(int fpsValue, float sessionTimestamp, float duration, int frames)
        {
            Frames = frames;
            SessionTimestamp = sessionTimestamp;
            FpsValue = fpsValue;
            Duration = duration;
        }

        #endregion


        #region --- Public Methods ---

        public override string ToString()
        {
            return $"{{sessionTimestamp: {SessionTimestamp}, FPS: {FpsValue}, duration: {Duration}, frames: {Frames}}}";
        }

        #endregion
    }
}
#endif