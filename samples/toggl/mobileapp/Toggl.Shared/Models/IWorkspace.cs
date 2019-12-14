using System;

namespace Toggl.Shared.Models
{
    public interface IWorkspace : IIdentifiable, IDeletable, ILastChangedDatable
    {
        string Name { get; }

        bool Admin { get; }

        DateTimeOffset? SuspendedAt { get; }

        double? DefaultHourlyRate { get; }

        string DefaultCurrency { get; }

        bool OnlyAdminsMayCreateProjects { get; }

        bool OnlyAdminsSeeBillableRates { get; }

        bool OnlyAdminsSeeTeamDashboard { get; }

        bool ProjectsBillableByDefault { get; }

        int Rounding { get; }

        int RoundingMinutes { get; }

        string LogoUrl { get; }
    }
}
