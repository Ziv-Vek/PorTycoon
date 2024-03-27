using System;
using JetBrains.Annotations;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal delegate void OnIOSNativeDialogClose(string buttonIndex);

    internal delegate void OnIOSNativeMessageClose ();

    internal delegate void OnIOSTrackingAuthorizationChange(SwAttAuthorizationStatus authorizationStatus);

    internal delegate void OnAppStoreCloseMessage(bool error);

    internal class SwIosNativeMessagesService : MonoBehaviour
    {
        #region --- Events ---

        public event OnAppStoreCloseMessage OnAppStoreCloseMessageEvent;
        public event OnIOSNativeDialogClose OnIOSNativeDialogCloseEvent;
        public event OnIOSNativeMessageClose OnIOSNativeMessageCloseEvent;
        public event OnIOSTrackingAuthorizationChange OnIOSTrackingAuthorizationChangeEvent;

        #endregion


        #region --- Public Methods ---

        public void AppStoreCloseNativeMessage(string message)
        {
            var error = !string.IsNullOrEmpty(message);
            OnAppStoreCloseMessageEvent?.Invoke(error);
        }

        public void IOSNativeDialogCloseMessage(string buttonIndex)
        {
            OnIOSNativeDialogCloseEvent?.Invoke(buttonIndex);
        }

        public void IOSNativeMessageCloseMessage ()
        {
            OnIOSNativeMessageCloseEvent?.Invoke();
        }

        public void IOSTrackingAuthorizationChangeMessage(string authorizationStatusString)
        {
            int authorizationStatusInt = Convert.ToInt16(authorizationStatusString);
            var authorizationStatus = (SwAttAuthorizationStatus)authorizationStatusInt;
            OnIOSTrackingAuthorizationChangeEvent?.Invoke(authorizationStatus);
        }

        public void UpdatePostbackUpdateCompletedMessage(string authorizationStatusString)
        {
            SwSKAdNetworkAdapter.OnUpdatePostbackUpdateCompleted(authorizationStatusString);
        }
        
        #endregion
    }
}