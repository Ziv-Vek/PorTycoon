#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal abstract class BaseCvUpdater
    {
        #region --- Constants ---
        
        public const string SKAN4_V1_IAP = "skan4_V1_IAP";
        public const string USER_CV = "user_cv{0}";
        public const int LOCK_FROM_TIME_DEFAULT_VALUE = -1;
        public const int LOCK_FROM_COARSE_DEFAULT_VALUE = -1;

        #endregion


        #region --- Members ---

        protected readonly MonoBehaviour _mono;
        private readonly SwStage10Tracker _tracker;
        private readonly SwStage10UserData _userData;
        private readonly SwStage10RevenueCalculator _revenueCalculator;

        private int _cv;
        private int _version;
        private SwCvConfig _config;
        protected bool _isInitialized;
        private ECVCoarseValue _coarse;

        #endregion


        #region --- Properties ---

        public Tuple<EConfigListenerType, EConfigListenerType> ListenerType
        {
            get { return new Tuple<EConfigListenerType, EConfigListenerType>(EConfigListenerType.FinishWaitingForRemote, EConfigListenerType.GameStarted); }
        }

        internal SwPostbackWindowManager WindowManager { get; private set; }
        
        internal int CurrentCv
        {
            get { return WindowManager.CurrentWindowType != EPostbackWindow.Done ? _cv : -1; }
        }
        
        internal ECVCoarseValue CurrentCoarse
        {
            get { return WindowManager.CurrentWindowType != EPostbackWindow.Done ? _coarse : ECVCoarseValue.Unknown; }
        }

        protected double Revenue
        {
            get
            {
                return _revenueCalculator?.Revenue ?? 0;
            }
        }

        #endregion


        #region --- Construction ---

        public BaseCvUpdater(MonoBehaviour mono, SwStage10RevenueCalculator revenueCalculator, SwStage10UserData userData, SwStage10Tracker tracker)
        {
            _revenueCalculator = revenueCalculator;
            _userData = userData;
            _tracker = tracker;
            _mono = mono;
            _cv = -1;
            _coarse = ECVCoarseValue.Unknown;
        }

        ~BaseCvUpdater()
        {
            if (_revenueCalculator == null) return;
            
            _revenueCalculator.RevenueChangedEvent -= TryUpdateCv;
        }

        #endregion
        

        #region --- public Methods ---
        
        public IEnumerator TryUpdateFirstCvUpdate()
        {
            if (!SwUtils.System.IsRunningOnIos() || !_userData.IsNew) yield break;

            yield return new WaitForSeconds(1);

            SwSKAdNetworkAdapter.UpdatePostbackConversionAndCoarseValue(0, (int)0, false, authorizationStatus => 
                OnCvUpdated(0, 0, null, 0, 0, false));
        }

        #endregion

        
        #region --- Private Methods ---

        protected void Init(SwCvConfig config, MonoBehaviour mono)
        {
            if (!SwUtils.System.IsRunningOnIos()) return;
            
            _isInitialized = true;
            _version = SwSKAdNetworkAdapter.GetSkanVersion();
            _config = config;
            _revenueCalculator.RevenueChangedEvent += TryUpdateCv;
            
            InitWindowManager(config, mono);
            
            _cv = SwCvUtils.GetCv(_config, WindowManager.CurrentWindowType, Revenue);
            _coarse = SwCvUtils.GetCoarse(_config, WindowManager.CurrentWindowType, Revenue);
            
            SwInfra.Logger.Log(EWisdomLogType.ConversionValue, $"Skan version = {_version}, cv = {_cv}, coarse = {_coarse}");
        }

        private void InitWindowManager(SwCvConfig config, MonoBehaviour mono)
        {
            WindowManager = new SwPostbackWindowManager();
            WindowManager.Init(_userData.InstallDateTime, config.ToLockFromTimeDictionary(), mono, OnWindowLocked);
        }

        protected void Configure(SwCvConfig config)
        {
            WindowManager.UpdateLockFromTimeDict(config.ToLockFromTimeDictionary());
        }

        private void OnWindowLocked()
        {
            TryUpdateCv(_revenueCalculator.Revenue);
        }

        protected virtual void OnCvUpdated(int newConversionValue, ECVCoarseValue newCoarseValue, object payload, int postbackNumber, double revenue, bool didLock)
        {
            _cv = newConversionValue;
            _coarse = newCoarseValue;

            if (WindowManager != null)
            {
                WindowManager.IsLocked = didLock;
            }

            _tracker?.TrackConversionValueEvent(_version, newConversionValue, newCoarseValue, payload?.ToString() ?? string.Empty, postbackNumber, revenue, didLock);
            _userData?.AfterUserStateChangeInternal(SwUserStateChangeReason.UpdateImpConversionValue, true);
            SwInfra.Logger.Log(EWisdomLogType.ConversionValue, "Update CV operation ended:\nNew Cv: {0}\nNew coarse: {1}\nDid lock: {2}\nPayLoad {3}\nPostback {4}".Format(_cv, _coarse, didLock, payload ?? "", postbackNumber));
        }

        #endregion


        #region --- Event Handler ---
        
        private void TryUpdateCv(double newRevenue, object payload = null)
        {
            if (!_isInitialized) return;
            
            SwInfra.Logger.Log(EWisdomLogType.ConversionValue, "Try update CV with params:\nRevenue - {0}\npayLoad - {1}".Format(newRevenue, payload ?? ""));

            switch (_version)
            {
                case 4:
                    UpdateSkan4(newRevenue, payload);
                    break;
                
                default:
                    UpdateSkan3OrLower(newRevenue, payload);
                    break;
            }
        }

        private void UpdateSkan3OrLower(double newRevenue, object payload)
        {
            var newCv = SwCvUtils.GetCv(_config, WindowManager.CurrentWindowType, newRevenue);
            var windowNumber = WindowManager.CurrentWindowNumber;
            var shouldLockFromTime = WindowManager.ShouldLock;
            var isLocked = WindowManager.IsLocked;

            var didCvChanged = _cv != newCv;
            var isWindowZero = windowNumber == 0;

            SwInfra.Logger.Log(EWisdomLogType.ConversionValue, "Calculation:\nNew Cv: {0}\nShould Lock : {1}\nIs Locked : {2}\nPayLoad {3}\nPostback {4}".Format(newCv, shouldLockFromTime, isLocked, payload ?? "", windowNumber));
            SwInfra.Logger.Log(EWisdomLogType.ConversionValue, "didCvChanged: {0}".Format(didCvChanged));

            if (isLocked || !isWindowZero || !didCvChanged) return;
            
            SwSKAdNetworkAdapter.UpdatePostbackConversionValue(newCv, authorizationStatusString =>
            {
                OnCvUpdated(newCv, ECVCoarseValue.Unknown, payload, windowNumber, newRevenue, shouldLockFromTime);
            });
        }

        private void UpdateSkan4(double newRevenue, object payload)
        {
            var newCv = SwCvUtils.GetCv(_config, WindowManager.CurrentWindowType, newRevenue);
            var newCoarse = SwCvUtils.GetCoarse(_config, WindowManager.CurrentWindowType, newRevenue);
            var shouldLockFromCoarse = SwCvUtils.ShouldLock(_config, WindowManager.CurrentWindowType, newCoarse);
            var shouldLockFromTime = WindowManager.ShouldLock;
            var shouldLock = shouldLockFromCoarse | shouldLockFromTime;
            var postback = WindowManager.CurrentWindowNumber;
            var isLocked = WindowManager.IsLocked;
            
            var didCvChanged = _cv != newCv;
            var didCoarseChanged = _coarse != newCoarse;
            var didLockChangedAndTrue = shouldLock && !WindowManager.IsLocked;

            SwInfra.Logger.Log(EWisdomLogType.ConversionValue, 
                "Calculation:\nNew Cv: {0}\nNew coarse: {1}\nShould lock from coarse: {2}\nnShould lock from time: {3}\nPayLoad {4}\nPostback {5}\nIsLocked {6}"
                    .Format(newCv, newCoarse, shouldLockFromCoarse, shouldLockFromTime, payload ?? "", postback, isLocked));
            SwInfra.Logger.Log(EWisdomLogType.ConversionValue, 
                "didCvChanged: {0}\ndidCoarseChanged: {1}\ndidLockChangedAndTrue: {2}"
                .Format(didCvChanged, didCoarseChanged, didLockChangedAndTrue));
            
            if (isLocked || !didCvChanged && !didCoarseChanged && !didLockChangedAndTrue) return;

            SwSKAdNetworkAdapter.UpdatePostbackConversionAndCoarseValue(newCv, (int)newCoarse, shouldLock, authorizationStatus => 
                OnCvUpdated(newCv, newCoarse, payload, postback, newRevenue, shouldLock));
        }

        #endregion
    }
}
#endif