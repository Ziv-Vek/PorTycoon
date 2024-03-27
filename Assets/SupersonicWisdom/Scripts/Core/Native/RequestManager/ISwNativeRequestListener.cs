using SupersonicWisdomSDK;

namespace SupersonicWisdomSDK
{
    internal interface ISwNativeRequestListener
    {
        #region --- Public Methods ---

        void OnIteration(SwWebResponse response);
        void OnFail(SwWebResponse response);
        void OnSuccess(SwWebResponse response);

        #endregion
    }
}