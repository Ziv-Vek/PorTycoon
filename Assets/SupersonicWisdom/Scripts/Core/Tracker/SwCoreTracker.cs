using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using UnityEngine.Networking;

namespace SupersonicWisdomSDK
{
    internal class SwCoreTracker : ISwTracker
    {
        #region --- Constants ---

        protected const string TRACKER_ENCRYPTION_KEY = "4hT9xkWzVJcFQyR6pL8rBbG5mN0sDfZq";
        protected const string TRACKER_ENCRYPTION_IV = "uW3jH8kL4vN9tB6w";

        private const string CUSTOM6 = "custom6";
        private const string CLIENT_CATEGORY_KEY = "clientCategory";
        internal const string PROGRESS_EVENT_TYPE = "Progress";
        private const string GAMEPLAY_TYPE_KEY = "gameplayType";
        private const string GAMEPLAY_PROGRESS_TYPE_KEY = "gameplayProgressType";
        private const string LEVEL_CUSTOM_STRING_KEY = CUSTOM6;
        private const string PREVIOUS_LEVEL_TYPE_KEY = "previousLevelType";
        private const string PREVIOUS_LEVEL_TYPE_NUMBER_KEY = "previousLevelTypeNumber";

        #endregion


        #region --- Members ---

        private static readonly Dictionary<NetworkReachability, string> ConnectionDictionary = new Dictionary<NetworkReachability, string>
        {
            [NetworkReachability.NotReachable] = "offline",
            [NetworkReachability.ReachableViaLocalAreaNetwork] = "wifi",
            [NetworkReachability.ReachableViaCarrierDataNetwork] = "carrier",
        };

        private readonly Dictionary<string, object> _gameProgressDictionary = new Dictionary<string, object>
        {
            { GAMEPLAY_TYPE_KEY, ESwGameplayType.Level },
            { GAMEPLAY_PROGRESS_TYPE_KEY, SwProgressEvent.LevelStarted },
            { LEVEL_CUSTOM_STRING_KEY, "" },
            { PREVIOUS_LEVEL_TYPE_KEY, ESwLevelType.Regular },
            { PREVIOUS_LEVEL_TYPE_NUMBER_KEY, 0 },
        };

        private readonly ISwWebRequestClient _webRequestClient;
        private readonly SwCoreNativeAdapter _wisdomCoreNativeAdapter;
        private readonly SwCoreUserData _coreUserData;
        private readonly SwTimerManager _timerManager;
        private readonly SwCoreDataBridge _dataBridge;
        private readonly List<ISwTrackerDataProvider> _trackerListeners;

        #endregion

        
        #region --- Properties ---

        public EConfigListenerType ListenerType
        {
            get { return EConfigListenerType.EndOfGame; }
        }

        #endregion


        #region --- Construction ---

        public SwCoreTracker(SwCoreNativeAdapter wisdomCoreNativeAdapter, SwCoreUserData coreUserData, ISwWebRequestClient webRequestClient, SwTimerManager timerManager, SwCoreDataBridge dataBridge)
        {
            _wisdomCoreNativeAdapter = wisdomCoreNativeAdapter;
            _coreUserData = coreUserData;
            _webRequestClient = webRequestClient;
            _timerManager = timerManager;
            _dataBridge = dataBridge;
            _trackerListeners = new List<ISwTrackerDataProvider>();
        }

        #endregion


        #region --- Public Methods ---

        public static SwJsonDictionary GenerateEventCustoms(params string[] customs)
        {
            var customParams = new SwJsonDictionary();

            for (var i = 0; i < customs.Length; i++)
            {
                customParams.Add("custom" + (i + 1), customs[i] ?? "");
            }

            return customParams;
        }

        public IEnumerator SendEvent(string url, object data)
        {
            SwInfra.Logger.Log(EWisdomLogType.Analytics, "endpoint | " + url);

            if (SwTestUtils.IsRunningTests)
            {
                yield break;
            }

            var response = new SwWebResponse();

            yield return _webRequestClient.Post(url, data, response, SwConstants.DEFAULT_REQUEST_TIMEOUT);
            SwInfra.Logger.Log(EWisdomLogType.Analytics, "sent");

            if (response.DidFail)
            {
                SwInfra.Logger.LogError(EWisdomLogType.Analytics, "Fail | " + $"code: {response.code} | error: {response.error} | " + $"Internet Reachability: {Application.internetReachability}");
            }
            else
            {
                SwInfra.Logger.Log(EWisdomLogType.Analytics, "Success");
            }
        }

        public void SendUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return;
            }

