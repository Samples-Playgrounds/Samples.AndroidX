using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Toggl.Core.Extensions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Toggl.Core.UI.ViewModels.TimeEntriesLog.Identity;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using static Toggl.Core.UI.ViewModels.TimeEntriesLog.LogItemVisualizationIntent;

namespace Toggl.Core.UI.Transformations
{
    using LogGrouping = IGrouping<DateTime, IThreadSafeTimeEntry>;
    using MainLogSection = AnimatableSectionModel<DaySummaryViewModel, LogItemViewModel, IMainLogKey>;

    internal sealed class TimeEntriesGroupsFlattening
    {
        private delegate IImmutableList<IImmutableList<IThreadSafeTimeEntry>> GroupingStrategy(IEnumerable<IThreadSafeTimeEntry> timeEntries);

        private readonly ITimeService timeService;
        private readonly HashSet<GroupId> expandedGroups;
        private int indexInLog;

        private DurationFormat durationFormat;

        public TimeEntriesGroupsFlattening(ITimeService timeService)
        {
            this.timeService = timeService;
            expandedGroups = new HashSet<GroupId>();
        }

        public IImmutableList<MainLogSection> Flatten(IEnumerable<LogGrouping> days, IThreadSafePreferences preferences)
        {
            durationFormat = preferences.DurationFormat;
            indexInLog = 0;

            return days.Select((day, dayInLog) =>
            {
                var today = timeService.CurrentDateTime.Date;
                var sample = day.First().Start.Date;
                var daysInThePast = (today - sample).Days;

                var strategy = preferences.CollapseTimeEntries
                    ? (GroupingStrategy)bySimilarTimeEntries
                    : (GroupingStrategy)withJustSingleTimeEntries;
                return flatten(strategy, dayInLog, daysInThePast)(day);
            }).ToImmutableList();
        }

        public void ToggleGroupExpansion(GroupId groupId)
        {
            if (expandedGroups.Contains(groupId))
            {
                expandedGroups.Remove(groupId);
            }
            else
            {
                expandedGroups.Add(groupId);
            }
        }

        private Func<LogGrouping, MainLogSection> flatten(GroupingStrategy groupingStrategy, int dayInLog, int daysInThePast)
        {
            return day =>
            {
                var items = groupingStrategy(day);
                var title = DateToTitleString.Convert(day.Key, timeService.CurrentDateTime);
                var duration = totalTrackedTime(items).ToFormattedString(durationFormat);
                return new MainLogSection(
                    new DaySummaryViewModel(day.Key, title, duration),
                    flattenGroups(items, dayInLog, daysInThePast)
                );
            };
        }

        private TimeSpan totalTrackedTime(IImmutableList<IImmutableList<IThreadSafeTimeEntry>> groups)
        {
            var trackedSeconds = groups.Sum(group => group.Sum(timeEntry => timeEntry.Duration ?? 0));
            return TimeSpan.FromSeconds(trackedSeconds);
        }

        private IEnumerable<LogItemViewModel> flattenGroups(IImmutableList<IImmutableList<IThreadSafeTimeEntry>> groups, int dayInLog, int daysInThePast)
        {
            return groups.SelectMany(group => flattenGroup(group, dayInLog, daysInThePast));
        }

        private IEnumerable<LogItemViewModel> flattenGroup(IImmutableList<IThreadSafeTimeEntry> group, int dayInLog, int daysInThePast)
        {
            var sample = group.First();
            var groupId = new GroupId(sample);

            if (expandedGroups.Contains(groupId))
            {
                if (group.Count > 1)
                {
                    var headerIndex = indexInLog++;
                    return group
                        .Select(timeEntry => timeEntry.ToViewModel(groupId, GroupItem, durationFormat, indexInLog++, dayInLog, daysInThePast))
                        .Prepend(expandedHeader(groupId, group, headerIndex, dayInLog, daysInThePast));
                }

                expandedGroups.Remove(groupId);
            }

            var item = group.Count == 1
                ? sample.ToViewModel(groupId, SingleItem, durationFormat, indexInLog++, dayInLog, daysInThePast)
                : collapsedHeader(groupId, group, indexInLog++, dayInLog, daysInThePast);

            return new[] { item };
        }

