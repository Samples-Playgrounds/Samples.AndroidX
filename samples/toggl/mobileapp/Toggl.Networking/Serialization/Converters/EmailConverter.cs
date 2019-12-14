using Newtonsoft.Json;
using System;
using Toggl.Shared;

namespace Toggl.Networking.Serialization.Converters
{
    [Preserve(AllMembers = true)]
    internal sealed class EmailConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(Email);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            => Email.From(reader.Value.ToString());

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var email = (Email)value;
            writer.WriteValue(email.ToString());
        }
    }
}
