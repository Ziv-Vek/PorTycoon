namespace SupersonicWisdomSDK
{
    public class SupersonicWisdom
    {
        #region --- Members ---

        private static SwApi _apiInstance;

        #endregion


        #region --- Properties ---

        public static SwApi Api
        {
            get
            {
                if (_apiInstance == null)
                {
                    _apiInstance = new SwApi();
                }

                return _apiInstance;
            }
        }

        #endregion


        #region --- Private Methods ---

        internal static void Teardown ()
        {
            _apiInstance?.Teardown();
            _apiInstance = null;
        }

        #endregion
    }
}