namespace SupersonicWisdomSDK
{
    public interface ISwAttListener
    {
        #region --- Public Methods ---

        void OnAttAuthorizationStatusChanged(SwAttAuthorizationStatus status);
        void OnAttShow ();

        #endregion
    }
}