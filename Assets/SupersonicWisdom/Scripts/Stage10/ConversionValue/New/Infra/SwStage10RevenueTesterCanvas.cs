#if SW_STAGE_STAGE10_OR_ABOVE
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace SupersonicWisdomSDK
{
    internal class SwStage10RevenueTesterCanvas : SwBaseTestPanel
    {
        #region --- Constants ---

        private const string TESTER_KEY = "CvTester";
        
        #endregion
        
        
        #region --- Inspector ---

        [SerializeField] private Text _windowNumber;
        [SerializeField] private Text _startDate;
        [SerializeField] private Text _countdown;
        [SerializeField] private Text _currentCv;
        [SerializeField] private Text _currentCoarse;
        [SerializeField] private Text _status;
        [SerializeField] private Text _currentRevenue;
        [SerializeField] private Text _adsRevenue;
        [SerializeField] private Text _iapRevenue;
        [SerializeField] private Button _revenueDirectButtonPlus;
        [SerializeField] private Button _revenueDirectButtonMinus;

        #endregion


        #region --- Members ---

        private BaseCvUpdater _cvUpdater;
        private bool _isCvUpdaterActive;
        private ISwTimerListener _timer;
        private SwStage10RevenueCalculator _revenueCalculator;

        #endregion


        #region --- Properties ---

        public override string PanelName
        {
            get { return "Revenue"; }
        }

        #endregion


        #region --- Mono Override ---

        private void Update()
        {
            if (!_isReady)
            {
                return;
            }

            if (!_isCvUpdaterActive && _cvUpdater?.WindowManager != null && _cvUpdater?.WindowManager?.Timer != null)
            {
                _isCvUpdaterActive = true;
                _timer = _cvUpdater.WindowManager.Timer;
            }

            UpdateText();
        }

        #endregion


        #region --- Public Methods ---

        public virtual void PlusOneRevenue()
        {
            _revenueCalculator.TryAddRevenue(new SwStage10RevenueData(1, SwRevenueType.Invalid, true));
        }

        public virtual void MinusOneRevenue()
        {
            _revenueCalculator.TryAddRevenue(new SwStage10RevenueData(-1, SwRevenueType.Invalid, true));
        }

        #endregion


        #region --- Private Methods ---

        protected override void OnSwReady()
        {
            base.OnSwReady();
            
            _revenueCalculator = SupersonicWisdom.Api.GetRevenueCalculator();
            _cvUpdater = SupersonicWisdom.Api.GetCvUpdater();
            
            _revenueDirectButtonPlus.interactable = true;
            _revenueDirectButtonMinus.interactable = true;
        }

        private void UpdateText()
        {
            if (_isCvUpdaterActive)
            {
                _windowNumber.text = _cvUpdater.WindowManager.CurrentWindowType.ToString();
                _status.text = _cvUpdater.WindowManager.IsLocked ? "Locked" : " Open";
                _countdown.text = _timer != null && _timer.IsEnabled ? (_timer.Duration - _timer.Elapsed).ToString(CultureInfo.InvariantCulture) : "--:--:--";
                _startDate.text = _cvUpdater.WindowManager.Model.CurrentStartTime.ToString(CultureInfo.InvariantCulture);
                _currentCv.text = _cvUpdater.CurrentCv.ToString();
                _currentCoarse.text = _cvUpdater.CurrentCoarse.ToString();
            }

            _currentRevenue.text = _revenueCalculator.Revenue.ToString("N");
            _adsRevenue.text = _revenueCalculator.GetRevenueByType(SwRevenueType.Ad).ToString("N");
            _iapRevenue.text = _revenueCalculator.GetRevenueByType(SwRevenueType.IAP).ToString("N");
        }

        #endregion
    }
}
#endif