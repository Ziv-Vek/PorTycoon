using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace SupersonicWisdomSDK.Editor
{
    internal static class SwEditorTracker
    {
        #region --- Constants ---
        
        private const string HOST_NAME_WISDOM_EDITOR_ANALYTICS = "wisdom-editor-analytics";
        
        #endregion
        
        
        #region --- Members ---
        
        private static SwEditorAnalyticsConfig _editorConfig;
        private static readonly SwTrackEventScheduler TrackEventScheduler;
        private static readonly SwEditorTrackerConfigFetcher ConfigFetcher;
        
        #endregion

        
        #region --- Constructor ---
        
        static SwEditorTracker()
        {
            ConfigFetcher = new SwEditorTrackerConfigFetcher();
            TrackEventScheduler = new SwTrackEventScheduler(new SwDataDogTracker());
        }
        
        #endregion
        
        
        #region --- Public Methods ---
        
        public static void TrackEditorEvent(string eventName, ESwEditorWisdomLogType eventTag, ESwEventSeverity eventSeverity, string eventLog)
        {
            if (_editorConfig == null)
            {
                SwEditorLogger.Log($"{nameof(SwEditorTracker)} | {nameof(TrackEditorEvent)} | editorConfig is null, creating default");
                return;
            }
            
            if (!_editorConfig.ShouldTrackEditorEvents)
            {
                SwEditorLogger.Log($"{nameof(SwEditorTracker)} | {nameof(TrackEditorEvent)} | Unity version or SDK version are blacklisted");
                return;
            }
            
            var customEventData = CreateEventData(eventName, eventTag, eventSeverity, eventLog);
            
            #if SUPERSONIC_WISDOM_TEST
            Debug.Log("SupersonicWisdom: SwEditorTracker | TryToTrackEvent | " + customEventData.SwToString());  //Log to use in Unit test, do not remove
            #endif
            
            Task.Run(() => TrackEventScheduler.TryToTrackEvent(customEventData));        
        }
        
        #endregion
        
        
        #region --- Private Methods ---

        private static Dictionary<string, object> CreateEventData(string eventName, ESwEditorWisdomLogType eventTag, ESwEventSeverity eventSeverity, string eventLog)
        {
            var customEventData = new Dictionary<string, object>
            {
                { "event_log", eventLog },
            }.SwMerge(false, new SwEditorTrackerDefaultValues());
            
            var eventData = new Dictionary<string, object>
            {
                { "title", $"{eventName}" },
                { "text", JsonConvert.SerializeObject(customEventData) },
                { "tags", JsonConvert.SerializeObject(new List<string> { $"wisdom:{eventTag.ToString().ToLower()}" }) },
                { "alert_type", eventSeverity.ToString().ToLower() },
                { "host", HOST_NAME_WISDOM_EDITOR_ANALYTICS },
            };

            return eventData;
        }
        
        #endregion


        #region --- Internal Class ---

        private class SwEditorTrackerConfigFetcher : ISwEditorConfigListener
        {
            #region --- Constructor ---
            
            public SwEditorTrackerConfigFetcher()
            {
                SwEditorConfigManager.RegisterListener(this);
            }
            
            #endregion
            
            
            #region --- Public Methods ---
            
            public void OnConfigUpdate(SwEditorConfigData newConfigData)
            {
                _editorConfig = newConfigData.config?.analyticsConfig;
            }
            
            #endregion
        }

        #endregion
    }
}