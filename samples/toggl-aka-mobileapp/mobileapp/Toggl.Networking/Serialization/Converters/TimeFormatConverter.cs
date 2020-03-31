using Newtonsoft.Json;
using System;
using Toggl.Shared;

namespace Toggl.Networking.Serialization.Converters
{
    [Preserve(AllMembers = true)]
    public sealed class TimeFormatConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(TimeFormat);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            => TimeFormat.FromLocalizedTimeFormat(reader.Value.ToString());

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var dateFormat = (TimeFormat)value;
            writer.WriteValue(dateFormat.Localized);
        }
    }
}
