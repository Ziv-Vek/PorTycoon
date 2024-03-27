using System;
using System.Collections.Generic;
using SwJsonDictionary = System.Collections.Generic.Dictionary<string, object>;

namespace SupersonicWisdomSDK.Editor
{
    [Serializable]
    internal class SwSelfUpdateRawConfiguration
    {
        #region --- Members ---
        
        public List<string> sourceVersionsBlackList;
        public List<string> targetVersionsBlackList;
        public List<string> sourceVersionsPostInstallVerificationBlackList;
        public List<string> targetVersionsPostInstallVerificationBlackList;
        public SwJsonDictionary stageUpdate;
        public SwJsonDictionary stageAndVersionUpdate;
        public SwJsonDictionary versionUpdate;

        #endregion
    }
}