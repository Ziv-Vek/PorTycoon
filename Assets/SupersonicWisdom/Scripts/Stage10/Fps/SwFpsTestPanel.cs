#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwFpsTestPanel : SwBaseTestPanel
    {
        #region --- Inspector ---

        [SerializeField] private TextMeshProUGUI _minFps;
        [SerializeField] private TextMeshProUGUI _medianFps;
        [SerializeField] private TextMeshProUGUI _averageFps;
        [SerializeField] private TextMeshProUGUI _criticalDuration;

        #endregion


        #region --- Members ---

        private Guid _fpsMeterKey;
        private SwCoreFpsMeasurementManager _fpsManager;

        #endregion


        #region --- Properties ---

        public override string PanelName
        {
            get { return "FPS"; }
        }

        #endregion


        #region --- Mono Override ---

        private void Update()
        {
            if (!_isReady) return;
            
            if (_fpsMeterKey == Guid.Empty)
            {
                _fpsMeterKey = _fpsManager.Start();
            }

            var measurement = _fpsManager.Measure(_fpsMeterKey);

            if (measurement == null)
            {
                return;
            }

            UpdatePanelText(measurement);
        }

        #endregion


        #region --- Private Methods ---

        private void UpdatePanelText(FpsMeasurementSummary measurement)
        {
            _minFps.SetText(measurement.criticalMinFps.ToString());
            _medianFps.SetText(measurement.medianFps.ToString());
            _averageFps.SetText(measurement.averageFps.ToString());
            _criticalDuration.SetText(measurement.criticalFpsDuration.SwToString());
        }

        protected override void OnSwReady()
        {
            base.OnSwReady();
            
            _fpsManager = SupersonicWisdom.Api.GetFpsManager();
        }

        #endregion
    }
}
#endif