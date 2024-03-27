using System;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;

namespace SupersonicWisdomSDK
{
    public class SwAndroidDefaultPrefsStore : ISwKeyValueStore
    {
        #region --- Members ---
        
        private readonly AndroidJavaObject _defaultSharedPreferences = SwNativeAndroidBridge.GetDefaultSharedPreferences();
        private AndroidJavaObject _lazySharedPreferencesEditor;
        
        #endregion
        
        
        #region --- Properties ---

        private AndroidJavaObject SharedPreferencesEditor
        {
            get { return _lazySharedPreferencesEditor ??= _defaultSharedPreferences.Call<AndroidJavaObject>("edit"); }
        }
        
        #endregion
        
        
        #region --- Public Methods ---

        public void DeleteAll()
        {
            SwInfra.Logger.Log(EWisdomLogType.Cache, "DeleteAll");
            SharedPreferencesEditor.Call<AndroidJavaObject>("clear").Call<bool>("commit");
        }

        public ISwKeyValueStore DeleteKey(string key, bool isInternal = true)
        {
            if (key.SwIsNullOrEmpty()) return this;

            SwInfra.Logger.Log(EWisdomLogType.Cache, $"Key: {key}");
            SharedPreferencesEditor.Call<AndroidJavaObject>("remove", key).Call<bool>("commit");

            return this;
        }

        public bool GetBoolean(string key, bool defaultValue = false, bool isInternal = true)
        {
            if (key.SwIsNullOrEmpty()) return defaultValue;

            var value = GetInt(key, defaultValue ? 1 : 0) == 1;
            SwInfra.Logger.Log(EWisdomLogType.KeyValueStore, $"{nameof(SwAndroidDefaultPrefsStore)} | GetBoolean | {key} | {value}");

            return value;
        }

        public float GetFloat(string key, float defaultValue = 0f, bool isInternal = true)
        {
            if (key.SwIsNullOrEmpty()) return defaultValue;

            var value = _defaultSharedPreferences.Call<float>("getFloat", key, defaultValue);
            SwInfra.Logger.Log(EWisdomLogType.KeyValueStore, $"{nameof(SwAndroidDefaultPrefsStore)} | GetFloat | {key} | {value}");

            return value;
        }

        public int GetInt(string key, int defaultValue = 0, bool isInternal = true)
        {
            if (key.SwIsNullOrEmpty()) return defaultValue;

            var value = _defaultSharedPreferences.Call<int>("getInt", key, defaultValue);
            SwInfra.Logger.Log(EWisdomLogType.KeyValueStore, $"{nameof(SwAndroidDefaultPrefsStore)} | GetInt | {key} | {value}");

            return value;
        }

        public string GetString(string key, string defaultValue = "", bool isInternal = true)
        {
            if (key.SwIsNullOrEmpty()) return defaultValue;

            var value = _defaultSharedPreferences.Call<string>("getString", key, defaultValue);
            SwInfra.Logger.Log(EWisdomLogType.KeyValueStore, $"{nameof(SwAndroidDefaultPrefsStore)} | GetString | {key} | {value ?? "Null"}");

            return value;
        }

        public T GetGenericSerializedData<T>(string key, T defaultValue, bool isInternal = true)
        {
            SwInfra.Logger.LogWarning(EWisdomLogType.KeyValueStore, $"{nameof(SwAndroidDefaultPrefsStore)} | GetGenericSerializedData | {key} | GenericSerialization is not supported in AndroidPrefs");

            return defaultValue;
        }

        public bool HasKey(string key, bool isInternal = true)
        {
            return SharedPreferencesEditor.Call<bool>("contains", key);
        }

        public DateTime GetDate(string key, DateTime defaultValue)
        {
            var dateInString = GetString(key, string.Empty);

            try
            {
                return JsonConvert.DeserializeObject<DateTime>(dateInString, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto,
                });
            }
            catch (Exception e)
            {
                SwInfra.Logger.LogError(EWisdomLogType.KeyValueStore, e);

                return defaultValue;
            }
        }
        
        public void Save()
        {
            SharedPreferencesEditor.Call<bool>("commit");
        }

        public ISwKeyValueStore SetBoolean(string key, bool value, bool isInternal = true, bool save = false)
        {
            if (key.SwIsNullOrEmpty()) return this;

            SwInfra.Logger.Log(EWisdomLogType.KeyValueStore, $"Set | {key} : {value}");
            SetInt(key, value ? 1 : 0, isInternal, save);

            return this;
        }

        public ISwKeyValueStore SetFloat(string key, float value, bool isInternal = true, bool save = false)
        {
            if (key.SwIsNullOrEmpty()) return this;

            SwInfra.Logger.Log(EWisdomLogType.KeyValueStore, $"{nameof(SwAndroidDefaultPrefsStore)} | SetFloat | {key} | {value}");
            SharedPreferencesEditor.Call<AndroidJavaObject>("putFloat", key, value);
            
            if (save) Save();

            return this;
        }

        public ISwKeyValueStore SetInt(string key, int value, bool isInternal = true, bool save = false)
        {
            if (key.SwIsNullOrEmpty()) return this;

            SharedPreferencesEditor.Call<AndroidJavaObject>("putInt", key, value);
            SwInfra.Logger.Log(EWisdomLogType.KeyValueStore, $"{nameof(SwAndroidDefaultPrefsStore)} | SetInt | {key} | {value}");

            if (save) Save();
            
            return this;
        }

        public ISwKeyValueStore SetString(string key, string value, bool isInternal = true, bool save = false)
        {
            if (key.SwIsNullOrEmpty()) return this;

            SwInfra.Logger.Log(EWisdomLogType.Cache, $"{key} | {value ?? "Null"}");
            SharedPreferencesEditor.Call<AndroidJavaObject>("putString", key, value);

            if (save) Save();
            
            return this;
        }

        public ISwKeyValueStore SetGenericSerializedData(string key, object value, bool isInternal = true, bool save = false)
        {
            SwInfra.Logger.LogWarning(EWisdomLogType.KeyValueStore, $"{nameof(SwAndroidDefaultPrefsStore)} | SetGenericSerializedData | {key} | GenericSerialization is not supported in AndroidPrefs");

            return this;
        }

        public ISwKeyValueStore SetDate(string key, DateTime value, bool save = false)
        {
            var jsonString = JsonConvert.SerializeObject(value, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Culture = CultureInfo.InvariantCulture,
            });

            return SetString(key, jsonString, save: save);
        }

        #endregion
    }
}