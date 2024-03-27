#if UNITY_IOS
using System.Runtime.InteropServices;
using UnityEngine.iOS;
#endif

namespace SupersonicWisdomSDK
{
    internal static class SwAttUtils
    {
        public static SwAttAuthorizationStatus GetStatus ()
        {
            var status = SwAttAuthorizationStatus.Unsupported;
#if UNITY_IOS && !UNITY_EDITOR
            status = (SwAttAuthorizationStatus) _swGetTrackingConsentValue();
#endif
            return status;
        }

        public static bool IsMandatory ()
        {
#if !UNITY_IOS || UNITY_EDITOR
            return false;
#elif UNITY_IOS
            return _swIsTrackingConsentMandatory();
#endif
        }

        public static bool IsRequired()
        {
            return GetStatus() == SwAttAuthorizationStatus.NotDetermined;
        }

#if UNITY_IOS
        [DllImport("__Internal")]
        extern private static int _swGetTrackingConsentValue();

        [DllImport("__Internal")]
        extern private static bool _swIsTrackingConsentMandatory();
#endif
    }
}