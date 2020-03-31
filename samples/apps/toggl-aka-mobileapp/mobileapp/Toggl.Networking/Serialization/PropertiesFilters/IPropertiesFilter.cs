using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Toggl.Networking.Serialization
{
    public interface IPropertiesFilter
    {
        IList<JsonProperty> Filter(IList<JsonProperty> properties);
    }
}
