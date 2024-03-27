using System.Collections.Generic;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwPlatformCommunication
    {
        #region --- Public Methods ---

        public static Dictionary<string, string> CreateAuthorizationHeadersDictionary()
        {
            return SupersonicWisdomSDK.SwPlatformCommunication.CreateAuthorizationHeadersDictionary(SwAccountUtils.AccountToken);
        }

        #endregion


        #region --- Inner Classes ---

        internal static class URLs
        {
            #region --- Constants ---

            internal const string LOGIN = SupersonicWisdomSDK.SwPlatformCommunication.URLs.BASE_PARTNERS + "login";
            
            private const string BASE_WISDOM = SupersonicWisdomSDK.SwPlatformCommunication.URLs.BASE + "wisdom/";
            internal const string TITLES = BASE_WISDOM + "titles";
            internal const string CURRENT_STAGE_API = BASE_WISDOM + "current-stage";
            internal const string DOWNLOAD_WISDOM_PACKAGE = BASE_WISDOM + "download-package";
            internal const string WISDOM_PACKAGE_MANIFEST = BASE_WISDOM + "package-manifest";

            #endregion
        }

        #endregion
    }
}