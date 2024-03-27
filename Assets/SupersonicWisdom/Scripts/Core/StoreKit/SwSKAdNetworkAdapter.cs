using System;
using UnityEngine;

#if UNITY_IOS
using System.Runtime.InteropServices;
using UnityEngine.iOS;
#endif

namespace SupersonicWisdomSDK
{
    internal static class SwSKAdNetworkAdapter
    {
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        extern private static void _swUpdatePostbackConversionValue(int conversionValue);
        
        [DllImport("__Internal")]
        extern private static void _swUpdatePostbackConversionAndCoarseValue(int conversionValue, int coarseConversionValue, bool lockWindow);

        [DllImport("__Internal")]
        extern private static int _swGetSkanVersion();

        private static Action<string> _callback;
#endif

        public static void UpdatePostbackConversionValue(int conversionValue = 0, Action<string> callback = null)
        {
#if !UNITY_IOS || UNITY_EDITOR
            throw new Exception($"UpdatePostbackConversionValue({conversionValue}) Unsupported system {Application.platform}");
#elif UNITY_IOS
            _callback = callback;
            _swUpdatePostbackConversionValue(conversionValue);
            SwInfra.Logger.Log(EWisdomLogType.StoreKit, $"SKAdNetwork | UpdatePostbackConversionValue | {conversionValue}");
#endif
        }

        public static void UpdatePostbackConversionAndCoarseValue(int conversionValue = 0, int coarseConversionValue = 0, bool lockWindow = false, Action<string> callback = null)
        {
#if !UNITY_IOS
            throw new Exception($"UpdatePostbackConversionAndCoarseValue( cv = {conversionValue} coarseConversionValue = {coarseConversionValue} lockWindow = {lockWindow}) Unsupported system {Application.platform}");
#else
            SwInfra.Logger.Log(EWisdomLogType.Ad, $"cv = {conversionValue} coarseConversionValue = {coarseConversionValue} lockWindow = {lockWindow}");
#if UNITY_EDITOR
            callback?.Invoke("editor-test");
#else
            _callback = callback;
            _swUpdatePostbackConversionAndCoarseValue(conversionValue, coarseConversionValue, lockWindow);
            SwInfra.Logger.Log(EWisdomLogType.StoreKit, $"SKAdNetwork | {nameof(UpdatePostbackConversionAndCoarseValue)} | cv = {conversionValue} coarseConversionValue = {coarseConversionValue} lockWindow = {lockWindow}");
#endif
#endif
        }

        public static void OnUpdatePostbackUpdateCompleted(string authorizationStatusString)
        {
#if !UNITY_IOS || UNITY_EDITOR
            throw new Exception($"OnUpdatePostbackUpdateCompleted({authorizationStatusString}) Unsupported system {Application.platform}");
#elif UNITY_IOS
            SwInfra.Logger.Log(EWisdomLogType.StoreKit, $"SKAdNetwork | UpdatePostbackConversionValue | Callback | {authorizationStatusString}");
            _callback?.Invoke(authorizationStatusString);
            _callback = null;
#endif
        }
        
        public static int GetSkanVersion()
        {
#if !UNITY_IOS 
            throw new Exception($"GetSkanVersion() Unsupported system {Application.platform}");
#else
            SwInfra.Logger.Log(EWisdomLogType.Ad, $"");
#if UNITY_EDITOR
            return -1;
#else
            return _swGetSkanVersion();
#endif
#endif
        }
    }
}