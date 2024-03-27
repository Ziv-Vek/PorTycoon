using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace SupersonicWisdomSDK
{
    public class DictionaryConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, T>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jObject = JObject.Load(reader);
            var dictionary = new Dictionary<string, T>();

            foreach (var property in jObject.Properties())
            {
                try {
                    dictionary.Add((property.Name), property.Value.ToObject<T>());
                }
                catch (Exception e)
                {
                    SwInfra.Logger.Log(EWisdomLogType.Config, $"{property.Name} is null or can't be convert, Error: {e.Message}");
                }
            }

            return dictionary;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is not Dictionary<string, T> dictionary)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();
            
            foreach (var pair in dictionary)
            {
                writer.WritePropertyName(pair.Key);
                serializer.Serialize(writer, pair.Value);
            }
            
            writer.WriteEndObject();
        }
    }
}