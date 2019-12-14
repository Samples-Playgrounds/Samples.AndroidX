﻿using System;
using System;
using Toggl.Core.Calendar;
using System.Linq;
using Toggl.Core.Helper;
using Toggl.Core.Models;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Models;

namespace Toggl.Core.Suggestions
{
    [Preserve(AllMembers = true)]
    public sealed class Suggestion : ITimeEntryPrototype, IEquatable<Suggestion>
    {
        public string Description { get; } = "";

        public long? ProjectId { get; } = null;

        public long? TaskId { get; } = null;

        public string ProjectColor { get; } = Helper.Colors.NoProject;

        public string ProjectName { get; } = "";

        public string TaskName { get; } = "";

        public string ClientName { get; } = "";

        public bool HasProject { get; } = false;

        public bool HasClient { get; } = false;

        public bool HasTask { get; } = false;

        public long WorkspaceId { get; }

        public bool IsBillable { get; } = false;

        public long[] TagIds { get; } = Array.Empty<long>();

        public DateTimeOffset StartTime { get; }

        public TimeSpan? Duration { get; } = null;

        public SuggestionProviderType ProviderType { get; }

        internal Suggestion(IDatabaseTimeEntry timeEntry, SuggestionProviderType providerType)
        {
            ProviderType = providerType;

            TaskId = timeEntry.TaskId;
            ProjectId = timeEntry.ProjectId;
            IsBillable = timeEntry.Billable;
            Description = timeEntry.Description;
            WorkspaceId = timeEntry.WorkspaceId;

            TagIds = timeEntry.TagIds?.ToArray() ?? Array.Empty<long>();

            if (timeEntry.Project == null)
                return;

            HasProject = true;
            ProjectName = timeEntry.Project.Name;
            ProjectColor = timeEntry.Project.Color;

            ClientName = timeEntry.Project.Client?.Name ?? "";
            HasClient = !string.IsNullOrEmpty(ClientName);

            if (timeEntry.Task == null)
                return;

            TaskName = timeEntry.Task.Name;
            HasTask = true;
        }

        internal Suggestion(CalendarItem calendarItem, long workspaceId, SuggestionProviderType providerType)
        {
            Ensure.Argument.IsNotNullOrWhiteSpaceString(calendarItem.Description, nameof(calendarItem.Description));

            WorkspaceId = workspaceId;
            Description = calendarItem.Description;

            ProviderType = providerType;
        }

        public bool Equals(Suggestion other)
        {
            if (other is null)
                return false;

            return Description == other.Description
                && ProjectId == other.ProjectId
                && TaskId == other.TaskId
                && WorkspaceId == other.WorkspaceId
                && StartTime == other.StartTime
                && Duration == other.Duration
                && IsBillable == other.IsBillable
                && TagIds.SetEquals(other.TagIds);
        }
    }
}
