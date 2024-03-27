using System.Linq;
using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
using UnityEngine.iOS;
#endif

namespace SupersonicWisdomSDK
{
    internal class SwLangAndCountryUtils
    {
        #region --- Members ---

        private readonly SystemLanguage[] RtlLanguages =
        {
            SystemLanguage.Arabic, SystemLanguage.Hebrew,
        };
        
        #endregion


        #region --- Public Methods ---

        internal string GetCountry()
        {
#if UNITY_EDITOR
            return "";
#elif UNITY_IOS
            return _swGetCountry();
#elif UNITY_ANDROID
            var locale = new AndroidJavaClass("java.util.Locale");
            var defautLocale = locale.CallStatic<AndroidJavaObject>("getDefault");
            var country = defautLocale.Call<string>("getCountry");
            return country;
#else
            return "";
#endif
        }
        
        public bool IsRtlLanguage(SystemLanguage lang)
        {
            return RtlLanguages.ToList().Contains(lang);
        }
        
        public string GetSystemLanguageIso6391()
        {
            var lang = Application.systemLanguage;
            var code = "en";

            switch (lang)
            {
                case SystemLanguage.Afrikaans:
                    code = "af";

                    break;
                case SystemLanguage.Arabic:
                    code = "ar";

                    break;
                case SystemLanguage.Basque:
                    code = "eu";

                    break;
                case SystemLanguage.Belarusian:
                    code = "by";

                    break;
                case SystemLanguage.Bulgarian:
                    code = "bg";

                    break;
                case SystemLanguage.Catalan:
                    code = "ca";

                    break;
                case SystemLanguage.Chinese:
                    code = "zh";

                    break;
                case SystemLanguage.ChineseSimplified:
                    code = "zh";

                    break;
                case SystemLanguage.ChineseTraditional:
                    code = "zh";

                    break;
                case SystemLanguage.Czech:
                    code = "cs";

                    break;
                case SystemLanguage.Danish:
                    code = "da";

                    break;
                case SystemLanguage.Dutch:
                    code = "nl";

                    break;
                case SystemLanguage.English:
                    code = "en";

                    break;
                case SystemLanguage.Estonian:
                    code = "et";

                    break;
                case SystemLanguage.Faroese:
                    code = "fo";

                    break;
                case SystemLanguage.Finnish:
                    code = "fi";

                    break;
                case SystemLanguage.French:
                    code = "fr";

                    break;
                case SystemLanguage.German:
                    code = "de";

                    break;
                case SystemLanguage.Greek:
                    code = "el";

                    break;
                case SystemLanguage.Hebrew:
                    code = "iw";

                    break;
                case SystemLanguage.Hungarian:
                    code = "hu";

                    break;
                case SystemLanguage.Icelandic:
                    code = "is";

                    break;
                case SystemLanguage.Indonesian:
                    code = "in";

                    break;
                case SystemLanguage.Italian:
                    code = "it";

                    break;
                case SystemLanguage.Japanese:
                    code = "ja";

                    break;
                case SystemLanguage.Korean:
                    code = "ko";

                    break;
                case SystemLanguage.Latvian:
                    code = "lv";

                    break;
                case SystemLanguage.Lithuanian:
                    code = "lt";

                    break;
                case SystemLanguage.Norwegian:
                    code = "no";

                    break;
                case SystemLanguage.Polish:
                    code = "pl";

                    break;
                case SystemLanguage.Portuguese:
                    code = "pt";

                    break;
                case SystemLanguage.Romanian:
                    code = "ro";

                    break;
                case SystemLanguage.Russian:
                    code = "ru";

                    break;
                case SystemLanguage.SerboCroatian:
                    code = "sh";

                    break;
                case SystemLanguage.Slovak:
                    code = "sk";

                    break;
                case SystemLanguage.Slovenian:
                    code = "sl";

                    break;
                case SystemLanguage.Spanish:
                    code = "es";

                    break;
                case SystemLanguage.Swedish:
                    code = "sv";

                    break;
                case SystemLanguage.Thai:
                    code = "th";

                    break;
                case SystemLanguage.Turkish:
                    code = "tr";

                    break;
                case SystemLanguage.Ukrainian:
                    code = "uk";

                    break;
                case SystemLanguage.Unknown:
                    code = "en";

                    break;
                case SystemLanguage.Vietnamese:
                    code = "vi";

                    break;
            }

            return code;
        }

        #endregion


        #region --- Private Methods ---

        #if UNITY_IOS && !UNITY_EDITOR
		[DllImport("__Internal")]
		private static extern string _swGetCountry();
        #endif

        #endregion
    }
}