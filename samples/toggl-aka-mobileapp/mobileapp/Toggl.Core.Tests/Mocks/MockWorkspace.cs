using System;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage;

namespace Toggl.Core.Tests.Mocks
{
    public sealed class MockWorkspace : IThreadSafeWorkspace
    {
        public string Name { get; set; }

        public bool Admin { get; set; }

        public DateTimeOffset? SuspendedAt { get; set; }

        public DateTimeOffset? ServerDeletedAt { get; set; }

        public double? DefaultHourlyRate { get; set; }

        public string DefaultCurrency { get; set; }

        public bool OnlyAdminsMayCreateProjects { get; set; }

        public bool OnlyAdminsSeeBillableRates { get; set; }

        public bool OnlyAdminsSeeTeamDashboard { get; set; }

        public bool ProjectsBillableByDefault { get; set; }

        public int Rounding { get; set; }

        public int RoundingMinutes { get; set; }

        public DateTimeOffset At { get; set; }

        public string LogoUrl { get; set; }

        public long Id { get; set; }

        public SyncStatus SyncStatus { get; set; }

        public string LastSyncErrorMessage { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsInaccessible { get; set; }

        public MockWorkspace() { }

        public MockWorkspace(
            long id,
            bool isInaccessible = false,
            SyncStatus syncStatus = SyncStatus.InSync
        ) : this()
        {
            Id = id;
            IsInaccessible = isInaccessible;
            SyncStatus = syncStatus;
        }
    }
}
