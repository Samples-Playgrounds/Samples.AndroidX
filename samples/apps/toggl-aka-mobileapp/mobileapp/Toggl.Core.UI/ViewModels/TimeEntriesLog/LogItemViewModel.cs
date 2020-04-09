using System;
using System.Linq;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels.TimeEntriesLog.Identity;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels.TimeEntriesLog
{
    public sealed class LogItemViewModel : IDiffable<IMainLogKey>, IEquatable<LogItemViewModel>
    {
        public GroupId GroupId { get; }
        public long[] RepresentedTimeEntriesIds { get; }
        public LogItemVisualizationIntent VisualizationIntent { get; }

        public bool IsBillable { get; }
        public bool IsActive { get; }
        public string Description { get; }
        public string ProjectName { get; }
        public string ProjectColor { get; }
        public string ClientName { get; }
        public string TaskName { get; }
        public string Duration { get; }

        public bool HasProject => !string.IsNullOrEmpty(ProjectName);
        public bool HasTags { get; }
        public bool HasDescription => !string.IsNullOrEmpty(Description);

        public bool NeedsSync { get; }
        public bool CanContinue { get; }

        public IMainLogKey Identity { get; }

        public int IndexInLog { get; }
        public int DayInLog { get; }
        public int DaysInThePast { get; }

        public bool IsTimeEntryGroupHeader =>
            VisualizationIntent == LogItemVisualizationIntent.ExpandedGroupHeader ||
            VisualizationIntent == LogItemVisualizationIntent.CollapsedGroupHeader;

        public bool BelongsToGroup =>
            VisualizationIntent == LogItemVisualizationIntent.GroupItem;

        public bool ProjectIsPlaceholder { get; }

        public bool TaskIsPlaceholder { get; }

        public LogItemViewModel(
            GroupId groupId,
            long[] representedTimeEntriesIds,
            LogItemVisualizationIntent visualizationIntent,
            bool isBillable,
            bool isActive,
            string description,
            string duration,
            string projectName,
            string projectColor,
            string clientName,
            string taskName,
            bool hasTags,
            bool needsSync,
            bool canSync,
            bool isInaccessible,
            int indexInLog,
            int dayInLog,
            int daysInThePast,
            bool projectIsPlaceholder,
            bool taskIsPlaceholder)
        {
            GroupId = groupId;
            RepresentedTimeEntriesIds = representedTimeEntriesIds.OrderBy(id => id).ToArray();
            VisualizationIntent = visualizationIntent;
            IsBillable = isBillable;
            IsActive = isActive;
            Description = description;
            Duration = duration;
            ProjectName = projectName ?? string.Empty;
            ProjectColor = projectColor ?? string.Empty;
            ClientName = clientName ?? string.Empty;
            TaskName = taskName ?? string.Empty;
            HasTags = hasTags;
            NeedsSync = needsSync;
            CanContinue = canSync && !isInaccessible;

            Identity = IsTimeEntryGroupHeader
                ? new TimeEntriesGroupKey(groupId) as IMainLogKey
                : new SingleTimeEntryKey(representedTimeEntriesIds.Single());

            IndexInLog = indexInLog;
            DayInLog = dayInLog;
            DaysInThePast = daysInThePast;
            ProjectIsPlaceholder = projectIsPlaceholder;
            TaskIsPlaceholder = taskIsPlaceholder;
        }

        public bool Equals(LogItemViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return RepresentedTimeEntriesIds.SequenceEqual(other.RepresentedTimeEntriesIds)
                && IsBillable == other.IsBillable
                && IsActive == other.IsActive
                && string.Equals(Description, other.Description)
                && string.Equals(ProjectName, other.ProjectName)
                && string.Equals(ProjectColor, other.ProjectColor)
                && string.Equals(ClientName, other.ClientName)
                && string.Equals(TaskName, other.TaskName)
                && Duration.Equals(other.Duration)
                && HasProject == other.HasProject
                && HasTags == other.HasTags
                && NeedsSync == other.NeedsSync
                && CanContinue == other.CanContinue
                && VisualizationIntent == other.VisualizationIntent;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return obj is LogItemViewModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            var hasSoFar = HashCode.Combine(
                RepresentedTimeEntriesIds.Aggregate(
                    (acc, id) => HashCode.Combine(acc, id)),
                IsBillable,
                IsActive,
                Description,
                ProjectName,
                ProjectColor,
                ClientName);

            return HashCode.Combine(
                hasSoFar,
                TaskName,
                Duration,
                HasProject,
                HasTags,
                NeedsSync,
                CanContinue);
        }

        public static bool operator ==(LogItemViewModel left, LogItemViewModel right) => Equals(left, right);

        public static bool operator !=(LogItemViewModel left, LogItemViewModel right) => !Equals(left, right);

    }
}
