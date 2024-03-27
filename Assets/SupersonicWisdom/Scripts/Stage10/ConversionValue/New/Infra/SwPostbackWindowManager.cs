#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwPostbackWindowManager
    {
        #region --- Constants ---

        private const string POSTBACK_FORMAT = "postback_{0}";
        private const string TIMER_NAME = "CvUpdaterWindowTimer";

        #endregion


        #region --- Members ---

        internal ISwTimer Timer { get; private set; }
        internal SwPostbackWindowsModel Model;
        
        private SwCvConfig _config;
        private Action _onWindowLocked;

        #endregion


        #region --- Properties ---

        public EPostbackWindow CurrentWindowType
        {
            get { return Model.CurrentWindowType; }
        }

        public int CurrentWindowNumber
        {
            get { return (int)CurrentWindowType; }
        }

        public bool IsLocked
        {
            get
            {
                return Model.IsLocked(CurrentWindowType);
            }
            set
            {
                if (value)
                {
                    Model.SetLock(CurrentWindowType);
                    RestartAndUpdateTimer();
                }
            }
        }

        public bool ShouldLock
        {
            get
            {
                return Model.PassLockTime;
            }
        }

        #endregion


        #region --- Public Methods ---

        public void Init(DateTime installDate, Dictionary<EPostbackWindow, float> lockFromTimeDict, MonoBehaviour mono, Action onWindowLocked)
        {
            _onWindowLocked = onWindowLocked;

            CreateTimer(mono);
            LoadOrStartWindows(installDate);
            UpdateLockFromTimeDict(lockFromTimeDict);
        }

        public void UpdateLockFromTimeDict(Dictionary<EPostbackWindow, float> lockFromTimeDict)
        {
            Model.Setup(lockFromTimeDict, InvokeOnWindowLocked);
            RestartAndUpdateTimer();
        }

        #endregion


        #region --- Private Methods ---

        private void InvokeOnWindowLocked()
        {
            _onWindowLocked?.Invoke();
        }

        private void CloseWindow()
        {
            Update();
        }

        private void CreateTimer(Component mono)
        {
            Timer = SwTimer.Create(mono.gameObject, TIMER_NAME);
        }

        private void RestartAndUpdateTimer()
        {
            Timer.StopTimer();
            
            ClearTimersListeners();

            if (!Model.PassLockTime)
            {
                Timer.SetDuration((float)(Model.CurrentLockTime - DateTime.UtcNow).TotalSeconds);
                Timer.OnFinishEvent += OnLockWindowTimerFinished;
            }
            else
            {
                Timer.SetDuration((float)(Model.WindowEndTime - DateTime.UtcNow).TotalSeconds);
                Timer.OnFinishEvent += OnCloseWindowTimerFinished;
            }
            
            Timer.StartTimer();
        }

        private void ClearTimersListeners()
        {
            Timer.OnFinishEvent -= OnLockWindowTimerFinished;
            Timer.OnFinishEvent -= OnCloseWindowTimerFinished;
        }

        private void LoadOrStartWindows(DateTime installDate)
        {
            Model = SwPostbackWindowsModel.Load();
            
            if (Model == null)
            {
                Model = new SwPostbackWindowsModel();
                Model.Start(installDate);
            }
        }

        private void Update()
        {
            Model.Calculate(DateTime.UtcNow);
            RestartAndUpdateTimer();
        }

        #endregion


        #region --- Event Handler ---

        private void OnLockWindowTimerFinished()
        {
            Timer.OnFinishEvent -= OnLockWindowTimerFinished;
            
            Update();
        }

        private void OnCloseWindowTimerFinished()
        {
            Timer.OnFinishEvent -= OnCloseWindowTimerFinished;
            
            CloseWindow();
        }

        #endregion
    }
}
#endif