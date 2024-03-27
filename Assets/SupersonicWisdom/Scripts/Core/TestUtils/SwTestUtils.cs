using System.Collections.Generic;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal static class SwTestUtils
    {
        #region --- Members ---

        public static int ForceStage = -1;

        #endregion


        #region --- Properties ---

        public static bool IsRunningAutomaticTests { get; set; }
        public static bool IsRunningTests { get; set; }

        // public static SwUserState _userState => Container.SwLegacyAdapter._userState;

        public static Dictionary<string, object> CustomUserState { get; set; }

        public static string CustomWebViewsConfigStorageKey { get; set; }

        #endregion
    }
}