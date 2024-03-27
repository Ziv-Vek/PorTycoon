using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    internal interface ISwWebRequestClient
    {
        #region --- Public Methods ---

        /// <summary>
        ///     Perform HTTP GET Request
        /// </summary>
        /// <param name="url">HTTP request URL</param>
        /// <param name="response">Response object to store the outcome of the http request</param>
        /// <param name="timeout">Request Timout in seconds</param>
        /// <param name="headers">Request Headers</param>
        /// <param name="logResponseText">
        ///     Whether to log response text
        ///     For example, video file should not be logged as text. A simple json should be logged.
        ///     In any case, logging will be occur only if it's globally enabled via SwSettings.enableDebug
        /// </param>
        /// <returns></returns>
        IEnumerator Get(string url, [NotNull] SwWebResponse response, int timeout = 0, Dictionary<string, string> headers = null, bool logResponseText = false);

        /// <summary>
        ///     Perform HTTP POST Request
        /// </summary>
        /// <param name="url">HTTP request URL</param>
        /// <param name="data">Request Body</param>
        /// <param name="response">Response object to store the outcome of the http request</param>
        /// <param name="timeout">Request Timout in seconds</param>
        /// <param name="headers">Request Headers</param>
        /// <param name="logResponseText">
        ///     Whether to log response text
        ///     For example, video file should not be logged as text. A simple json should be logged.
        ///     In any case, logging will be occur only if it's globally enabled via SwSettings.enableDebug
        /// </param>
        /// <returns></returns>
        IEnumerator Post(string url, object data, [NotNull] SwWebResponse response, int timeout = 0, Dictionary<string, string> headers = null, bool logResponseText = false);

        #endregion
    }
}