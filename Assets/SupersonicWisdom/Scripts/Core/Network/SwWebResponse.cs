using System;
using JetBrains.Annotations;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    [Serializable]
    public class SwWebResponse
    {
        #region --- Members ---
        
        /// <summary>
        ///     Request unique key (guid)
        /// </summary>
        public string key;
        
        /// <summary>
        ///     Did response resolved (both success/fail)
        /// </summary>
        public bool isDone;

        /// <summary>
        ///     Is response pending
        /// </summary>
        public bool isPending;

        /// <summary>
        ///     Response raw data
        /// </summary>
        [CanBeNull]
        public byte[] data;

        /// <summary>
        ///     Response HTTP Status code
        /// </summary>
        public long code;

        public long iteration;
        public long cap;
        public long time;
        public bool isReachedCap;

        public SwWebRequestError error;
        public string dataString;

        #endregion
        

        #region --- Properties ---

        public bool DidFail
        {
            get { return isDone && error != SwWebRequestError.None; }
        }

        public bool DidSucceed
        {
            get { return isDone && error == SwWebRequestError.None; }
        }

        /// <summary>
        ///     Text is computed from the response data
        ///     Computed only once
        /// </summary>
        [CanBeNull]
        public string Text
        {
            get
            {
                if(string.IsNullOrEmpty(dataString))
                {
                    dataString = data != null ? System.Text.Encoding.UTF8.GetString(data, 0, data.Length) : null;
                }

                return dataString;
            }
        }

        public SwWebResponse() { }
        
#if UNITY_EDITOR
        public SwWebResponse(string dataString, bool isDone = true, SwWebRequestError error = SwWebRequestError.None, int code = 200)
        {
            this.dataString = dataString;
            this.isDone = isDone;
            this.error = error;
            this.code = code;
        }
#endif

        #endregion
    }
}