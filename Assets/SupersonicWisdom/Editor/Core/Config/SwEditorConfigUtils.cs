using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwEditorConfigUtils
    {
        #region --- Public Methods ---
        
        public static bool IsVersionBlacklisted(string version, IReadOnlyCollection<string> blacklist)
        {
            // This method allows a version check with wildcard support. For examples: 7.5.* will match all version starting with 7.5 (7.5.1, 7.5.12)
            return blacklist != null && blacklist.Select(blacklistedVersion => "^" + Regex.Escape(blacklistedVersion).Replace("\\*", ".*") + "$").Any(pattern => Regex.IsMatch(version, pattern));
        }
        
        #endregion
    }
}