using System;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwCrossSessionTimer : SwTimer
    {
        #region --- Members ---

        private float _saveInterval;
        private float _lastSave;
        private Action _onCreated;
        private string _key;

        #endregion


        #region --- Mono Override ---
        
        protected void Start()
        {
            _key = $"sw_{Name}"; 
            Elapsed = SwInfra.KeyValueStore.GetInt(_key);

            if (Elapsed >= Duration)
            {
                ClearElapsed();
            }
            else if (Elapsed > 0)
            {
                Pause();
            }
            
            OnFinishEvent += ClearElapsed;
            _onCreated?.Invoke();
        }

        protected override void OnApplicationPause(bool didPause)
        {
            base.OnApplicationPause(didPause);
            
            if (!didPause) return;

            SaveElapsed();
        }

        protected override void Reset()
        {
            base.Reset();

            _lastSave = 0;
        }

        #endregion


        #region --- Public Methods ---

        public static SwCrossSessionTimer Create(GameObject gameObject, string name = "", float durationSeconds = 0, bool pauseWhenUnityOutOfFocus = false, float saveInterval = 5f, Action onCreated = null)
        {
            var instance = CreateGeneric<SwCrossSessionTimer>(gameObject, name, durationSeconds, pauseWhenUnityOutOfFocus);
            instance._saveInterval = saveInterval;
            instance._onCreated = onCreated; 

            return instance;
        }

        #endregion


        #region --- Private Methods ---

        protected override void BeforeInvokeTick()
        {
            base.BeforeInvokeTick();

            if (!(Elapsed - _lastSave >= _saveInterval)) return;

            _lastSave = Elapsed;
            SaveElapsed();
        }

        private void SaveElapsed()
        {
            SwInfra.KeyValueStore.SetInt(_key, (int)Elapsed).Save();
        }
        
        private void ClearElapsed()
        {
            Elapsed = 0;
            _lastSave = 0;
            SwInfra.KeyValueStore.DeleteKey(_key).Save();
        }

        #endregion
    }
}