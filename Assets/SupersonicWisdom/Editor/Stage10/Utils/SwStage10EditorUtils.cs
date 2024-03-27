#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameAnalyticsSDK;

namespace SupersonicWisdomSDK.Editor
{
    public class SwStage10EditorUtils : SwEditorUtils
    {
        #region --- Events ---

        internal delegate void DidChangeGameAnalyticsKeysDelegate ();

        internal static event DidChangeGameAnalyticsKeysDelegate OnGameAnalyticsKeysChangedEvent;

        #endregion


        static SwStage10EditorUtils()
        {
            SwAccountUtils.FetchTitlesCompletedEvent += OnTitlesFetched;
        }

        #region --- Public Methods ---

        public static Tuple<string, string> GetGameAnalyticsKeys(RuntimePlatform platform)
        {
            var platformIndex = GameAnalytics.SettingsGA.Platforms.IndexOf(platform);

            if (platformIndex < 0) return new Tuple<string, string>("", "");

            return new Tuple<string, string>(GameAnalytics.SettingsGA.GetGameKey(platformIndex), GameAnalytics.SettingsGA.GetSecretKey(platformIndex));
        }

        public static void SetGameAnalyticsKeys(RuntimePlatform platform, string gameKey, string secretKey)
        {
            var platformIndex = GameAnalytics.SettingsGA.Platforms.IndexOf(platform);

            if (platformIndex < 0)
            {
                GameAnalytics.SettingsGA.AddPlatform(platform);
                platformIndex = GameAnalytics.SettingsGA.Platforms.IndexOf(platform);
            }

            GameAnalytics.SettingsGA.UpdateGameKey(platformIndex, gameKey);
            GameAnalytics.SettingsGA.UpdateSecretKey(platformIndex, secretKey);

            DidSetGameAnalyticsKeys();
        }

        #endregion


        #region --- Private Methods ---

        private static void DidSetGameAnalyticsKeys ()
        {
            OnGameAnalyticsKeysChangedEvent?.Invoke();
        }

        internal static void SetupGameAnalytics()
        {
            var games = SwAccountUtils.TitlesList.SwSafelyGet(SwSettings.selectedGameIndex, null)?.games;

            if (games == null) return;
            
            var gamePlatformsDictionary = games.ToDictionary(e => e.platform);

            if (games.Count != gamePlatformsDictionary.Count) return;

            SetupGameAnalytics(gamePlatformsDictionary, SwSettings);
        }

        private static void SetupGameAnalytics(Dictionary<string, GamePlatform> gamePlatforms, SwSettings settings)
        {
            GamePlatform gamePlatform = null;

            gamePlatform = gamePlatforms.SwSafelyGet(SwEditorConstants.GamePlatformKey.Ios, null);

            if (gamePlatform != null)
            {
                SetGameAnalyticsKeys(RuntimePlatform.IPhonePlayer, gamePlatform.gameAnalyticsKey, gamePlatform.gameAnalyticsSecret);
            }
            else
            {
                SetGameAnalyticsKeys(RuntimePlatform.IPhonePlayer, string.Empty, string.Empty);
            }

            gamePlatform = gamePlatforms.SwSafelyGet(SwEditorConstants.GamePlatformKey.Android, null);

            if (gamePlatform != null)
            {
                SetGameAnalyticsKeys(RuntimePlatform.Android, gamePlatform.gameAnalyticsKey, gamePlatform.gameAnalyticsSecret);
            }
            else
            {
                SetGameAnalyticsKeys(RuntimePlatform.Android, string.Empty, string.Empty);
            }

            if (settings.iosChinaBuildEnabled)
            {
                // Before this methods was called, the SDK added / removed these two GA values in the `settings`, so we can rely on them now.
                SetGameAnalyticsKeys(RuntimePlatform.IPhonePlayer, settings.iosChinaGameAnalyticsGameKey, settings.iosChinaGameAnalyticsSecretKey);
            }
            else
            {
                // Before this methods was called, the SDK added / removed these two GA values in the `settings`, so we can rely on them now.
                SetGameAnalyticsKeys(RuntimePlatform.IPhonePlayer, settings.iosGameAnalyticsGameKey, settings.iosGameAnalyticsSecretKey);
            }
        }

        #endregion
        
        
        #region --- Event Handler ---

        private static void OnTitlesFetched()
        {
            SetupGameAnalytics();
            SwSelfUpdate.CheckForUpdates();
        }

        #endregion
    }
}

#endif