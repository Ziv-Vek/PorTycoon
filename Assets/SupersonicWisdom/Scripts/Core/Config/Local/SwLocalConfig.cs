using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    internal abstract class SwLocalConfig
    {
        #region --- Properties ---

        public abstract Dictionary<string, object> LocalConfigValues { get; }

        #endregion
    }
}