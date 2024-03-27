using System;
using System.Collections.Generic;

namespace SupersonicWisdomSDK.Editor
{
    [Serializable]
    public class GamePlatform
    {
        #region --- Members ---

        public string adMobId;
        public string bundleId;
        public string facebookAppId;
        public string facebookToken;
        public string gameAnalyticsKey = "";
        public string gameAnalyticsSecret = "";
        public string id;
        public string isAppKey;
        public string os;
        public string platform;
        public string storeId;

        #endregion


        #region --- Public Methods ---

        public override string ToString ()
        {
            return $"platform: {platform}, store ID: {storeId}, game ID: {id}";
        }

        #endregion
    }

    [Serializable]
    public class TitleDetails
    {
        #region --- Members ---

        public List<GamePlatform> games;
        public string id;
        public string name;
        public string unityProjectId;

        #endregion
    }

    [Serializable]
    public class TitlesResponse
    {
        #region --- Members ---

        public List<TitleDetails> result;

        #endregion
    }

    [Serializable]
    public class AccountLogin
    {
        #region --- Members ---

        public string message;
        public string token;

        #endregion
    }
}