            SwInfra.CoroutineService.StartCoroutine(SendUrlCoroutine(url));
        }

        public void TrackEvent(string evt, params string[] customs)
        {
            SwInfra.Logger.Log(EWisdomLogType.Analytics, $"Event name: {evt}");
            TrackEventInternal(evt, customs);
        }

        public void TrackGameProgressEvent(Dictionary<string, object> customs = null)
        {
            TrackEventWithParams(PROGRESS_EVENT_TYPE, customs);
        }

        public void TrackInfraEvent(params string[] customs)
        {
            TrackInfraEvent(GenerateEventCustoms(customs));
        }

        public void TrackInfraEvent(Dictionary<string, object> customs)
        {
            TrackEventWithParams(ClientCategory.Infra.ToString(), customs);
        }

        public void TrackEventWithParams(string eventName, Dictionary<string, object> customs = null, ClientCategory? clientCategory = null)
        {
            customs ??= new SwJsonDictionary();
            var dataKeyValues = new SwJsonDictionary(_dataBridge.CustomAndExternalDataDictionary);
            var extraData = GetEventExtraDataFromListeners();
            
            if (clientCategory.HasValue)
            {
                customs.SwAddOrReplace(CLIENT_CATEGORY_KEY, clientCategory.Value.ToString());
            }
            
            customs.SwMerge(false, dataKeyValues);
            customs.SwMerge(true, extraData);

            var settings = new JsonSerializerSettings
            {
                Converters = new List<JsonConverter> { new StringEnumConverter() },
                Culture = CultureInfo.InvariantCulture,
            };
            
            var customsJson = JsonConvert.SerializeObject(customs, settings);
            var extraJson = JsonConvert.SerializeObject(GetEventDetailsExtra(), settings);

            _wisdomCoreNativeAdapter.TrackEvent(eventName, customsJson, extraJson);
        }

        public void TrackEventWithParams(string eventName, Dictionary<string, object> customsDictionary, ClientCategory? clientCategory = null, params string[] customs)
        {
            var eventCustoms = GenerateEventCustoms(customs);
            var dataKeyValues = SwJsonDictionary.Parse(_dataBridge.CustomAndExternalDataDictionary);
            var extraData = GetEventExtraDataFromListeners();
            
            if (clientCategory.HasValue)
            {
                eventCustoms.SwAddOrReplace(CLIENT_CATEGORY_KEY, clientCategory.Value.ToString());
            }

            customsDictionary.SwMerge(false, eventCustoms);
            customsDictionary.SwMerge(false, dataKeyValues);
            customsDictionary.SwMerge(true, extraData);
            
            var customsJson = customsDictionary.SwToJsonString();
            var extraJson = JsonUtility.ToJson(GetEventDetailsExtra());

            _wisdomCoreNativeAdapter.TrackEvent(eventName, customsJson, extraJson);
        }
        
        public void TrackEventWithParams(string eventName, Dictionary<string, object> customs, List<string> keysToEncrypt, ClientCategory? clientCategory = null)
        {
            var encryptedCustoms = new Dictionary<string, object>();

            foreach (var key in keysToEncrypt)
            {
                if (!customs.TryGetValue(key, value: out var custom)) continue;

                var toEncrypt = custom?.ToString().SwToString() ?? string.Empty;
                var encrypted = EncryptData(toEncrypt);
                encryptedCustoms[key] = encrypted;
            }
            
            foreach (var custom in customs.Where(custom => !encryptedCustoms.ContainsKey(custom.Key)))
            {
                encryptedCustoms[custom.Key] = custom.Value;
            }

            TrackEventWithParams(eventName, encryptedCustoms, clientCategory);
        }

        public void AddListeners(List<ISwTrackerDataProvider> listeners)
        {
            _trackerListeners.AddRange(listeners);
        }

        private SwJsonDictionary GetEventExtraDataFromListeners()
        {
            var extraData = new SwJsonDictionary();

            if (_trackerListeners == null) return extraData;

            foreach (var listener in _trackerListeners)
            {
                try
                {
                    var (listenerData, keysToEncrypt) = listener.AddExtraDataToTrackEvent();
                    
                    foreach (var key in keysToEncrypt)
                    {
                        if (listenerData.TryGetValue(key, out var value))
                        {
                            listenerData[key] = EncryptData(value.ToString());
                        }
                    }
                    
                    extraData.SwMerge(true, listenerData);
                }
                catch (Exception e)
                {
                    SwInfra.Logger.Log(EWisdomLogType.Analytics,
                        $"An error occured while trying to merge {listener} \n message: {e.Message}");
                }
            }

            return extraData;
        }
        
        internal static string EncryptCustom(string payload)
        {
            return EncryptData(payload);
        }

        #endregion


        #region --- Private Methods ---

        private static IEnumerator SendUrlCoroutine(string url)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                SwInfra.Logger.Log(EWisdomLogType.Analytics, "SendEvent | error | network not reachable");

                yield break;
            }

            using (var webRequest = UnityWebRequest.Get(url))
            {
                yield return webRequest.SendWebRequest();
                SwInfra.Logger.Log(EWisdomLogType.Analytics, $"Url: {url}");
                var code = webRequest.responseCode;

                if (code == 0 || code >= 400)
                {
                    SwInfra.Logger.LogError(EWisdomLogType.Analytics, $"Fail: {code}");
                }
                else
                {
                    SwInfra.Logger.Log(EWisdomLogType.Analytics, "Success");
                }
            }
        }

        protected SwEventDetailsExtra GetEventDetailsExtra()
        {
            var eventDetailsExtra = new SwEventDetailsExtra
            {
                lang = _coreUserData.Language,
                country = _coreUserData.Country,
            };

            // The following properties are relying Unity API.
            // Unity API can be accessed only via main thread
            if (SwUtils.System.IsRunningOnMainThread)
            {
                eventDetailsExtra.connection = ConnectionDictionary[Application.internetReachability];
                eventDetailsExtra.dpi = $"{Screen.dpi}";
                eventDetailsExtra.resolutionWidth = $"{Screen.currentResolution.width}";
                eventDetailsExtra.resolutionHeight = $"{Screen.currentResolution.height}";
            }

            return eventDetailsExtra;
        }

        protected internal void TrackEventInternal(string eventName, params string[] customs)
        {
            var eventCustoms = GenerateEventCustoms(customs);
            TrackEventWithParams(eventName, eventCustoms);
        }

        protected static string EncryptData(string payload)
        {
            return SwEncryptor.EncryptAesBase64(payload, TRACKER_ENCRYPTION_KEY, TRACKER_ENCRYPTION_IV);
        }

        #endregion


        #region --- Inner Classes ---

        internal static class SharedParams
        {
            #region --- Constants ---

            public const string ERROR = "error";
            public const string ERROR_CODE = "errorCode";
            public const string ERROR_SOURCE = "errorSource";
            public const string CUSTOM1 = "custom1";

            #endregion
        }

        #endregion
    }
}