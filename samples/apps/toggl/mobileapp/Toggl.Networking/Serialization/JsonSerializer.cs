using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using static Toggl.Networking.Serialization.SerializationReason;

namespace Toggl.Networking.Serialization
{
    internal sealed class JsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings defaultSettings = SerializerSettings.For(new DefaultContractResolver());

        private JsonSerializerSettings postSettings()
            => SerializerSettings.For(new FilterPropertiesContractResolver(
                new List<IPropertiesFilter>
                {
                    new IgnoreAttributeFilter<IgnoreWhenPostingAttribute>()
                }));

        public T Deserialize<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json, defaultSettings);
            }
            catch (Exception e)
            {
                throw new DeserializationException(typeof(T), e);
            }
        }

        public string Serialize<T>(T data, SerializationReason reason = Default)
        {
            try
            {
                return JsonConvert.SerializeObject(data, Formatting.None, getSettings(reason));
            }
            catch (Exception e)
            {
                throw new SerializationException(typeof(T), e);
            }
        }

        private JsonSerializerSettings getSettings(SerializationReason reason)
        {
            switch (reason)
            {
                case Post:
                    return postSettings();
                default:
                    return defaultSettings;
            }
        }
    }
}
