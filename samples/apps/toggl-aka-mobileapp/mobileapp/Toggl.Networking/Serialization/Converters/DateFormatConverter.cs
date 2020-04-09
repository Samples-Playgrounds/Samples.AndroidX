using Newtonsoft.Json;
using System;
using Toggl.Shared;

namespace Toggl.Networking.Serialization.Converters
{
    [Preserve(AllMembers = true)]
    public sealed class DateFormatConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
            => objectType == typeof(DateFormat);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            => DateFormat.FromLocalizedDateFormat(reader.Value.ToString());

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var dateFormat = (DateFormat)value;
            writer.WriteValue(dateFormat.Localized);
        }
    }
}
