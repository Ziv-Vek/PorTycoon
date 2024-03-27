using System.Collections.Generic;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    public interface ISwSettings
    {
        #region --- Public Methods ---

        string GetAppKey ();

        string GetGameId ();

        void Init();

        bool IsDebugEnabled ();

        bool IsPrivacyPolicyEnabled ();

        void OverwritePartially(IReadOnlyDictionary<string, object> dict, [NotNull] ISwKeyValueStore keyValueStore);

        #endregion
    }
}