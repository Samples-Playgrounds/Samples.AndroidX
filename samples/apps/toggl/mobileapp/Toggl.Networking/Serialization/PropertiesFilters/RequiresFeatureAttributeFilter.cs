using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using Toggl.Networking.Serialization.Attributes;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;

namespace Toggl.Networking.Serialization
{
    internal sealed class RequiresFeatureAttributeFilter : IPropertiesFilter
    {
        private readonly IWorkspaceFeatureCollection featuresCollection;

        public RequiresFeatureAttributeFilter(IWorkspaceFeatureCollection featuresCollection)
        {
            this.featuresCollection = featuresCollection;
        }

        public IList<JsonProperty> Filter(IList<JsonProperty> properties)
        {
            if (featuresCollection == null)
                return properties;

            foreach (JsonProperty property in properties)
            {
                var attributes = property.AttributeProvider.GetAttributes(typeof(RequiresFeatureAttribute), false);
                if (attributes.OfType<RequiresFeatureAttribute>().Any(isNotEnabled))
                    property.ShouldSerialize = _ => false;
            }

            return properties;
        }

        private bool isNotEnabled(RequiresFeatureAttribute attribute)
            => !featuresCollection.IsEnabled(attribute.RequiredFeature);
    }
}