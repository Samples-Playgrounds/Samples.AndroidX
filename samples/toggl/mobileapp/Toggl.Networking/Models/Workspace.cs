using Newtonsoft.Json;
using System;
using Toggl.Networking.Serialization;
using Toggl.Shared.Models;

namespace Toggl.Networking.Models
{
    internal sealed partial class Workspace : IWorkspace
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public bool Admin { get; set; }

        [JsonProperty("SuspendedAt")]
        public DateTimeOffset? SuspendedAt { get; set; }

        public DateTimeOffset? ServerDeletedAt { get; set; }

        [IgnoreWhenPosting]
        public double? DefaultHourlyRate { get; set; }

        [IgnoreWhenPosting]
        public string DefaultCurrency { get; set; }

        [IgnoreWhenPosting]
        public bool OnlyAdminsMayCreateProjects { get; set; }

        [IgnoreWhenPosting]
        public bool OnlyAdminsSeeBillableRates { get; set; }

        [IgnoreWhenPosting]
        public bool OnlyAdminsSeeTeamDashboard { get; set; }

        [IgnoreWhenPosting]
        public bool ProjectsBillableByDefault { get; set; }

        [IgnoreWhenPosting]
        public int Rounding { get; set; }

        [IgnoreWhenPosting]
        public int RoundingMinutes { get; set; }

        [IgnoreWhenPosting]
        public DateTimeOffset At { get; set; }

        public string LogoUrl { get; set; }
    }
}
