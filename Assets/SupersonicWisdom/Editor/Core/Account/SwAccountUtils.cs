using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwAccountUtils
    {
        #region --- Constants ---

        private const string ANDROID_APP_ID_FIELD_NAME = "GoogleMobileAdsAndroidAppId";
        private const string IOS_APP_ID_FIELD_NAME = "GoogleMobileAdsIOSAppId";
        private const string DUMMY_SELECT_TITLE_NAME = "--- select ---";
        private const string ANDROID = "android";
        private const string IOS = "ios";

        #endregion


        #region --- Events ---

        public static event Action FetchTitlesCompletedEvent;

        #endregion


        #region --- Members ---

        private static bool _isUpdatingGamesList;

        #endregion


        #region --- Properties ---

        public static bool AreIosChinaSettingsValid
        {
            get { return !string.IsNullOrEmpty(Settings.iosChinaAppId) && !string.IsNullOrEmpty(Settings.iosChinaGameId) && !string.IsNullOrEmpty(Settings.iosChinaGameAnalyticsGameKey) && !string.IsNullOrEmpty(Settings.iosChinaGameAnalyticsSecretKey) && !string.IsNullOrEmpty(AdMobIosChinaAppId) && !string.IsNullOrEmpty(Settings.ironSourceIosChinaAppKey); }
        }

        public static bool IsLoggedIn
        {
            get { return !string.IsNullOrEmpty(AccountToken); }
        }

        public static bool IsSelectedTitleDummy
        {
            get { return string.IsNullOrEmpty(SelectedTitle?.id); }
        }

        public static bool WasLoggedIn
        {
            get { return Settings.wasLoggedIn; }
        }

        public static List<TitleDetails> TitlesList
        {
            get { return SwEditorUtils.SwAccountData.TitleDetailsList; }
            private set { SwEditorUtils.SwAccountData.TitleDetailsList = value; }
        }

        public static string AdMobAndroidAppId
        {
            get
            {
                if (!Settings.adMobAndroidAppId.SwIsNullOrEmpty())
                {
                    var appId = Settings.adMobAndroidAppId;
                    Settings.adMobAndroidAppId = null;
                    AdMobAndroidAppId = appId;

                    return appId;
                }

                return SwReflectionUtils.GetProperty(ANDROID_APP_ID_FIELD_NAME, string.Empty, GoogleMobileAdsSettingsInstance);
            }

            set
            {
                if (SwReflectionUtils.SetProperty(ANDROID_APP_ID_FIELD_NAME, value, GoogleMobileAdsSettingsInstance))
                {
                    EditorUtility.SetDirty(GoogleMobileAdsSettingsInstance);
                }
                else
                {
                    SwInfra.Logger.Log(EWisdomLogType.Privacy, $"Failed to set GoogleMobileAdsSetting ({ANDROID_APP_ID_FIELD_NAME}) for Android.");
                }
            }
        }

        public static string AdMobIosAppId
        {
            get
            {
                if (!Settings.adMobIosAppId.SwIsNullOrEmpty())
                {
                    var appId = Settings.adMobIosAppId;
                    Settings.adMobIosAppId = null;
                    AdMobIosAppId = appId;

                    return appId;
                }

                if (Settings.iosChinaBuildEnabled) return string.Empty;

                return SwReflectionUtils.GetProperty(IOS_APP_ID_FIELD_NAME, string.Empty, GoogleMobileAdsSettingsInstance);
            }

            set
            {
                if (Settings.iosChinaBuildEnabled) return;

                if (SwReflectionUtils.SetProperty(IOS_APP_ID_FIELD_NAME, value, GoogleMobileAdsSettingsInstance))
                {
                    EditorUtility.SetDirty(GoogleMobileAdsSettingsInstance);
                }
                else
                {
                    SwInfra.Logger.Log(EWisdomLogType.Privacy, $"Failed to set GoogleMobileAdsSetting ({IOS_APP_ID_FIELD_NAME}) for iOS.");
                }
            }
        }

        public static string AdMobIosChinaAppId
        {
            get
            {
                if (!Settings.iosChinaBuildEnabled) return string.Empty;

                if (!Settings.adMobIosChinaAppId.SwIsNullOrEmpty())
                {
                    var appId = Settings.adMobIosChinaAppId;
                    Settings.adMobIosChinaAppId = null;
                    AdMobIosChinaAppId = appId;

                    return appId;
                }

                return SwReflectionUtils.GetProperty(IOS_APP_ID_FIELD_NAME, string.Empty, GoogleMobileAdsSettingsInstance);
            }

            set
            {
                if (!Settings.iosChinaBuildEnabled) return;

                if (SwReflectionUtils.SetProperty(IOS_APP_ID_FIELD_NAME, value, GoogleMobileAdsSettingsInstance))
                {
                    EditorUtility.SetDirty(GoogleMobileAdsSettingsInstance);
                }
                else
                {
                    SwInfra.Logger.Log(EWisdomLogType.Privacy, $"Failed to set GoogleMobileAdsSetting ({IOS_APP_ID_FIELD_NAME}) for iOS China build.");
                }
            }
        }

        public static string AccountToken { get; private set; }

        private static bool IsTitleListEmpty
        {
            get { return TitlesList?.Count == 0; }
        }

        private static Dictionary<string, GamePlatform> SelectedGameDictionary
        {
            get { return SelectedGame.ToDictionary(e => e.platform); }
        }

        private static List<GamePlatform> SelectedGame
        {
            get { return SelectedTitle?.games; }
        }

        private static Object GoogleMobileAdsSettingsInstance
        {
            get { return SwGoogleMobileAdsAsset.GoogleMobileAdsSettingsInstance; }
        }

        private static SwSettings Settings
        {
            get { return SwEditorUtils.SwSettings; }
        }

        private static TitleDetails SelectedTitle
        {
            get { return Settings == null ? null : TitlesList?.SwSafelyGet(Settings.selectedGameIndex, null); }
        }

        #endregion


        #region --- Public Methods ---

        public static void FetchTitles()
        {
            if (!IsLoggedIn || _isUpdatingGamesList)
            {
                return;
            }

            _isUpdatingGamesList = true;
            TitlesList = new List<TitleDetails>();

            SwNetworkHelper.PerformRequest(SwPlatformCommunication.URLs.TITLES, null, SwPlatformCommunication.CreateAuthorizationHeadersDictionary(), (value, error, httpResponseMessage) =>
            {
                if (error.IsValid)
                {
                    SwEditorLogger.LogError(error.ErrorMessage);
                    SwEditorTracker.TrackEditorEvent(nameof(FetchTitles), ESwEditorWisdomLogType.Account, ESwEventSeverity.Error, error.ErrorMessage);

                    if (IsLoggedIn && error.ResponseCode == 401 && (error.ErrorMessage?.ToLower().Contains("unauthorized") ?? false))
                    {
                        SetAccountToken(null);
                        SwEditorAlerts.AlertLoginExpired();
                    }
                    else
                    {
                        SwEditorAlerts.AlertError(error.ErrorMessage, error.ResponseCode, SwEditorConstants.UI.ButtonTitle.CANCEL);
                    }

                    return;
                }

                var jsonString = SwEditorConstants.EMPTY_STRINGIFIED_JSON;

                if (value != null)
                {
                    jsonString = value;
                }

                var titlesResponse = JsonUtility.FromJson<TitlesResponse>(jsonString);

                TitlesList = titlesResponse.result;
                _isUpdatingGamesList = false;
                Settings.selectedGameIndex = 0;
                SaveSelectedTitleToSettings();
                FetchTitlesCompletedEvent?.Invoke();
                SwEditorTracker.TrackEditorEvent(nameof(FetchTitles), ESwEditorWisdomLogType.Account, ESwEventSeverity.Info, "Titles fetched successful");
            });
        }

        public static void GoToLoginTab()
        {
            SwCoreSettingsInspector.SelectedTabIndex = 0;
            SwEditorUtils.OpenSettings();
        }

        public static void Login(string email, string password)
        {
            SwNetworkHelper.PerformRequest(SwPlatformCommunication.URLs.LOGIN, new Dictionary<string, object>
            {
                { "email", email },
                { "password", password },
            }, null, (responseString, error, httpResponseMessage) =>
            {
                var loginObject = JsonUtility.FromJson<AccountLogin>(responseString);
                SetAccountToken(loginObject?.token ?? "");
                string errorMessage = null;

                if (error.IsValid)
                {
                    errorMessage = error.ErrorMessage;
                }
                else if (!error.IsValid && !IsLoggedIn)
                {
                    errorMessage = "Unknown error";
                }

                if (!IsLoggedIn)
                {
                    SwEditorTracker.TrackEditorEvent(nameof(Login), ESwEditorWisdomLogType.Account, ESwEventSeverity.Error, errorMessage);
                    SwEditorAlerts.AlertError(errorMessage, (int)SwErrors.ESettings.LoginEndpoint, SwEditorConstants.UI.ButtonTitle.CLOSE);
                }
                else
                {
                    Settings.wasLoggedIn = true;
                    FetchTitles();
                    SwEditorTracker.TrackEditorEvent(nameof(Login), ESwEditorWisdomLogType.Account, ESwEventSeverity.Info, "Login successful");
                }
            });
        }

        public static void Logout(SwSettings settings)
        {
            settings.selectedGameIndex = 0;
            SetAccountToken(string.Empty);
            TitlesList.Clear();
            _isUpdatingGamesList = false;
            SwEditorTracker.TrackEditorEvent(nameof(Logout), ESwEditorWisdomLogType.Account, ESwEventSeverity.Info, "Logout successful");
        }

        public static void SaveSelectedTitleGamesToSettings()
        {
            Settings.unityProjectId = SelectedTitle.unityProjectId;
            
            // Select the facebook app id from the first platform where available 
            var gamePlatformWithFacebookAppId = SelectedGameDictionary.Values.FirstOr(gamePlatform => !string.IsNullOrWhiteSpace(gamePlatform.facebookAppId), null);
            SwEditorUtils.FacebookAppId = gamePlatformWithFacebookAppId?.facebookAppId ?? "";

            if (SwEditorUtils.FacebookClientToken.SwIsNullOrEmpty())
            {
                var gamePlatformWithFacebookClientToken = SelectedGameDictionary.Values.FirstOr(gamePlatform => !string.IsNullOrWhiteSpace(gamePlatform.facebookToken), null);
                SwEditorUtils.FacebookClientToken = gamePlatformWithFacebookClientToken?.facebookToken ?? "";
            }

            GamePlatform gamePlatform = null;
            gamePlatform = SelectedGameDictionary.SwSafelyGet(SwEditorConstants.GamePlatformKey.Android, null);

            if (gamePlatform != null)
            {
                Settings.androidGameId = gamePlatform.id.SwRemoveSpaces();
                AdMobAndroidAppId = gamePlatform.adMobId.SwRemoveSpaces();
                Settings.ironSourceAndroidAppKey = gamePlatform.isAppKey.SwRemoveSpaces();
                Settings.androidGameAnalyticsGameKey = gamePlatform.gameAnalyticsKey;
                Settings.androidGameAnalyticsSecretKey = gamePlatform.gameAnalyticsSecret;
            }
            else
            {
                Settings.androidGameId = AdMobAndroidAppId = Settings.ironSourceAndroidAppKey = string.Empty;
            }

            gamePlatform = SelectedGameDictionary.SwSafelyGet(SwEditorConstants.GamePlatformKey.Ios, null);

            if (gamePlatform != null)
            {
                Settings.iosGameId = gamePlatform.id.SwRemoveSpaces();
                Settings.iosAppId = gamePlatform.storeId.Replace("id", "").SwRemoveSpaces();
                Settings.iosGameAnalyticsGameKey = gamePlatform.gameAnalyticsKey.SwRemoveSpaces();
                Settings.iosGameAnalyticsSecretKey = gamePlatform.gameAnalyticsSecret.SwRemoveSpaces();
                AdMobIosAppId = gamePlatform.adMobId.SwRemoveSpaces();
                Settings.ironSourceIosAppKey = gamePlatform.isAppKey.SwRemoveSpaces();
            }
            else
            {
                Settings.iosGameId = Settings.iosAppId = AdMobIosAppId = Settings.ironSourceIosAppKey = Settings.iosGameAnalyticsGameKey = Settings.iosGameAnalyticsSecretKey = string.Empty;
            }

            gamePlatform = SelectedGameDictionary.SwSafelyGet(SwEditorConstants.GamePlatformKey.IosChina, null);

            if (gamePlatform != null)
            {
                Settings.iosChinaGameId = gamePlatform.id.SwRemoveSpaces();
                Settings.iosChinaAppId = gamePlatform.storeId.Replace("id", "").SwRemoveSpaces();
                AdMobIosChinaAppId = gamePlatform.adMobId.SwRemoveSpaces();
                Settings.ironSourceIosChinaAppKey = gamePlatform.isAppKey.SwRemoveSpaces();
                Settings.iosChinaGameAnalyticsGameKey = gamePlatform.gameAnalyticsKey.SwRemoveSpaces();
                Settings.iosChinaGameAnalyticsSecretKey = gamePlatform.gameAnalyticsSecret.SwRemoveSpaces();
            }
            else
            {
                Settings.iosChinaGameId = Settings.iosChinaAppId = AdMobIosChinaAppId = Settings.ironSourceIosChinaAppKey = Settings.iosChinaGameAnalyticsGameKey = Settings.iosChinaGameAnalyticsSecretKey = string.Empty;
            }
        }

        public static void TryToRestoreLoginToken()
        {
            if (IsLoggedIn)
            {
                return;
            }

            var restoredToken = EditorPrefs.GetString(SwEditorConstants.SwKeys.TEMP_AUTH_TOKEN, null);

            if (string.IsNullOrEmpty(restoredToken))
            {
                return;
            }

            SetAccountToken(restoredToken);

            if (IsLoggedIn)
            {
                EditorPrefs.DeleteKey(SwEditorConstants.SwKeys.TEMP_AUTH_TOKEN);
            }
        }

        #endregion


        #region --- Private Methods ---

        private static void AddDummyTitle(this List<TitleDetails> titlesList)
        {
            titlesList.Insert(0, new TitleDetails { name = DUMMY_SELECT_TITLE_NAME });
        }

        private static bool IsSettingsAndTitleAreTheSame(TitleDetails titleDetails, SwSettings settings)
        {
            if (titleDetails?.games == null || titleDetails.games.Count == 0)
            {
                return false;
            }

            foreach (var gamePlatform in titleDetails.games)
            {
                // Check if the facebook app id or client token are different from the settings and the game platform has a valid token (this is to cover the case where the user has a valid token but the game platform doesn't)
                if ((SwEditorUtils.FacebookAppId != gamePlatform.facebookAppId || SwEditorUtils.FacebookClientToken != gamePlatform.facebookToken) && !string.IsNullOrEmpty(gamePlatform.facebookToken))
                {
                    return false;
                }

                switch (gamePlatform.os)
                {
                    case "ios":
                    {
                        var notEqual = settings.iosGameId != gamePlatform.id.SwRemoveSpaces() || settings.iosAppId != gamePlatform.storeId.Replace("id", "").SwRemoveSpaces() || settings.iosGameAnalyticsGameKey != gamePlatform.gameAnalyticsKey.SwRemoveSpaces() || settings.iosGameAnalyticsSecretKey != gamePlatform.gameAnalyticsSecret.SwRemoveSpaces() || AdMobIosAppId != gamePlatform.adMobId.SwRemoveSpaces() || settings.ironSourceIosAppKey != gamePlatform.isAppKey.SwRemoveSpaces();

                        if (notEqual)
                        {
                            return false;
                        }

                        break;
                    }
                    case "iosChina":
                    {
                        var notEqual = settings.iosChinaGameId != gamePlatform.id.SwRemoveSpaces() || settings.iosChinaAppId != gamePlatform.storeId.Replace("id", "").SwRemoveSpaces() || AdMobIosChinaAppId != gamePlatform.adMobId.SwRemoveSpaces() || settings.ironSourceIosChinaAppKey != gamePlatform.isAppKey.SwRemoveSpaces() || settings.iosChinaGameAnalyticsGameKey != gamePlatform.gameAnalyticsKey.SwRemoveSpaces() || settings.iosChinaGameAnalyticsSecretKey != gamePlatform.gameAnalyticsSecret.SwRemoveSpaces();

                        if (notEqual)
                        {
                            return false;
                        }

                        break;
                    }
                    case "android":
                    {
                        var notEqual = settings.androidGameId != gamePlatform.id.SwRemoveSpaces() || settings.androidGameAnalyticsGameKey != gamePlatform.gameAnalyticsKey.SwRemoveSpaces() || settings.androidGameAnalyticsSecretKey != gamePlatform.gameAnalyticsSecret.SwRemoveSpaces() || AdMobAndroidAppId != gamePlatform.adMobId.SwRemoveSpaces() || settings.ironSourceAndroidAppKey != gamePlatform.isAppKey.SwRemoveSpaces();

                        if (notEqual)
                        {
                            return false;
                        }

                        break;
                    }
                }
            }

            return true;
        }

        private static bool IsTitleFoundAutomatically(List<TitleDetails> titles)
        {
            var index = SearchTitleInProjectSettings(titles);
            Settings.isTitleSelectedAutomatically = index > 0;
            Settings.selectedGameIndex = index;

            return Settings.isTitleSelectedAutomatically;
        }

        private static bool IsValidTitle(TitleDetails titleDetails)
        {
            return !string.IsNullOrEmpty(titleDetails.id);
        }

        private static void SaveSelectedTitleToSettings()
        {
            if (IsTitleListEmpty)
            {
                SwEditorAlerts.AlertMissingTitles();

                return;
            }

            TitlesList.AddDummyTitle();

            if (!IsTitleFoundAutomatically(TitlesList))
            {
                InternalEditorUtility.RepaintAllViews();
                SwEditorAlerts.AlertTitleNotFound(TitlesList);

                return;
            }

            if (IsSettingsAndTitleAreTheSame(SelectedTitle, Settings))
            {
                InternalEditorUtility.RepaintAllViews();
                SwEditorAlerts.AlertConfigurationIsUpToDate();

                return;
            }

            var gamePlatformsDictionary = SelectedGameDictionary;

            if (SelectedGame.Count != gamePlatformsDictionary.Count)
            {
                SwEditorAlerts.AlertDuplicatePlatform(SelectedGame);
            }
            else
            {
                SaveSelectedTitleGamesToSettings();
                SwEditorAlerts.AlertSuccess(SelectedTitle?.name);
            }
        }

        private static int SearchTitleInProjectSettings(List<TitleDetails> titles)
        {
            if (titles == null)
            {
                return 0;
            }

            var androidApplicationPackageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            var iosApplicationBundleIdentifier = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);

            var platformToBundle = new Dictionary<string, string>
            {
                { ANDROID, androidApplicationPackageName },
                { IOS, iosApplicationBundleIdentifier },
            };

            var first = SwSystemUtils.IsAndroidTarget() ? ANDROID : IOS;
            var second = SwSystemUtils.IsIosTarget() ? ANDROID : IOS;
            var titleIndex = TryGetIndexOfGame(titles, first, platformToBundle[first]);

            if (titleIndex == 0)
            {
                titleIndex = TryGetIndexOfGame(titles, second, platformToBundle[second]);
            }

            return titleIndex;
        }

        private static int TryGetIndexOfGame(List<TitleDetails> titles, string platformKey, string applicationBundleIdentifier)
        {
            for (var titleIndex = 1; titleIndex < titles.Count; titleIndex++)
            {
                var titleDetails = titles[titleIndex];

                if (!IsValidTitle(titleDetails))
                {
                    continue;
                }

                if (titleDetails.games.Where(platformGame => platformGame.os.ToLower().Equals(platformKey)).Any(platformGame => platformGame.bundleId == applicationBundleIdentifier))
                {
                    return titleIndex;
                }
            }

            return 0;
        }

        private static void SetAccountToken(string jwtToOverride)
        {
            AccountToken = jwtToOverride;

            if (string.IsNullOrEmpty(jwtToOverride))
            {
                EditorPrefs.DeleteKey(SwEditorConstants.SwKeys.TEMP_AUTH_TOKEN);
            }
        }

        #endregion
    }
}