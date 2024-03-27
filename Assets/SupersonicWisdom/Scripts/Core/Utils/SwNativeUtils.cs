using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwNativeUtils
    {
        #region --- Properties ---

        private readonly Lazy<bool> LazyIsIosSandbox = new Lazy<bool>(() =>
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            var isSandbox = false;
#if UNITY_IOS && !UNITY_EDITOR
            isSandbox = _swIsSandbox();
#endif
            SwInfra.Logger.Log(EWisdomLogType.Utils, $"SwSandboxDetector | IsIosSandbox | {isSandbox}");

            return isSandbox;
        });
        
        public bool IsIosSandbox
        {
            get { return LazyIsIosSandbox.Value; }
        }
        
        #endregion
        
        #region --- Public Methods ---
        
        public Resolution GetNativeResolution()
        {
            var res = new Resolution
            {
                width = Screen.currentResolution.width,
                height = Screen.currentResolution.height,
            };
#if UNITY_IOS && !UNITY_EDITOR
            res.width = _swGetNativeWidth();
            res.height = _swGetNativeHeight();
#elif UNITY_ANDROID && !UNITY_EDITOR
            var (androidNativeWidth, androidNativeHeight) = GetAndroidNativeResolution();
            if (androidNativeWidth != 0 && androidNativeHeight != 0)
            {
                res.width = androidNativeWidth;
                res.height = androidNativeHeight;
            }
#endif
            return res;
        }
        
        public Tuple<int, int> GetAndroidNativeResolution()
        {
            //TODO Add 2 fallbacks: (1) https://developer.android.com/reference/android/view/WindowMetrics#getBounds() or (2) https://developer.android.com/reference/android/view/Window#getDecorView()
            var unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");

            var width = 0;
            var height = 0;

            var windowManagerInstance = activityInstance.Call<AndroidJavaObject>("getWindowManager");

            try
            {
                var pointInstance = new AndroidJavaObject("android.graphics.Point");
                var displayInstance = windowManagerInstance.Call<AndroidJavaObject>("getDefaultDisplay");
                displayInstance.Call("getRealSize", pointInstance);
                width = pointInstance.Get<int>("x");
                height = pointInstance.Get<int>("y");

                SwInfra.Logger.Log(EWisdomLogType.Utils, $"Measured native resolution of {width}x{height}");
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Utils, "Cannot measure native resolution. Exception: " + e);
            }

            return new Tuple<int, int>(width, height);
        }

        #if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int _swGetNativeHeight();

        [DllImport("__Internal")]
        private static extern int _swGetNativeWidth();
        
        [DllImport("__Internal")]
        private static extern bool _swIsSandbox();
        #endif
        
        #endregion
    }
}