using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SupersonicWisdomSDK
{
    internal class SwJsonDictionary : Dictionary<string, object>, IComparable
    {
        public SwJsonDictionary() { }
        
        public SwJsonDictionary(Dictionary<string, object> other) : base(other ?? new Dictionary<string, object>()) { }

        public SwJsonDictionary(JObject jObject)
        {
            if (jObject == null) return;

            foreach (var keyValuePair in jObject)
            {
                this[keyValuePair.Key] = keyValuePair.Value;
            }
        }
        
        public SwJsonDictionary(ICollection<KeyValuePair<string, object>> someDictionary)
        {
            if (someDictionary == null) return;
            
            foreach (var keyValuePair in someDictionary)
            {
                this[keyValuePair.Key] = keyValuePair.Value;
            }
        }

        public SwJsonDictionary(object anyObject)
        {
            var someDictionary = Parse(JsonConvert.SerializeObject(anyObject));

            if (someDictionary == null) return;

            Merge(someDictionary);
        }
        
        public static SwJsonDictionary Parse(object other)
        {
            switch (other)
            {
                case Dictionary<string, object> otherDictionary:
                    return new SwJsonDictionary(otherDictionary);
                case JObject jObject:
                    return new SwJsonDictionary(jObject);
                case ICollection<KeyValuePair<string, object>> jObject:
                    return new SwJsonDictionary(jObject);
                case string jsonString when SafelyParseDictionary(jsonString) is { } jsonDictionary:
                    return new SwJsonDictionary(jsonDictionary);
                default:
                    return null;
            }
        }

        private static Dictionary<string, object> SafelyParseDictionary(string jsonString)
        {
            Dictionary<string, object> currentLevel = null;
            
            try
            {
                currentLevel = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);

                foreach (var keyValuePair in currentLevel ?? new Dictionary<string, object>())
                {
                    var parsed = Parse(keyValuePair.Value);

                    if (parsed != null)
                    {
                        currentLevel.SwAddOrReplace(keyValuePair.Key, parsed);
                    }
                }
                
            }
            catch (Exception)
            {
                // Ignore
            }
            
            return currentLevel;
        }

        internal SwJsonDictionary Merge(params Dictionary<string, object>[] others)
        {
            foreach (var other in others)
            {
                if (other == null) continue;

                foreach (var keyValuePair in other)
                {
                    if (!ContainsKey(keyValuePair.Key))
                    {
                        this[keyValuePair.Key] = keyValuePair.Value;
                    }
                    else
                    {
                        if (this[keyValuePair.Key] is SwJsonDictionary dictionaryOfThis && keyValuePair.Value is Dictionary<string, object> dictionaryOfOther)
                        {
                            dictionaryOfThis.Merge(dictionaryOfOther);
                        }
                        else
                        {
                            this[keyValuePair.Key] = keyValuePair.Value;
                        }

                    }
                }
            }

            return this;
        }

        public override string ToString()
        {
            return this.SwToString();
        }

        public int CompareTo(object other)
        {
            if (!(other is SwJsonDictionary otherJsonDictionary)) return -1;

            return string.Compare(this.SwToJsonString(), otherJsonDictionary.SwToJsonString(), StringComparison.Ordinal);
        }
        
        public Dictionary<string, object> AsDictionary()
        {
            return new Dictionary<string, object>(this);
        }
    }
}