        private LogItemViewModel collapsedHeader(
            GroupId groupId,
            IImmutableList<IThreadSafeTimeEntry> group,
            int indexInLog,
            int dayInLog,
            int daysInThePast)
            => header(groupId, group, CollapsedGroupHeader, indexInLog, dayInLog, daysInThePast);

        private LogItemViewModel expandedHeader(
            GroupId groupId,
            IImmutableList<IThreadSafeTimeEntry> group,
            int indexInLog,
            int dayInLog,
            int daysInThePast)
            => header(groupId, group, ExpandedGroupHeader, indexInLog, dayInLog, daysInThePast);

        private LogItemViewModel header(
            GroupId groupId,
            IImmutableList<IThreadSafeTimeEntry> group,
            LogItemVisualizationIntent visualizationIntent,
            int indexInLog,
            int dayInLog,
            int daysInThePast)
        {
            var sample = group.First();
            return new LogItemViewModel(
                groupId: groupId,
                representedTimeEntriesIds: group.Select(timeEntry => timeEntry.Id).ToArray(),
                visualizationIntent: visualizationIntent,
                isBillable: sample.Billable,
                isActive: sample.Project?.Active ?? true,
                description: sample.Description,
                duration: DurationAndFormatToString.Convert(
                    TimeSpan.FromSeconds(group.Sum(timeEntry => timeEntry.Duration ?? 0)),
                    durationFormat),
                projectName: sample.Project?.DisplayName(),
                projectColor: sample.Project?.Color,
                clientName: sample.Project?.Client?.Name,
                taskName: sample.Task?.Name,
                hasTags: sample.Tags.Any(),
                needsSync: group.Any(timeEntry => timeEntry.SyncStatus == SyncStatus.SyncNeeded),
                canSync: group.All(timeEntry => timeEntry.SyncStatus != SyncStatus.SyncFailed),
                isInaccessible: sample.IsInaccessible,
                indexInLog: indexInLog,
                dayInLog: dayInLog,
                daysInThePast: daysInThePast,
                projectIsPlaceholder: sample.Project?.IsPlaceholder() ?? false,
                taskIsPlaceholder: sample.Task?.IsPlaceholder() ?? false);
        }

        private static IImmutableList<IImmutableList<IThreadSafeTimeEntry>> bySimilarTimeEntries(
            IEnumerable<IThreadSafeTimeEntry> timeEntries)
            => timeEntries
                .GroupBy(timeEntry => timeEntry, new TimeEntriesComparer())
                .OrderByDescending(group => group.Max(timeEntry => timeEntry.Start))
                .Select(group => group.ToIImmutableList())
                .ToImmutableList();
        
        private static IImmutableList<IImmutableList<IThreadSafeTimeEntry>> withJustSingleTimeEntries(
            IEnumerable<IThreadSafeTimeEntry> timeEntries)
            => timeEntries
                .Select(timeEntry => timeEntry.Yield().ToIImmutableList())
                .ToImmutableList();

        private sealed class TimeEntriesComparer : IEqualityComparer<IThreadSafeTimeEntry>
        {
            public bool Equals(IThreadSafeTimeEntry x, IThreadSafeTimeEntry y)
                => x != null
                   && y != null
                   && x.WorkspaceId == y.WorkspaceId
                   && x.Description == y.Description
                   && x.Project?.Id == y.Project?.Id
                   && x.Task?.Id == y.Task?.Id
                   && x.Billable == y.Billable
                   && haveSameTags(x.TagIds?.ToArray(), y.TagIds?.ToArray());

            public int GetHashCode(IThreadSafeTimeEntry timeEntry)
            {
                var hashCode = HashCode.Combine(
                    timeEntry.Workspace.Id,
                    timeEntry.Description,
                    timeEntry.Project?.Id,
                    timeEntry.Task?.Id,
                    timeEntry.Billable);

                var tags = timeEntry.TagIds.OrderBy(id => id);
                foreach (var tag in tags)
                {
                    hashCode = HashCode.Combine(hashCode, tag);
                }

                return hashCode;
            }

            private static bool haveSameTags(long[] a, long[] b)
                => a?.Length == b?.Length && (a?.OrderBy(id => id).SequenceEqual(b.OrderBy(id => id)) ?? true);
        }
    }
}
