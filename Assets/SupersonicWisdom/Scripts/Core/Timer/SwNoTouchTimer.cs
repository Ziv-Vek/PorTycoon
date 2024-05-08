using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwNoTouchTimer : SwTimer
    {
        #region --- Mono Override ---

        protected override void Update()
        {
            if (SwUtils.System.IsRunningOnDevice() && Input.touchCount > 0 || !SwUtils.System.IsRunningOnDevice() && Input.GetMouseButton(0))
            {
                if (IsDisabled) return;
                
                if (IsEnabled || DidFinish)
                {
                    SwInfra.Logger.Log(EWisdomLogType.Time, $"StartTimer due to touch | {Name}");
                    StartTimer();
                }
                else if (IsPaused)
                {
                    Elapsed = 0;
                    DidFinish = false;
                }
            }

            base.Update();
        }

        #endregion


        #region --- Public Methods ---

        public new static SwNoTouchTimer Create(GameObject gameObject, string name = "", float duration = 0, bool shouldPauseWhenUnityOutOfFocus = false)
        {
            return CreateGeneric<SwNoTouchTimer>(gameObject, name, duration, shouldPauseWhenUnityOutOfFocus);
        }

        #endregion
    }
}