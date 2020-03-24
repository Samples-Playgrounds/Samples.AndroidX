using System;
using Toggl.Networking.Serialization;
using Toggl.Shared.Models;

namespace Toggl.Networking.Models
{
    internal sealed partial class Project : IProject
    {
        public long Id { get; set; }

        public long WorkspaceId { get; set; }

        public long? ClientId { get; set; }

        public string Name { get; set; }

        public bool IsPrivate { get; set; }

        public bool Active { get; set; }

        public DateTimeOffset At { get; set; }

        [IgnoreWhenPosting]
        public DateTimeOffset? ServerDeletedAt { get; set; }

        private string color;
        public string Color
        {
            get => color;
            set => color = value.ToLower();
        }

        [IgnoreWhenPosting]
        public bool? Billable { get; set; }

        [IgnoreWhenPosting]
        public bool? Template { get; set; }

        [IgnoreWhenPosting]
        public bool? AutoEstimates { get; set; }

        [IgnoreWhenPosting]
        public long? EstimatedHours { get; set; }

        [IgnoreWhenPosting]
        public double? Rate { get; set; }

        [IgnoreWhenPosting]
        public string Currency { get; set; }

        [IgnoreWhenPosting]
        public int? ActualHours { get; set; }
    }
}
