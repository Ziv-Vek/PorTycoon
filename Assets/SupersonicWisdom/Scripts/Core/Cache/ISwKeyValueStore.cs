namespace SupersonicWisdomSDK
{
    public interface ISwKeyValueStore
    {
        #region --- Public Methods ---

        void DeleteAll();
        ISwKeyValueStore DeleteKey(string key, bool isInternal = true);
        bool GetBoolean(string key, bool defaultValue = false, bool isInternal = true);
        float GetFloat(string key, float defaultValue = 0f, bool isInternal = true);
        int GetInt(string key, int defaultValue = 0, bool isInternal = true);
        string GetString(string key, string defaultValue = "", bool isInternal = true);
        T GetGenericSerializedData<T>(string key, T defaultValue, bool isInternal = true);
        bool HasKey(string key, bool isInternal = true);
        void Save ();
        ISwKeyValueStore SetBoolean(string key, bool value, bool isInternal = true, bool save = false);
        ISwKeyValueStore SetFloat(string key, float value, bool isInternal = true, bool save = false);
        ISwKeyValueStore SetInt(string key, int value, bool isInternal = true, bool save = false);
        ISwKeyValueStore SetString(string key, string value, bool isInternal = true, bool save = false);
        ISwKeyValueStore SetGenericSerializedData(string key, object value, bool isInternal = true, bool save = false);

        #endregion
    }
}