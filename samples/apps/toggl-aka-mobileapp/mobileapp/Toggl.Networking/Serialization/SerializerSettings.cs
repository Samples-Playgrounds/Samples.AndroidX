using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Toggl.Networking.Serialization
{
    internal static class SerializerSettings
    {
        public static JsonSerializerSettings For<TContractResolver>(TContractResolver contractResolver)
            where TContractResolver : DefaultContractResolver
        {
            contractResolver.NamingStrategy = new SnakeCaseNamingStrategy();

            return new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                DateFormatString = @"yyyy-MM-dd\THH:mm:ssK"
            };
        }
    }
}
