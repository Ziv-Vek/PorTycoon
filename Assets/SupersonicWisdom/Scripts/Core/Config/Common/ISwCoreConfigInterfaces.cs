using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    internal interface ISwConfigManagerState
    {
        EConfigListenerType Timing { get; }
        EConfigStatus Status { get; }
        SwWebRequestError WebRequestError { get; }
    }
    
    /// <summary>
    ///     Remote config repository
    ///     Responsible for fetching and persisting remote config and ab test data
    /// </summary>
    internal interface ISwConfigManager : ISwConfigManagerState
    {
        #region --- Public Methods ---

        /// <summary>
        ///     Add listener for finishing resolving remote config
        ///     The name "onLoaded" is used here because it's eventually
        ///     exposed via `SupersonicWisdom.Api.AddOnLoadedListener`
        /// </summary>
        /// <param name="onLoadedCallback"></param>
        void AddOnLoadedListener(OnLoaded onLoadedCallback);

        /// <summary>
        ///     Did finish process of fetching remote config (success/fail)
        /// </summary>
        /// <returns></returns>
        bool DidResolve { get; }

        /// <summary>
        ///     Fetch the remote config
        ///     After fetch OnLoaded listeners should be called
        ///     Fetch cannot be called before Init
        /// </summary>
        /// <returns></returns>
        IEnumerator Fetch ();

        /// <summary>
        ///     Init the repository
        ///     This method must be called before Fetch to restore any persisted remote config
        /// </summary>
        /// <param name="swLocalConfigProviders"></param>
        void Init(ISwLocalConfigProvider[] swLocalConfigProviders);
        void RemoveOnLoadedListener(OnLoaded onLoadedCallback);
        SwCoreConfig Config { get; }

        #endregion
    }

    internal interface ISwCoreInternalConfig : ISwConfigAccessor
    {
        public Dictionary<string, object> DynamicConfig { get; }
        Dictionary<string, object> AsDictionary();
        SwAbConfig Ab  { get; }
    }

    /// <summary>
    ///     Config values accessor by keys
    ///     It gets the accessible dictionary and the ab config and determine which value to return per key
    /// </summary>
    [PublicAPI]
    public interface ISwConfigAccessor
    {
        #region --- Public Methods ---

        int GetValue(string key, int defaultVal);
        
        /// <summary>
        /// Retrieves a value from a dynamic configuration based on the provided keys.
        /// If no matching key is found, the default value is returned.
        /// </summary>
        /// <param name="defaultVal">The default value to return if no matching key is found.</param>
        /// <param name="key">The keys used to retrieve a value from the dynamic configuration. The keys are processed in the order they are provided.</param>
        /// <returns>The value associated with the first matching key found in the dynamic configuration, or the default value if no matching key is found.</returns>
        int GetValue(int defaultVal, params string[] key);
        
        bool GetValue(string key, bool defaultVal);
        
        /// <summary>
        /// Retrieves a value from a dynamic configuration based on the provided keys.
        /// If no matching key is found, the default value is returned.
        /// </summary>
        /// <param name="defaultVal">The default value to return if no matching key is found.</param>
        /// <param name="key">The keys used to retrieve a value from the dynamic configuration. The keys are processed in the order they are provided.</param>
        /// <returns>The value associated with the first matching key found in the dynamic configuration, or the default value if no matching key is found.</returns>
        bool GetValue(bool defaultVal, params string[] key);
        
        float GetValue(string key, float defaultVal);
        string GetValue(string key, string defaultVal);
        
        /// <summary>
        ///     Does key exist in config
        /// </summary>
        /// <param name="key">Key to lookup</param>
        /// <returns></returns>
        bool HasConfigKey(string key);

        #endregion
    }
}