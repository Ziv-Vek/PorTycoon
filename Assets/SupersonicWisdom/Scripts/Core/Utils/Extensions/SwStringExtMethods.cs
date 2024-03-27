using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal static class SwStringExtMethods
    {
        #region --- Public Methods ---

        /// <summary>
        ///     This utility method helps to use split a string with string as token (instead of char / chars array) in Unity 2020.
        /// </summary>
        /// <param name="self">The String instance to split</param>
        /// <param name="token">The part that will be extracted while all other parts will be in the result components array</param>
        /// <returns></returns>
        internal static string[] SwSplit(this string self, string token)
        {
            if (string.IsNullOrEmpty(token)) return new[] { self };

#if UNITY_2021_3_OR_NEWER
                return self.Split(token);
#else
            var copy = self.Clone() as string ?? "";
            var components = new List<string>();

            int tokenIndex;

            do
            {
                tokenIndex = copy.IndexOf(token, StringComparison.Ordinal);

                if (tokenIndex < 0) continue;

                var component = copy.Substring(0, tokenIndex);
                components.Add(component);
                copy = copy.Remove(0, tokenIndex + token.Length);
            }
            while (tokenIndex >= 0);

            components.Add(copy);

            return components.ToArray();
#endif
        }

        internal static Color ExtractColorFromHex(this string colorHex, Color defaultColor)
        {
            if (colorHex == null || colorHex.Equals(string.Empty)) return defaultColor;

            return ColorUtility.TryParseHtmlString(colorHex, out var color) ? color : defaultColor;
        }

        internal static bool SwIsValidEmailAddress(this string self)
        {
            var emailAddressRegex = new Regex(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$");

            return emailAddressRegex.IsMatch(self);
        }

        internal static string SwRemoveSpaces(this string self)
        {
            return self?.Replace(" ", "");
        }

        internal static bool SwIsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        internal static Dictionary<string, object> SwToJsonDictionary(this string self)
        {
            return SwJsonParser.DeserializeToDictionary(self);
        }
        
        public static string Format(this string format, params object[] list)
        {
            return string.Format(format, list);
        }

  #endregion
    }
}