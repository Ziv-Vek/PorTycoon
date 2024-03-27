using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwDeepLinkHandler
    {
        #region --- Constants ---

        private const string SETTINGS_PREFIX = "settings.";
        
        #endregion


        #region --- Members ---

        protected internal readonly Dictionary<string, string> DeepLinkParams = new Dictionary<string, string>();
        private readonly ISwSettings Settings;
        private readonly ISwWebRequestClient WebRequestClient;
        private readonly List<ISwDeepLinkListener> _deepLinkListeners;

        #endregion


        #region --- Properties ---

        protected internal Dictionary<string, string> DeepLinkParamsClone { get; private set; } = new Dictionary<string, string>();

        #endregion


        #region --- Construction ---

        public SwDeepLinkHandler(ISwSettings settings, ISwWebRequestClient webRequestClient)
        {
            Settings = settings;
            WebRequestClient = webRequestClient;
            _deepLinkListeners = new List<ISwDeepLinkListener>();
        }

        #endregion

        
        #region --- Public Methods ---

        public IEnumerator SetupDeepLink()
        {
            Application.deepLinkActivated += OnDeepLinkActivated;
            
            // Handle deep link when app is opened via deep link
            if (!string.IsNullOrEmpty(Application.absoluteURL))
            {
                yield return ApplyDeepLinkByUrl(Application.absoluteURL);
            }
        }

        public void AddListeners(IEnumerable<ISwDeepLinkListener> deepLinkListeners)
        {
            _deepLinkListeners.AddRange(deepLinkListeners);
        }

        #endregion


        #region --- Private Methods ---

        internal static string GetDeepLinkScheme(string gameId)
        {
            return $"sw.{gameId.ToLower()}";
        }

        /// <summary>
        ///     Extract parameters from wisdom deep link url
        /// </summary>
        /// <param name="url"></param>
        protected internal IEnumerator ApplyDeepLinkByUrl(string url)
        {
            SwInfra.Logger.Log(EWisdomLogType.DeepLink, $"OnDeepLinkActivated | {url}");
            var uri = new Uri(url);

            if (uri.Scheme.Equals(GetDeepLinkScheme(Settings.GetGameId())))
            {
                var paramsDictionary = new Dictionary<string, string>();
                var urlParts = uri.PathAndQuery.Split('?');

                if (urlParts.Length != 2) yield break;
                var paramAndValueItems = urlParts[1].Split('&');

                foreach (var paramAndValue in paramAndValueItems)
                {
                    var keyValue = paramAndValue.Split('=');
                    var key = "";
                    var value = "true";

                    if (keyValue.Length > 1)
                    {
                        key = keyValue[0];
                        value = Uri.UnescapeDataString(keyValue[1]);
                    }
                    else
                    {
                        key = paramAndValue;
                    }

                    value = paramsDictionary[key] = value;
                    SwInfra.Logger.Log(EWisdomLogType.DeepLink, $"Deep Link Param | {key}={value}");
                }

                yield return VerifyDeepLinkParams(paramsDictionary);

                if (paramsDictionary.Any())
                {
                    DeepLinkParams.SwMerge(true, paramsDictionary);
                    DeepLinkParamsClone = new Dictionary<string, string>(DeepLinkParams);
                }

                yield return OnDeepLinkParamsResolve();
            }
        }

        private IEnumerator NotifyListeners()
        {
            if (_deepLinkListeners == null) yield break;

            foreach (var deepLinkListener in _deepLinkListeners)
            {
                yield return deepLinkListener.OnDeepLinkResolved(DeepLinkParams);
            }
        }

        protected virtual bool DoesKeyRequireAdminVerification(string key)
        {
            return key.StartsWith(SETTINGS_PREFIX) || key.Equals(SwConfigConstants.SwDictionaryKey);
        }

        protected virtual IEnumerator OnDeepLinkParamsResolve ()
        {
            yield return NotifyListeners();

            HandleSettingsOverwriteParams();
        }

        private void HandleSettingsOverwriteParams()
        {
            // pass only pairs where the key start with "settings." (and remove this prefix)
            Settings.OverwritePartially(DeepLinkParams.Where(pair => pair.Key.StartsWith(SETTINGS_PREFIX)).ToDictionary(k => k.Key.Replace(SETTINGS_PREFIX, ""), k => (object)k.Value), SwInfra.KeyValueStore);
        }

        private IEnumerator VerifyDeepLinkParams(Dictionary<string, string> deepLinkParams)
        {
            if (!deepLinkParams.Any() || Debug.isDebugBuild)
            {
                yield break;
            }

            var isAdminVerificationRequired = deepLinkParams.Keys.Where(DoesKeyRequireAdminVerification).Any();
            var isVerified = !isAdminVerificationRequired;
            
            yield return TryIsAdminUser(deepLinkParams, isAdminVerificationRequired, isVerifiedResult => { isVerified = isVerifiedResult; });

            if (isVerified)
            {
                SwInfra.Logger.Log(EWisdomLogType.DeepLink, SwJsonParser.Serialize(deepLinkParams));
            }
            else
            {
                SwInfra.Logger.LogError(EWisdomLogType.DeepLink,  SwJsonParser.Serialize(deepLinkParams));
                deepLinkParams.Clear();
            }
        }

        private IEnumerator TryIsAdminUser(Dictionary<string, string> deepLinkParams, bool isAdminVerificationRequired, Action<bool> isVerifiedResult)
        {
            if (!isAdminVerificationRequired) yield break;
            
            var response = new SwWebResponse();
            var authorizationHeaderValue = deepLinkParams.SwSafelyGet("authorization", "");

            if (!string.IsNullOrEmpty(authorizationHeaderValue))
            {
                var headers = SwPlatformCommunication.CreateAuthorizationHeadersDictionary(authorizationHeaderValue.Replace("Bearer ", ""));

                yield return WebRequestClient.Get(SwPlatformCommunication.URLs.USERS_ME, response, 0, headers);

                if (response.DidSucceed)
                {
                    try
                    {
                        var platformUser = JsonUtility.FromJson<SwPlatformUser>(response.Text);

                        if (platformUser == null)
                        {
                            throw new SwException("platformUser == null");
                        }

                        isVerifiedResult(platformUser.role.isAdmin);
                    }
                    catch (Exception e)
                    {
                        SwInfra.Logger.LogError(EWisdomLogType.DeepLink, $"Cannot deserialize SwPlatformUser from response={response.Text} | error={e.Message}");
                    }
                }
            }
        }

        #endregion


        #region --- Event Handler ---

        private void OnDeepLinkActivated(string url)
        {
            SwInfra.CoroutineService.StartCoroutine(ApplyDeepLinkByUrl(url));
        }

        #endregion


        #region --- Inner Classes ---

        [Serializable]
        private class SwPlatformUser
        {
            #region --- Members ---

            public SwPlatformUserRole role;

            #endregion
        }

        [Serializable]
        private class SwPlatformUserRole
        {
            #region --- Members ---

            public bool isAdmin;

            #endregion
        }

        #endregion
    }

    internal class SwDeepLinkConstants
    {
        #region --- Constants ---

        internal const string DEEP_LINK_HOST = "supersonic.com";

        #endregion
    }
}