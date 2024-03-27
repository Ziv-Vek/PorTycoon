using System;
using System.Collections;

namespace SupersonicWisdomSDK
{
    public delegate void OnAttRequestFinish(SwAttAuthorizationStatus newStatus);

    internal interface ISwAttAdapter
    {
        #region --- Public Methods ---

        void OpenPrivacyTracking ();
        IEnumerator Request(Action onShow = null, OnAttRequestFinish onFinish = null);

        #endregion
    }
}