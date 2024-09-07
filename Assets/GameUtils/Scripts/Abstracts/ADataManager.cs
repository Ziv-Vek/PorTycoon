using System;
using UnityEngine;

namespace YsoCorp {

    public static class ADataManager {

        private static int _version = 1;
        private static string _prefix = "";

        public static string GetKey(string key) {
            return _prefix + key + _version;
        }

        public static bool HasKey(string key) {
            return PlayerPrefs.HasKey(GetKey(key));
        }

        public static void DeleteAll(bool forceDeletion = false) {
#if UNITY_EDITOR
            PlayerPrefs.DeleteAll();
#endif
            if (forceDeletion == true) {
                PlayerPrefs.DeleteAll();
            }
        }

        public static void DeleteKey(string key) {
            PlayerPrefs.DeleteKey(GetKey(key));
        }

        public static void ForceSave() {
            PlayerPrefs.Save();
        }

        // INT
        public static int GetInt(string key, int defaultValue = 0) {
            return PlayerPrefs.GetInt(GetKey(key), defaultValue);
        }
        public static void SetInt(string key, int value) {
            PlayerPrefs.SetInt(GetKey(key), value);
        }

        // BOOL
        public static bool GetBool(string key, bool defaultValue = false) {
            return PlayerPrefs.GetInt(GetKey(key), defaultValue ? 1 : 0) == 1;
        }
        public static void SetBool(string key, bool value) {
            PlayerPrefs.SetInt(GetKey(key), value ? 1 : 0);
        }

        // FLOAT
        public static float GetFloat(string key, float defaultValue = 0) {
            return PlayerPrefs.GetFloat(GetKey(key), defaultValue);
        }
        public static void SetFloat(string key, float value) {
            PlayerPrefs.SetFloat(GetKey(key), value);
        }

        // STRING
        public static string GetString(string key, string value = "") {
            return PlayerPrefs.GetString(GetKey(key), value);
        }
        public static void SetString(string key, string value) {
            PlayerPrefs.SetString(GetKey(key), value);
        }

        // OBJECT
        public static T GetObject<T>(string key, string value = "{}") {
#if YC_NEWTONSOFT
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(PlayerPrefs.GetString(GetKey(key), value));
#else
            return default;
#endif
        }
        public static void SetObject<T>(string key, T value) {
#if YC_NEWTONSOFT
            PlayerPrefs.SetString(GetKey(key), Newtonsoft.Json.JsonConvert.SerializeObject(value));
#endif
        }

        // ARRAY
        public static T[] GetArray<T>(string key, string value = "[]") {
            return GetObject<T[]>(key, value);
        }
        public static void SetArray<T>(string key, T[] value) {
            SetObject(key, value);
        }

    }

}