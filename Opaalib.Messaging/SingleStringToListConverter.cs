using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Opaalib.Messaging
{
    internal class SingleStringToListConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<string>) || objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(List<string>) && reader.ValueType == typeof(string))
            {
                var str = serializer.Deserialize<string>(reader);
                return new List<string> { str };
            }

            if (objectType == typeof(List<string>) && reader.TokenType == JsonToken.StartArray)
            {
                var list = serializer.Deserialize<List<string>>(reader);
                return list;
            }

            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is List<string> list)
            {
                if (list.Count == 1) writer.WriteValue(list[0]);
                else serializer.Serialize(writer, value, typeof(List<string>));
                return;
            }


            throw new NotImplementedException();
        }
    }
}