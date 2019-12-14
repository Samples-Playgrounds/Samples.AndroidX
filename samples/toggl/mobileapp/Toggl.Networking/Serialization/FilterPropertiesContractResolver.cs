using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Toggl.Networking.Serialization
{
    internal sealed class FilterPropertiesContractResolver : DefaultContractResolver
    {
        private readonly IList<IPropertiesFilter> filters;

        public FilterPropertiesContractResolver(IList<IPropertiesFilter> filters)
        {
            this.filters = filters;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            => filters
                .Where(filter => filter != null)
                .Aggregate(
                    base.CreateProperties(type, memberSerialization),
                    (acc, propertiesFilter) => propertiesFilter.Filter(acc));
    }
}
