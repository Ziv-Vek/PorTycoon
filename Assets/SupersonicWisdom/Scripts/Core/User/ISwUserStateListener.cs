namespace SupersonicWisdomSDK
{
    internal interface ISwUserStateListener
    {
        #region --- Public Methods ---

        void OnCoreUserStateChange(SwUserState newState, SwUserStateChangeReason reason);

        #endregion
    }
}