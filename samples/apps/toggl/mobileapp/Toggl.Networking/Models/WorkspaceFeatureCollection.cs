using Newtonsoft.Json;
using System.Collections.Generic;
using Toggl.Networking.Serialization.Converters;
using Toggl.Shared.Models;

namespace Toggl.Networking.Models
{
    internal sealed partial class WorkspaceFeatureCollection : IWorkspaceFeatureCollection
    {
        public long WorkspaceId { get; set; }

        [JsonConverter(typeof(ConcreteListTypeConverter<WorkspaceFeature, IWorkspaceFeature>))]
        public IEnumerable<IWorkspaceFeature> Features { get; set; }
    }
}
