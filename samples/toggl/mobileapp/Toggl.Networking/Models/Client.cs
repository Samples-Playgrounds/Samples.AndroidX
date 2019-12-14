using Newtonsoft.Json;
using System;
using Toggl.Shared.Models;

namespace Toggl.Networking.Models
{
    internal sealed partial class Client : IClient
    {
        public long Id { get; set; }

        [JsonProperty("wid")]
        public long WorkspaceId { get; set; }

        public string Name { get; set; }

        public DateTimeOffset At { get; set; }

        public DateTimeOffset? ServerDeletedAt { get; set; }
    }
}
