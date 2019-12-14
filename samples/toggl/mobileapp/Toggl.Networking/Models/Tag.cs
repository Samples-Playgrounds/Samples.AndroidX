using Newtonsoft.Json;
using System;
using Toggl.Shared.Models;

namespace Toggl.Networking.Models
{
    internal sealed partial class Tag : ITag
    {
        public long Id { get; set; }

        public long WorkspaceId { get; set; }

        public string Name { get; set; }

        public DateTimeOffset At { get; set; }

        [JsonProperty("deleted_at")]
        public DateTimeOffset? ServerDeletedAt { get; set; }
    }
}
