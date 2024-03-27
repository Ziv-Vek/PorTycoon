using UnityEngine;
using UnityEngine.EventSystems;

namespace SupersonicWisdomSDK
{
    public class SwUiUtils
    {
        #region --- Members ---
         
        private float _prevTimeScale;
        private bool _prevAutoRotateToPortrait;
        private bool _prevAutorotateToPortraitUpsideDown;
        
        #endregion
        
        #region --- Public Methods ---
        
        public void LockUI()
        {
            LockUnityUI();
            LockScreenRotation();
        }
        
        public void UnlockUI()
        {
            UnlockUnityUI();
            RevertLockScreenRotation();
        }
        
        public bool IsLandscapeLayout()
        {
#if UNITY_EDITOR
            return Screen.height <= Screen.width;
#elif (UNITY_IOS || UNITY_ANDROID) && UNITY_2019_4_OR_NEWER
            return Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight;
#elif UNITY_IOS || UNITY_ANDROID
            return Screen.orientation == ScreenOrientation.Landscape;
#else
            return true;
#endif
        }
        
        #endregion
        
        #region --- Private Methods ---
        
        private void LockUnityUI()
        {
            _prevTimeScale = Time.timeScale;

            Time.timeScale = 0;

            if (SwUtils.System.IsRunningOnDevice())
            {
                Object.FindObjectOfType<EventSystem>().enabled = false;
            }
        }
        
        private void LockScreenRotation()
        {
            _prevAutoRotateToPortrait = Screen.autorotateToPortrait;
            _prevAutorotateToPortraitUpsideDown = Screen.autorotateToPortraitUpsideDown;

            Screen.autorotateToPortrait = false;
            Screen.autorotateToPortraitUpsideDown = false;
        }
        
        private void UnlockUnityUI()
        {
            if (SwUtils.System.IsRunningOnDevice())
            {
                Object.FindObjectOfType<EventSystem>().enabled = true;
            }

            Time.timeScale = _prevTimeScale;
        }
        
        private void RevertLockScreenRotation()
        {
            Screen.autorotateToPortrait = _prevAutoRotateToPortrait;
            Screen.autorotateToPortraitUpsideDown = _prevAutorotateToPortraitUpsideDown;
        }
        
        #endregion
    }
}