using UnityEngine;
using System.Collections.Generic;

namespace YsoCorp {

    public static class AResourcesManager {

        public static void DebugCheckNbResources<T>(string path, int nb) where T : Object {
#if UNITY_EDITOR
            T[] elems = LoadIterator<T>(path);
            if (elems.Length != nb) {
                Debug.LogError(path + " not correct number " + nb + "!=" + elems.Length);
            }
            UnloadUnusedAssets();
#endif
        }

        public static void UnloadUnusedAssets() {
            Resources.UnloadUnusedAssets();
        }

        public static T Load<T>(string path) where T : Object {
            return Resources.Load<T>(path);
        }

        public static T[] LoadIterator<T>(string path, int startIndex = 0) where T : Object {
            List<T> ts = new List<T>();
            for (int i = startIndex; ; i++) {
                T t = Load<T>(path + i);
                if (t) {
                    ts.Add(t);
                } else {
                    break;
                }
            }
            DebugLog("[" + path + "] : " + ts.Count + " " + typeof(T).Name + " load !");
            return ts.ToArray();
        }

        public static Dictionary<string, T> LoadDictionary<T>(string path) where T : Object {
            Dictionary<string, T> dic = new Dictionary<string, T>();
            T[] files = Resources.LoadAll<T>(path);
            foreach (T t in files) {
                dic[t.name] = t;
            }
            DebugLog("[" + path + "] : " + dic.Count + " " + typeof(T).Name + " load !");
            return dic;
        }

        public static T LoadJson<T>(string file) {
            TextAsset jsonFile = (TextAsset)Resources.Load(file, typeof(TextAsset));
            if (jsonFile == null) {
                DebugLog("[JSON]: " + file + " not found !");
                return default(T);
            }
            return JsonUtility.FromJson<T>(jsonFile.text);
        }

        private static void DebugLog(object message) {
            if (GameUtils.YCManager.instance.ycConfig.activeLogs.HasFlag(GameUtils.YCConfig.YCLogCategories.GUResourceLoad)) {
                Debug.Log("[GameUtils - Ressource Loader] " + message);
            }
        }
    }
}
