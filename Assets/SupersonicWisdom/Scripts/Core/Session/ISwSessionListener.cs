namespace SupersonicWisdomSDK
{
    internal interface ISwSessionListener
    {
        #region --- Public Methods ---

        // For Android devices - OnSessionEnded will be invoked when app returns from the background
        void OnSessionEnded(string sessionId);
        void OnSessionStarted(string sessionId);

        #endregion
    }
}