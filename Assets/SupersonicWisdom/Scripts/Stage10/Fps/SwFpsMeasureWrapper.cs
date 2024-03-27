#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    internal class SwCoreFpsMeasureWrapper
    {
        #region --- Members ---

        private readonly SwCoreFpsMeasurementManager _fpsMeasurementManager;
        private Guid _fpsGuid;

        #endregion


        #region --- Properties ---

        [CanBeNull]
        protected internal FpsMeasurementSummary Summary
        {
            get { return IsMeasuring ? _fpsMeasurementManager.Measure(_fpsGuid) : null; }
        }

        private bool IsMeasuring
        {
            get { return _fpsMeasurementManager.IsMeasuring(_fpsGuid); }
        }

        #endregion


        #region --- Construction ---

        public SwCoreFpsMeasureWrapper(SwCoreFpsMeasurementManager fpsMeasurementManager)
        {
            _fpsMeasurementManager = fpsMeasurementManager;
        }

        #endregion


        #region --- Private Methods ---

        protected internal void StartMeasure()
        {
            if (IsMeasuring) return;

            _fpsGuid = _fpsMeasurementManager.Start();
        }

        protected internal void StopMeasure()
        {
            if (!IsMeasuring) return;

            _fpsMeasurementManager.Stop(_fpsGuid);
            _fpsGuid = Guid.Empty;
        }

        #endregion
    }
}
#endif