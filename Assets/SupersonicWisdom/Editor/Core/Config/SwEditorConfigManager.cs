using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    [InitializeOnLoad, DefaultExecutionOrder(-100)]
    internal static class SwEditorConfigManager
    {
        #region --- Constants ---
        
        private const int INTERVAL = 1; // Interval is set in hours
        private const string CONFIG_FILE_NAME = "editor-config.json";
        private const double SECONDS_IN_HOUR = 3600;
        
        #endregion
        
        
        #region --- Members ---

        private static double _lastUpdateTime;
        private static SwEditorConfigData _currentConfigData;
        private static readonly List<ISwEditorConfigListener> Listeners = new List<ISwEditorConfigListener>();
        private static readonly string ConfigURL = SwEditorConstants.AWS_ASSETS_URL + $"editor/config/{CONFIG_FILE_NAME}";
        private static readonly string ConfigFilePath = Path.Combine(Application.temporaryCachePath, CONFIG_FILE_NAME);
        
        #endregion
        
        
        #region --- Constructor ---

        static SwEditorConfigManager()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
            Task.Run(LoadConfigFromFile);
        }
        
        #endregion
        
        
        #region --- Public Methods ---
        
        public static void RegisterListener(ISwEditorConfigListener listener)
        {
            if (Listeners.Contains(listener)) return;

            Listeners.Add(listener);
            
            if (_currentConfigData != null)
            {
                listener.OnConfigUpdate(_currentConfigData);
            }
        }

        public static async Task<SwEditorConfigData> ManuallyFetchConfig()
        {
            if(_currentConfigData == null)
            {
                await FetchConfig();
            }
            
            return _currentConfigData;
        }
        
        
        #endregion
        
        
        #region --- Private Methods ---
    
        private static void Update()
        {
            // Method that checks if a certain interval of time has passed since the lastConfigUpdate.
            // If the interval has passed, the method updates the _lastUpdateTime and asynchronously fetches the configuration.
            if (!((EditorApplication.timeSinceStartup - _lastUpdateTime) / SECONDS_IN_HOUR >= INTERVAL)) return;

            _lastUpdateTime = EditorApplication.timeSinceStartup;
            Task.Run(FetchConfig);
        }

        private static async Task FetchConfig()
        {
            SwEditorConfigData configData = null;
            
            try
            {
                var (jsonString, error, httpResponseMessage) = await SwNetworkHelper.PerformRequest(ConfigURL, null, null);

                if (string.IsNullOrEmpty(jsonString) && error.IsValid)
                {
                    SwEditorLogger.LogError(error.ErrorMessage);
                    SwEditorTracker.TrackEditorEvent(nameof(FetchConfig), ESwEditorWisdomLogType.Config, ESwEventSeverity.Error, error.ErrorMessage);
                }
                else
                {
                    configData = JsonConvert.DeserializeObject<SwEditorConfigData>(jsonString ?? SwEditorConstants.EMPTY_STRINGIFIED_JSON);
                }
            }
            catch (Exception e)
            {
                SwEditorLogger.LogError(e);
                SwEditorTracker.TrackEditorEvent(nameof(FetchConfig), ESwEditorWisdomLogType.Config, ESwEventSeverity.Error, e.ToString());
            }
            
            _currentConfigData = configData;
            await File.WriteAllTextAsync(ConfigFilePath, JsonConvert.SerializeObject(_currentConfigData));
            
            NotifyListeners();
        }

        private static void NotifyListeners()
        {
            foreach (var listener in Listeners)
            {
                listener.OnConfigUpdate(_currentConfigData);
            }
        }
        
        private static async Task LoadConfigFromFile()
        {
            if (File.Exists(ConfigFilePath))
            {
                try
                {
                    var jsonString = await File.ReadAllTextAsync(ConfigFilePath);
            
                    if (!string.IsNullOrWhiteSpace(jsonString))
                    {
                        _currentConfigData = JsonConvert.DeserializeObject<SwEditorConfigData>(jsonString);
                    }
                }
                catch (Exception e)
                {
                    SwEditorLogger.LogError(e);
                    SwEditorTracker.TrackEditorEvent(nameof(LoadConfigFromFile), ESwEditorWisdomLogType.Config, ESwEventSeverity.Error, e.ToString());
                }
            }

            if (_currentConfigData == null)
            {
                await FetchConfig();
            }
        }

        #endregion
    }
} 
