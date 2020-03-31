using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;

namespace Toggl.Networking.Serialization
{
    internal sealed class IgnoreAttributeFilter<TIgnoredAttribute> : IPropertiesFilter
        where TIgnoredAttribute : IgnoreSerializationAttribute
    {
        public IList<JsonProperty> Filter(IList<JsonProperty> properties)
        {
            foreach (JsonProperty property in properties)
            {
                var attributes = property.AttributeProvider.GetAttributes(typeof(TIgnoredAttribute), false);
                if (attributes.Any())
                    property.ShouldSerialize = _ => false;
            }

            return properties;
        }
    }
}
