using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using Toggl.Networking.Serialization;
using Toggl.Shared.Models;

namespace Toggl.Networking.Models
{
    internal sealed partial class TimeEntry : ITimeEntry
    {
        public long Id { get; set; }

        public long WorkspaceId { get; set; }

        public long? ProjectId { get; set; }

        public long? TaskId { get; set; }

        public bool Billable { get; set; }

        public DateTimeOffset Start { get; set; }

        public DateTimeOffset? Stop
        {
            get => Duration.HasValue ? Start.AddSeconds(Duration.Value) : null as DateTimeOffset?;
            set { }
        }

        [JsonIgnore]
        public long? Duration { get; set; }

        [JsonProperty("duration")]
        public long ApiDuration
        {
            get => Duration ?? -Start.ToUnixTimeSeconds();
            set => Duration = value < 0 ? null : value as long?;
        }

        public string Description { get; set; }

        public IEnumerable<long> TagIds { get; set; }

        public DateTimeOffset At { get; set; }

        [IgnoreWhenPosting]
        public DateTimeOffset? ServerDeletedAt { get; set; }

        [IgnoreWhenPosting]
        public long UserId { get; set; }

        public string CreatedWith { get; set; }
    }
}
