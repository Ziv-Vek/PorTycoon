namespace SupersonicWisdomSDK
{
    public delegate void OnBlockingLoaderVisibilityChanged(bool isVisible);

    internal interface ISwLoader
    {
        #region --- Events ---

        event OnBlockingLoaderVisibilityChanged OnBlockingLoaderVisibilityChangedEvent;

        #endregion


        #region --- Properties ---

        bool IsVisible { get; }

        #endregion


        #region --- Public Methods ---

        bool Hide ();
        bool Show ();

        #endregion
    }
}