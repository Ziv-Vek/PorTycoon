using UnityEngine;

namespace SupersonicWisdomSDK
{
    internal class SwSettingsManager<T> where T : class, ISwSettings
    {
        #region --- Members ---
        
        private readonly string _resourcePath;

        #endregion


        #region --- Properties ---

        public T Settings { get; private set; }

        #endregion


        #region --- Construction ---

        public SwSettingsManager(string resourcePath = SwConstants.SETTINGS_RESOURCE_PATH)
        {
            _resourcePath = resourcePath;
            Load(); 
        }

        #endregion


        #region --- Public Methods ---

        public void Load(T settings = null)
        {
            var resourceSettings = settings ?? Resources.Load(_resourcePath, typeof(T)) as T;

            if (resourceSettings == null && Application.isPlaying)
            {
                Debug.LogError("Missing SupersonicWisdom's Settings asset!\nTo create the settings asset, go to: Window > SupersonicWisdom > Edit Settings.\nQuitting the app!");
                Application.Quit();
            }
            else if (resourceSettings != null)
            {
                Settings = resourceSettings;
                Settings.Init();
                SwInfra.Logger.Setup(Settings.IsDebugEnabled);
            }
        }

        #endregion
    }
}