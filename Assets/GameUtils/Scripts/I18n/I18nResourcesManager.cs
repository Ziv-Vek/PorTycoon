using UnityEngine;
using System.Collections.Generic;
#if YC_NEWTONSOFT
using Newtonsoft.Json;
#endif

namespace YsoCorp {
    namespace GameUtils {

        [DefaultExecutionOrder(-15)]
        public class I18nResourcesManager : BaseManager {

            public Dictionary<string, Dictionary<string, string>> i18ns = new Dictionary<string, Dictionary<string, string>>();

            private void Awake() {
                Dictionary<string, TextAsset> texts = AResourcesManager.LoadDictionary<TextAsset>("I18n");
                foreach (KeyValuePair<string, TextAsset> t in texts) {
#if YC_NEWTONSOFT
                    this.i18ns[t.Key] = JsonConvert.DeserializeObject<Dictionary<string, string>>(t.Value.text);
                    TextAsset jsonBase = AResourcesManager.Load<TextAsset>("I18nBase/" + t.Key);
                    if (jsonBase) {
                        Dictionary<string, string> bases = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonBase.text);
                        foreach (KeyValuePair<string, string> tBase in bases) {
                            if (this.i18ns[t.Key].ContainsKey(tBase.Key) == false) {
                                this.i18ns[t.Key][tBase.Key] = tBase.Value;
                            }
                        }
                    }
#endif
                }
            }

            public Dictionary<string, string> GetStrings(string lang) {
                if (this.i18ns.ContainsKey(lang)) {
                    return this.i18ns[lang];
                }
                return null;
            }

            public string GetString(string key, string lang) {
                Dictionary<string, string> strings = this.GetStrings(lang);
                if (strings != null && strings.ContainsKey(key)) {
                    return strings[key];
                } else if (strings != null && strings.Count > 0) {
                    Debug.LogError("[YCManager.i18nManager] Couldn't find the " + YCManager.instance.dataManager.GetLanguage() + " translation for \"" + key + "\"");
                }
                return key;
            }

            public Sprite GetSprite(string lang) {
                return AResourcesManager.Load<Sprite>("Sprites/I18n/" + lang);
            }
        }

    }
}