#if SW_STAGE_STAGE10_OR_ABOVE
using System;
using System.Collections.Generic;
using AppsFlyerSDK;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Scripting;

namespace SupersonicWisdomSDK
{
    public class SwAppsFlyerEventDispatcher : MonoBehaviour, IAppsFlyerConversionData
    {
        #region --- Members ---

        private readonly List<ISwAppsFlyerListener> _listeners = new List<ISwAppsFlyerListener>();

        #endregion


        #region --- Mono Override ---

        private void Awake ()
        {
            AppsFlyer.OnRequestResponse += OnAppsFlyerRequestResponse;
        }

        #endregion


        #region --- Public Methods ---

        public void AddListener(ISwAppsFlyerListener listener)
        {
            _listeners.Add(listener);
        }

        public void onAppOpenAttribution(string attributionData)
        {
            foreach (var l in _listeners)
            {
                l.onAppOpenAttribution(attributionData);
            }
        }

        public void onAppOpenAttributionFailure(string error)
        {
            foreach (var l in _listeners)
            {
                l.onAppOpenAttributionFailure(error);
            }
        }

        public void onConversionDataFail(string error)
        {
            foreach (var l in _listeners)
            {
                l.onConversionDataFail(error);
            }
        }

        public void onConversionDataSuccess(string conversionData)
        {
            foreach (var l in _listeners)
            {
                l.onConversionDataSuccess(conversionData);
            }
        }

        [Preserve]
        public void requestResponseReceived(string response)
        {
            var appsFlyerArgs = ParseRequestCallback(response);

            foreach (var l in _listeners)
            {
                l.OnAppsFlyerRequestResponse(null, appsFlyerArgs);
            }
        }

        #endregion


        #region --- Private Methods ---

        private static AppsFlyerRequestEventArgs ParseRequestCallback(string response)
        {
            var responseCode = 0;
            var errorDescription = "";

            try
            {
                var dictionary = AppsFlyer.CallbackStringToDictionary(response);
                var errorResponse = dictionary.ContainsKey("errorDescription") ? dictionary["errorDescription"] : "";
                errorDescription = (string)errorResponse;
                responseCode = (int)(long)dictionary["statusCode"];
            }
            catch (Exception e)
            {
                errorDescription = $"AppsFlyerRequestEventArgs Parse Error: {e.Message}";
            }

            return new AppsFlyerRequestEventArgs(responseCode, errorDescription);
        }

        #endregion


        #region --- Event Handler ---

        /// <summary>
        ///     Dummy callback
        ///     Bug in AppsFlyer design forces listening to OnRequestResponse in order for `requestResponseReceived` to be even
        ///     invoked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAppsFlyerRequestResponse(object sender, EventArgs e)
        { }

        #endregion
    }
}
#endif