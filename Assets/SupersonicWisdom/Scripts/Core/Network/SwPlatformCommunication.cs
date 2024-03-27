using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    public static class SwPlatformCommunication
    {
        #region --- Public Methods ---

        public static Dictionary<string, string> CreateAuthorizationHeadersDictionary(string Token)
        {
            return string.IsNullOrWhiteSpace(Token) ? new Dictionary<string, string>() : new Dictionary<string, string>
            {
                { "authorization", "Bearer " + Token },
            };
        }

        #endregion


        #region --- Inner Classes ---

        internal static class URLs
        {
            #region --- Constants ---

            internal const string BASE = "https://partners.super-api.supersonic.com/v1/";

            internal const string BASE_PARTNERS = BASE + "partners/";
            internal const string USERS_ME = BASE_PARTNERS + "users/me";

            #endregion
        }

        #endregion
    }
}