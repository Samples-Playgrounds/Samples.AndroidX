using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using Toggl.Networking.Models;
using Toggl.Networking.Serialization;
using Toggl.Networking.Serialization.Attributes;
using Toggl.Shared.Models;
using Xunit;
using static Toggl.Shared.WorkspaceFeatureId;

namespace Toggl.Networking.Tests.Serialization
{
    public sealed class RequiresFeatureAttributeSerializaitonTests
    {
        [Fact, LogIfTooSlow]
        public void ThePropertyIsNotSerializedIfTheFeatureIsNotEnabled()
        {
            var settings = getSettings(getFeatures(false, false));

            var json = JsonConvert.SerializeObject(model, Formatting.None, settings);

            json.Should().Be(serializedWithoutTheProperty);
        }

        [Fact, LogIfTooSlow]
        public void ThePropertyIsSerializedIfTheFeatureIsEnabled()
        {
            var settings = getSettings(getFeatures(true, false));

            var json = JsonConvert.SerializeObject(model, Formatting.None, settings);

            json.Should().Be(serializedWithTheProperty);
        }

        [Fact, LogIfTooSlow]
        public void ThePropertyIsSerializedIfAllRequiredFeaturesAreEnabled()
        {
            var settings = getSettings(getFeatures(true, true));

            var json = JsonConvert.SerializeObject(model, Formatting.None, settings);

            json.Should().Be(serializedWithAllProperties);
        }

        [Fact, LogIfTooSlow]
        public void ThePropertyIsNotSerializedIfTheFeatureIsNotInTheListAtAll()
        {
            var settings = getSettings(new WorkspaceFeatureCollection { Features = new List<WorkspaceFeature>() });

            var json = JsonConvert.SerializeObject(model, Formatting.None, settings);

            json.Should().Be(serializedWithoutTheProperty);
        }

        [Fact, LogIfTooSlow]
        public void ThePropertyIsSerializedIfTheSerializerHasNoKnowledgeOfEnabledFeatures()
        {
            var settings = SerializerSettings.For(new DefaultContractResolver());

            var json = JsonConvert.SerializeObject(model, Formatting.None, settings);

            json.Should().Be(serializedWithAllProperties);
        }

        private class TestModel
        {
            public int A { get; set; }

            [RequiresFeature(Pro)]
            public int B { get; set; }

            [RequiresFeature(Pro)]
            [RequiresFeature(TimeAudits)]
            public int C { get; set; }
        }

        private TestModel model => new TestModel { A = 1, B = 2, C = 3 };

        private string serializedWithTheProperty = "{\"a\":1,\"b\":2}";

        private string serializedWithoutTheProperty = "{\"a\":1}";

        private string serializedWithAllProperties = "{\"a\":1,\"b\":2,\"c\":3}";

        private JsonSerializerSettings getSettings(IWorkspaceFeatureCollection features)
            => SerializerSettings.For(new FilterPropertiesContractResolver(new List<IPropertiesFilter>
            {
                new RequiresFeatureAttributeFilter(features)
            }));

        private IWorkspaceFeatureCollection getFeatures(bool proEnabled, bool timeAuditsEnabled)
            => new WorkspaceFeatureCollection
            {
                Features = new[]
                {
                    new WorkspaceFeature { Enabled = proEnabled, FeatureId = Pro },
                    new WorkspaceFeature { Enabled = timeAuditsEnabled, FeatureId = TimeAudits }
                }
            };
    }
}
