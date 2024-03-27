using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
using UnityEngine.iOS;
#endif

namespace SupersonicWisdomSDK
{
    internal class SwAttAdapter : ISwAttAdapter
    {
        private readonly ISwAttListener[] _listeners;
        public event OnAttRequestFinish OnFinishEvent;

        public SwAttAdapter([CanBeNull] ISwAttListener[] listeners)
        {
            _listeners = listeners;
        }

        public IEnumerator Request(Action onShow = null, OnAttRequestFinish onFinish = null)
        {
            if (SwAttUtils.IsRequired() && SwAttUtils.IsMandatory())
            {
                onShow?.Invoke();
                OpenAttDialog();

                while (SwAttUtils.GetStatus() == SwAttAuthorizationStatus.NotDetermined)
                {
                    yield return new WaitForSeconds(0.2f);
                }
            }

            var newStatus = SwAttUtils.GetStatus();

            if (_listeners != null)
            {
                foreach (var listener in _listeners)
                {
                    listener.OnAttAuthorizationStatusChanged(newStatus);
                }
            }

            onFinish?.Invoke(newStatus);
            OnFinishEvent?.Invoke(newStatus);
        }

        public void OpenPrivacyTracking ()
        {
#if !UNITY_IOS || UNITY_EDITOR
            throw new Exception($"SwAttAdapter | OpenPrivacyTracking | Unsupported system {Application.platform}");
#elif UNITY_IOS
            _swOpenSettingsPage();
#endif
        }

        private void OpenAttDialog ()
        {
#if !UNITY_IOS || UNITY_EDITOR
            throw new Exception($"SwAttAdapter | OpenAttDialog | Unsupported system {Application.platform}");
#elif UNITY_IOS
            _swShowTrackingConsentDialog();
#endif
        }

#if UNITY_IOS
                [DllImport("__Internal")]
                extern private static void _swShowTrackingConsentDialog();

                [DllImport("__Internal")]
                extern private static void _swOpenSettingsPage();
#endif
    }
}