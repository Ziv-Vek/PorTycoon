using System;

namespace SupersonicWisdomSDK
{
    internal class SwLoader : ISwLoader
    {
        #region --- Events ---

        public event OnBlockingLoaderVisibilityChanged OnBlockingLoaderVisibilityChangedEvent;

        #endregion


        #region --- Members ---

        private readonly SwCoreNativeAdapter _coreNativeAdapter;
        private bool _isVisible;

        #endregion


        #region --- Properties ---

        public bool IsVisible
        {
            get { return _isVisible; }
            private set
            {
                if (value == _isVisible) return;

                _isVisible = value;
                NotifyListeners(_isVisible);
            }
        }

        #endregion


        #region --- Construction ---

        public SwLoader(SwCoreNativeAdapter wisdomCoreNativeAdapter)
        {
            _coreNativeAdapter = wisdomCoreNativeAdapter;
        }

        #endregion


        #region --- Public Methods ---

        public bool Hide()
        {
            if (!_coreNativeAdapter.ToggleBlockingLoader(false)) return false;

            IsVisible = false;

            return true;
        }

        public bool Show()
        {
            if (!_coreNativeAdapter.ToggleBlockingLoader(true)) return false;

            IsVisible = true;

            return true;
        }

        #endregion


        #region --- Private Methods ---

        private void NotifyListeners(bool isVisible)
        {
            var delegates = OnBlockingLoaderVisibilityChangedEvent?.GetInvocationList();

            delegates?.SwForEach(listener =>
            {
                try
                {
                    (listener as OnBlockingLoaderVisibilityChanged)?.Invoke(isVisible);
                }
                catch (Exception e)
                {
                    SwInfra.Logger.LogError(EWisdomLogType.BlockingLoader, $"Caught exception during listeners invocation: {nameof(OnBlockingLoaderVisibilityChangedEvent)}?.Invoke({isVisible});. Exception: {e}");
                }
            });
        }

        #endregion
    }
}