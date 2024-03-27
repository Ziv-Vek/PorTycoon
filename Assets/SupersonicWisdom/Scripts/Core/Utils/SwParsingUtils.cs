using System;

namespace SupersonicWisdomSDK
{
    internal class SwParsingUtils
    {
        #region --- Public Methods ---

        internal int TryParseInt(string intString, int defaultValue = default)
        {
            if (string.IsNullOrEmpty(intString)) return defaultValue;

            try
            {
                int.TryParse(intString, out defaultValue);
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Utils, e);
            }

            return defaultValue;
        }
        
        internal bool? TryParseBool(string boolString, bool? defaultValue = null)
        {
            var result = defaultValue;
        
            if (string.IsNullOrEmpty(boolString)) return result;
        
            if (boolString == "1" || boolString.ToLower() == "true" || boolString.ToLower() == "yes")
            {
                result = true;
            }
        
            if (boolString == "0" || boolString.ToLower() == "false" || boolString.ToLower() == "no")
            {
                result = false;
            }
        
            return result;
        }

        #endregion
    }
}