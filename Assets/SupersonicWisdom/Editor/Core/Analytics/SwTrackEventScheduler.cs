using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Timer = System.Timers.Timer;

namespace SupersonicWisdomSDK.Editor
{
    internal class SwTrackEventScheduler
    {
        #region --- Members ---

        private ConcurrentQueue<Dictionary<string, object>> _eventQueue = new ConcurrentQueue<Dictionary<string, object>>();
        private readonly ISwExternalAnalyticsService _swExternalAnalyticsService;
        private readonly Timer _processTimer = new Timer(PROCESS_INTERVAL_MINUTES * 60 * MILLISECONDS_IN_SECONDS);
        private readonly string _diskStorageFilePath = Path.Combine(Application.temporaryCachePath, DISK_STORAGE_FILE_NAME);

        #endregion

        
        #region --- Constants ---

        private const int MAX_RETRY_COUNT = 3;
        private const int PROCESS_INTERVAL_MINUTES = 2;
        private const int REQUEST_TRY_COOLDOWN_SECONDS = 2;
        private const int MAX_QUEUE_SIZE = 100;
        
        private const string SCHEDULER_EVENT_AES_KEY = "4FDD9713A2800A12A2AF40868CD2772D";
        private const string SCHEDULER_EVENT_AES_IV = "2354CF0AA102A683";

        private const string DISK_STORAGE_FILE_NAME = "eventQueue.txt";
        private const int MILLISECONDS_IN_SECONDS = 1000;
        
        private const string DATE_HAPPENED_KEY = "date_happened";
        private const string CLIENT_TIMESTAMP_KEY = "client_timestamp";

        #endregion

        
        #region --- Constructor ---

        public SwTrackEventScheduler(ISwExternalAnalyticsService swExternalAnalyticsService)
        {
            _swExternalAnalyticsService = swExternalAnalyticsService;
            _processTimer.Elapsed += async (sender, e) => await ProcessPendingEvents();
            _processTimer.Start();

            LoadEventsFromDisk();
        }

        #endregion

        
        #region --- Public Methods ---

        public async Task TryToTrackEvent(Dictionary<string, object> customEventParams)
        {
            await SerializeQueueToDisk(); // Added to make sure that the queue is saved to disk to avoid losing events in case of crash/recompilation

            var tryCount = 0;
            
            SwEditorLogger.Log($"{nameof(SwTrackEventScheduler)} | {nameof(TryToTrackEvent)} | Trying to track event: {customEventParams.SwToString()}");

            var clientTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture);
            customEventParams.SwAddOrReplace(CLIENT_TIMESTAMP_KEY, clientTimestamp);
            
            while (tryCount < MAX_RETRY_COUNT)
            {
                
                if (await _swExternalAnalyticsService.TrackEvent(customEventParams))
                {
                    // Operation successful
                    return;
                }
                
                tryCount++;
                await Task.Delay(REQUEST_TRY_COOLDOWN_SECONDS * MILLISECONDS_IN_SECONDS);
            }

            // Tracking failed, enqueue event and add timestamp
            if (!customEventParams.ContainsKey(DATE_HAPPENED_KEY) && _eventQueue.Count < MAX_QUEUE_SIZE)
            {
                customEventParams.Add(DATE_HAPPENED_KEY, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture));
            }  
            else if (_eventQueue.Count >= MAX_QUEUE_SIZE)
            {
                SwEditorLogger.Log($"{nameof(SwTrackEventScheduler)} | {nameof(TryToTrackEvent)} | Event queue is full, replacing previous event");
                
                if(!_eventQueue.TryDequeue(out _)) return;
            }
            
            _eventQueue.Enqueue(customEventParams);
            await SerializeQueueToDisk();
        }

        #endregion

        
        #region --- Private Methods ---

        private void LoadEventsFromDisk()
        {
            if (!File.Exists(_diskStorageFilePath))
            {
                SwEditorLogger.Log($"{nameof(SwTrackEventScheduler)} | {nameof(LoadEventsFromDisk)} | Did not find file at path: {_diskStorageFilePath}");
                return;
            }

            var fileContent = File.ReadAllText(_diskStorageFilePath);
            var decryptedContent = DecryptFile(fileContent);
            _eventQueue = JsonConvert.DeserializeObject<ConcurrentQueue<Dictionary<string, object>>>(decryptedContent);
            SwEditorLogger.Log($"{nameof(SwTrackEventScheduler)} | {nameof(LoadEventsFromDisk)} | Loaded events from disk");
        }

        private async Task SerializeQueueToDisk()
        {
            var serializedQueue = EncryptFile(JsonConvert.SerializeObject(_eventQueue));
            SwEditorLogger.Log($"{nameof(SwTrackEventScheduler)} | {nameof(SerializeQueueToDisk)} | Serialized queue to disk {_diskStorageFilePath}");
            await File.WriteAllTextAsync(_diskStorageFilePath, serializedQueue);
        }

        private async Task ProcessPendingEvents()
        {
            while (_eventQueue.Count > 0)
            {
                if (_eventQueue.TryPeek(out var eventParams))
                {
                    if (await _swExternalAnalyticsService.TrackEvent(eventParams))
                    {
                        if (_eventQueue.TryDequeue(out eventParams))
                        {
                            SwEditorLogger.Log($"{nameof(SwTrackEventScheduler)} | {nameof(ProcessPendingEvents)} | Successfully Tracked event");
                        }
                        else
                        {
                            SwEditorLogger.Log($"{nameof(SwTrackEventScheduler)} | {nameof(ProcessPendingEvents)} | Failed to dequeue event");
                            break;
                        }
                    }
                    else
                    {
                        SwEditorLogger.Log($"{nameof(SwTrackEventScheduler)} | {nameof(ProcessPendingEvents)} | Failed to track event");
                        break;
                    }
                }
                
                await SerializeQueueToDisk();
            }
        }
    
        private static string EncryptFile(string content)
        {
            return SwEncryptor.EncryptAesBase64(content, SCHEDULER_EVENT_AES_KEY, SCHEDULER_EVENT_AES_IV);
        }

        private static string DecryptFile(string content)
        {
            return SwEncryptor.DecryptAesBase64(content, SCHEDULER_EVENT_AES_KEY, SCHEDULER_EVENT_AES_IV);
        }

        #endregion
    }
}