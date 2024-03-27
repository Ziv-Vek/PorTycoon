using System;

namespace SupersonicWisdomSDK
{
    [Serializable]
    internal struct SwEventDetailsExtra
    {
        #region --- Members ---

        public string connection;
        public string country;
        public string dpi;
        public string lang;
        public string resolutionHeight;
        public string resolutionWidth;

        #endregion
    }

    internal static class SwEventDetailsExtraConstants
    {
        #region --- Constants ---

        public const string KEY_CONNECTION = "connection";
        public const string KEY_COUNTRY = "country";
        public const string KEY_DPI = "dpi";
        public const string KEY_LANG = "lang";
        public const string KEY_RES_HEIGHT = "resolutionHeight";
        public const string KEY_RES_WIDTH = "resolutionWidth";

        #endregion
    }
}