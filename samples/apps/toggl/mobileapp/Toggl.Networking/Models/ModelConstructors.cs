using Toggl.Shared;
using Toggl.Shared.Models;

namespace Toggl.Networking.Models
{
    [Preserve(AllMembers = true)]
    internal sealed partial class Client
    {
        public Client() { }

        public Client(IClient entity)
        {
            Id = entity.Id;
            WorkspaceId = entity.WorkspaceId;
            Name = entity.Name;
            At = entity.At;
            ServerDeletedAt = entity.ServerDeletedAt;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class Country
    {
        public Country() { }

        public Country(ICountry entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            CountryCode = entity.CountryCode;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class Location
    {
        public Location() { }

        public Location(ILocation entity)
        {
            CountryName = entity.CountryName;
            CountryCode = entity.CountryCode;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class Preferences
    {
        public Preferences() { }

        public Preferences(IPreferences entity)
        {
            TimeOfDayFormat = entity.TimeOfDayFormat;
            DateFormat = entity.DateFormat;
            DurationFormat = entity.DurationFormat;
            CollapseTimeEntries = entity.CollapseTimeEntries;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class Project
    {
        public Project() { }

        public Project(IProject entity)
        {
            Id = entity.Id;
            WorkspaceId = entity.WorkspaceId;
            ClientId = entity.ClientId;
            Name = entity.Name;
            IsPrivate = entity.IsPrivate;
            Active = entity.Active;
            Color = entity.Color;
            Billable = entity.Billable;
            Template = entity.Template;
            AutoEstimates = entity.AutoEstimates;
            EstimatedHours = entity.EstimatedHours;
            Rate = entity.Rate;
            Currency = entity.Currency;
            ActualHours = entity.ActualHours;
            At = entity.At;
            ServerDeletedAt = entity.ServerDeletedAt;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class Tag
    {
        public Tag() { }

        public Tag(ITag entity)
        {
            Id = entity.Id;
            WorkspaceId = entity.WorkspaceId;
            Name = entity.Name;
            At = entity.At;
            ServerDeletedAt = entity.ServerDeletedAt;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class Task
    {
        public Task() { }

        public Task(ITask entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            ProjectId = entity.ProjectId;
            WorkspaceId = entity.WorkspaceId;
            UserId = entity.UserId;
            EstimatedSeconds = entity.EstimatedSeconds;
            Active = entity.Active;
            TrackedSeconds = entity.TrackedSeconds;
            At = entity.At;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class TimeEntry
    {
        public TimeEntry() { }

        public TimeEntry(ITimeEntry entity)
        {
            Id = entity.Id;
            WorkspaceId = entity.WorkspaceId;
            ProjectId = entity.ProjectId;
            TaskId = entity.TaskId;
            Billable = entity.Billable;
            Start = entity.Start;
            Duration = entity.Duration;
            Description = entity.Description;
            TagIds = entity.TagIds;
            UserId = entity.UserId;
            At = entity.At;
            ServerDeletedAt = entity.ServerDeletedAt;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class User
    {
        public User() { }

        public User(IUser entity)
        {
            Id = entity.Id;
            ApiToken = entity.ApiToken;
            DefaultWorkspaceId = entity.DefaultWorkspaceId;
            Email = entity.Email;
            Fullname = entity.Fullname;
            BeginningOfWeek = entity.BeginningOfWeek;
            Language = entity.Language;
            ImageUrl = entity.ImageUrl;
            Timezone = entity.Timezone;
            At = entity.At;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class Workspace
    {
        public Workspace() { }

        public Workspace(IWorkspace entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Admin = entity.Admin;
            SuspendedAt = entity.SuspendedAt;
            DefaultHourlyRate = entity.DefaultHourlyRate;
            DefaultCurrency = entity.DefaultCurrency;
            OnlyAdminsMayCreateProjects = entity.OnlyAdminsMayCreateProjects;
            OnlyAdminsSeeBillableRates = entity.OnlyAdminsSeeBillableRates;
            OnlyAdminsSeeTeamDashboard = entity.OnlyAdminsSeeTeamDashboard;
            ProjectsBillableByDefault = entity.ProjectsBillableByDefault;
            Rounding = entity.Rounding;
            RoundingMinutes = entity.RoundingMinutes;
            LogoUrl = entity.LogoUrl;
            At = entity.At;
            ServerDeletedAt = entity.ServerDeletedAt;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class WorkspaceFeature
    {
        public WorkspaceFeature() { }

        public WorkspaceFeature(IWorkspaceFeature entity)
        {
            FeatureId = entity.FeatureId;
            Enabled = entity.Enabled;
        }
    }

    [Preserve(AllMembers = true)]
    internal sealed partial class WorkspaceFeatureCollection
    {
        public WorkspaceFeatureCollection() { }

        public WorkspaceFeatureCollection(IWorkspaceFeatureCollection entity)
        {
            WorkspaceId = entity.WorkspaceId;
            Features = entity.Features;
        }
    }
}
