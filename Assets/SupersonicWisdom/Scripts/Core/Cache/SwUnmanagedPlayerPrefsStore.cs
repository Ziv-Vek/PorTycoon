using UnityEngine;

namespace SupersonicWisdomSDK
{
    /// <summary>
    /// The SwUnmanagedPlayerPrefsStore class is a wrapper for Unity's PlayerPrefs. 
    /// It is designed to be used by external libraries interacting with PlayerPrefs.
    /// For internal SDK use, the SwPlayerPrefsStore should be utilized instead as it provides 
    /// additional functionality such as keys prefixing and AES encryption.
    /// </summary>
    public class SwUnmanagedPlayerPrefsStore : ISwKeyValueStore
    {      
        #region --- Public Methods ---

        public virtual void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
        }

        public virtual ISwKeyValueStore DeleteKey(string key, bool isInternal = true)
        {
            PlayerPrefs.DeleteKey(key);
            return this;
        }

        public virtual bool GetBoolean(string key, bool defaultValue = false, bool isInternal = true)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) != 0;
        }

        public virtual float GetFloat(string key, float defaultValue = 0f, bool isInternal = true)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public virtual int GetInt(string key, int defaultValue = 0, bool isInternal = true)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public virtual string GetString(string key, string defaultValue = "", bool isInternal = true)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        // Note: This method implementation relies on Unity's JsonUtility and its limitations regarding types it can serialize/deserialize.
        public virtual T GetGenericSerializedData<T>(string key, T defaultValue, bool isInternal = true)
        {
            var jsonString = PlayerPrefs.GetString(key, JsonUtility.ToJson(defaultValue));
            return JsonUtility.FromJson<T>(jsonString);
        }

        public virtual bool HasKey(string key, bool isInternal = true)
        {
            return PlayerPrefs.HasKey(key);
        }

        public virtual void Save ()
        {
            PlayerPrefs.Save();
        }

        public virtual ISwKeyValueStore SetBoolean(string key, bool value, bool isInternal = true, bool save = false)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            if (save) Save();
            return this;
        }

        public virtual ISwKeyValueStore SetFloat(string key, float value, bool isInternal = true, bool save = false)
        {
            PlayerPrefs.SetFloat(key, value);
            if (save) Save();
            return this;
        }

        public virtual ISwKeyValueStore SetInt(string key, int value, bool isInternal = true, bool save = false)
        {
            PlayerPrefs.SetInt(key, value);
            if (save) Save();
            return this;
        }

        public virtual ISwKeyValueStore SetString(string key, string value, bool isInternal = true, bool save = false)
        {
            PlayerPrefs.SetString(key, value);
            if (save) Save();
            return this;
        }

        // Note: This method implementation relies on Unity's JsonUtility and its limitations regarding types it can serialize/deserialize.
        public virtual ISwKeyValueStore SetGenericSerializedData(string key, object value, bool isInternal = true, bool save = false)
        {
            var jsonString = JsonUtility.ToJson(value);
            PlayerPrefs.SetString(key, jsonString);
            if (save) Save();
            return this;
        }

        #endregion
    }
}