using Newtonsoft.Json;
using System;
using Toggl.Shared;

namespace Toggl.Networking.Serialization.Converters
{
    [Preserve(AllMembers = true)]
    internal sealed class PasswordConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(Password);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            => Password.From(reader.Value.ToString());

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var password = (Password)value;
            writer.WriteValue(password.ToString());
        }
    }
}
