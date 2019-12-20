using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Extensions;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Transformations;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Toggl.Core.UI.ViewModels.TimeEntriesLog.Identity;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Xunit;

namespace Toggl.Core.Tests.UI.Transformations
{
    public sealed class TimeEntriesFlatenningTests
    {
        private static readonly DateTimeOffset now = new DateTimeOffset(2019, 02, 07, 16, 25, 38, TimeSpan.FromHours(-1));

        private static readonly IThreadSafeWorkspace workspaceA = new MockWorkspace { Id = 1, IsInaccessible = false };
        private static readonly IThreadSafeWorkspace workspaceB = new MockWorkspace { Id = 2, IsInaccessible = false };

        private static readonly IThreadSafeTimeEntry[] singleItemGroup =
            group(createTimeEntry(now, workspaceA, "S", duration: 1));

        private static readonly IThreadSafeTimeEntry[] groupA =
            group(
                createTimeEntry(now, workspaceA, "A", duration: 1),
                createTimeEntry(now, workspaceA, "A", duration: 2),
                createTimeEntry(now, workspaceA, "A", duration: 4));

        private static readonly IThreadSafeTimeEntry[] groupB =
            group(
                createTimeEntry(now, workspaceB, "B", duration: 1),
                createTimeEntry(now, workspaceB, "B", duration: 2),
                createTimeEntry(now, workspaceB, "B", duration: 4));

        private static readonly IThreadSafeTimeEntry[] twoWorkspaces =
            group(
                createTimeEntry(now, workspaceA, "B", duration: 1),
                createTimeEntry(now, workspaceB, "B", duration: 2));

        private static readonly IThreadSafeTimeEntry[] differentDescriptions =
            group(
                createTimeEntry(now, workspaceA, "C1", duration: 1),
                createTimeEntry(now, workspaceA, "C1", duration: 2),
                createTimeEntry(now, workspaceA, "C2", duration: 4)
            );

        private static readonly IThreadSafeTimeEntry[] longDuration =
            group(
                createTimeEntry(now, workspaceA, "D1", duration: (long)TimeSpan.FromHours(1.5).TotalSeconds),
                createTimeEntry(now, workspaceA, "D1", duration: (long)TimeSpan.FromHours(2.5).TotalSeconds),
                createTimeEntry(now, workspaceA, "D2", duration: (long)TimeSpan.FromHours(3.5).TotalSeconds)
            );

        private readonly ITimeService timeService;

        public TimeEntriesFlatenningTests()
        {
            timeService = Substitute.For<ITimeService>();
            timeService.CurrentDateTime.Returns(now);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void TransformsTimeEntriesIntoACorrectTree(
            DurationFormat durationFormat,
            IEnumerable<IGrouping<DateTime, IThreadSafeTimeEntry>> log,
            HashSet<GroupId> expandedGroups,
            params AnimatableSectionModel<DaySummaryViewModel, LogItemViewModel, IMainLogKey>[] expectedTree)
        {
            var preferences =
                new MockPreferences
                {
                    CollapseTimeEntries = true,
                    DateFormat = DateFormat.FromLocalizedDateFormat("YYYY-MM-DD"),
                    DurationFormat = durationFormat
                };

            var collapsingStrategy = new TimeEntriesGroupsFlattening(timeService);
            expandedGroups.ForEach(collapsingStrategy.ToggleGroupExpansion);

            var actualTree = collapsingStrategy.Flatten(log, preferences);

            actualTree.Should().BeEquivalentTo(expectedTree);
        }

        public static IEnumerable<object[]> TestData
            => new[]
            {
                new object[]
                {
                    DurationFormat.Classic,
                    new[] { day(now, groupA) },
                    withExpanded(),
                    new[] { logOf(now.DateTime, "Today", "07 sec", collapsed(groupA, DurationFormat.Classic)) }
                },
                new object[]
                {
                    DurationFormat.Classic,
                    new[] { day(now, groupA) },
                    withExpanded(groupA),
                    new[] { logOf(now.DateTime, "Today", "07 sec", expanded(groupA, DurationFormat.Classic)) }
                },
                new object[]
                {
                    DurationFormat.Improved,
                    new[] { day(now, groupA, groupB) },
                    withExpanded(groupA),
                    new[] { logOf(now.DateTime, "Today", "0:00:14", expanded(groupA, DurationFormat.Improved).Concat(collapsed(groupB, DurationFormat.Improved))) }
                },
                new object[]
                {
                    DurationFormat.Improved,
                    new[] { day(now, singleItemGroup) },
                    withExpanded(groupB),
                    new[] { logOf(now.DateTime, "Today", "0:00:01", single(singleItemGroup.First(), DurationFormat.Improved)) }
                },
                new object[]
                {
                    DurationFormat.Classic,
                    new[] { day(now, groupA, singleItemGroup, groupB) },
                    withExpanded(),
                    new[]
                    {
                        logOf(
                            now.DateTime,
                            "Today",
                            "15 sec",
                            collapsed(groupA, DurationFormat.Classic)
                                .Concat(single(singleItemGroup.First(), DurationFormat.Classic))
                                .Concat(collapsed(groupB, DurationFormat.Classic)))
                    }
                },
                new object[]
                {
                    DurationFormat.Classic,
                    new[] { day(now, groupA, singleItemGroup, groupB) },
                    withExpanded(groupA),
                    new[]
                    {
                        logOf(
                            now.DateTime,
                            "Today",
                            "15 sec",
                            expanded(groupA, DurationFormat.Classic)
                                .Concat(single(singleItemGroup.First(), DurationFormat.Classic))
                                .Concat(collapsed(groupB, DurationFormat.Classic)))
                    }
                },
                new object[]
                {
                    DurationFormat.Classic,
                    new[] { day(now, groupA, singleItemGroup, groupB) },
                    withExpanded(groupB),
                    new[]
                    {
                        logOf(
                            now.DateTime,
                            "Today",
                            "15 sec",
                            collapsed(groupA, DurationFormat.Classic)
                                .Concat(single(singleItemGroup.First(), DurationFormat.Classic))
                                .Concat(expanded(groupB, DurationFormat.Classic)))
                    }
                },
                new object[]
                {
                    DurationFormat.Improved,
                    new[] { day(now, groupA, singleItemGroup, groupB) },
                    withExpanded(groupA, groupB),
                    new[]
                    {
                        logOf(
                            now.DateTime,
                            "Today",
                            "0:00:15",
                            expanded(groupA, DurationFormat.Improved)
                                .Concat(single(singleItemGroup.First(), DurationFormat.Improved))
                                .Concat(expanded(groupB, DurationFormat.Improved)))
                    }
                },
                new object[]
                {
                    DurationFormat.Classic,
                    new[] { day(now, twoWorkspaces) },
                    withExpanded(),
                    new[]
                    {
                        logOf(
                            now.DateTime,
                            "Today",
                            "03 sec",
                            single(twoWorkspaces[0], DurationFormat.Classic)
                                .Concat(single(twoWorkspaces[1], DurationFormat.Classic))
                        )
                    }
                },
                new object[]
                {
                    DurationFormat.Improved,
                    new[] { day(now, differentDescriptions) },
                    withExpanded(),
                    new[]
                    {
                        logOf(
                            now.DateTime,
                            "Today",
                            "0:00:07",
                            collapsed(differentDescriptions.Take(2).ToArray(), DurationFormat.Improved)
                                .Concat(single(differentDescriptions[2], DurationFormat.Improved))
                            )
                    }
                },
                new object[]
                {
                    DurationFormat.Decimal,
                    new[] { day(now, longDuration) },
                    withExpanded(),
                    new[]
                    {
                        logOf(
                            now.DateTime,
                            "Today",
                            "07.50 h",
                            collapsed(longDuration.Take(2).ToArray(), DurationFormat.Decimal)
                                .Concat(single(longDuration[2], DurationFormat.Decimal))
                            )
                    }
                }
            };

        private static HashSet<GroupId> withExpanded(params IThreadSafeTimeEntry[][] groups)
        {
            var set = new HashSet<GroupId>();
            foreach (var group in groups)
            {
                var groupId = new GroupId(group.First());
                set.Add(groupId);
            }

            return set;
        }

        private static IThreadSafeTimeEntry createTimeEntry(
            DateTimeOffset start,
            IThreadSafeWorkspace workspace,
            string description,
            long duration,
            IThreadSafeProject project = null,
            IThreadSafeTask task = null,
            IThreadSafeTag[] tags = null,
            bool billable = false)
            => new MockTimeEntry
            {
                Start = start,
                Workspace = workspace,
                WorkspaceId = workspace.Id,
                Description = description,
                Duration = duration,
                Project = project,
                ProjectId = project?.Id,
                Task = task,
                TaskId = task?.Id,
                Billable = billable,
                Tags = tags ?? Array.Empty<IThreadSafeTag>(),
                TagIds = tags?.Select(tag => tag.Id) ?? new long[0]
            };

        private static IGrouping<DateTime, IThreadSafeTimeEntry> day(
            DateTimeOffset date,
            params IThreadSafeTimeEntry[][] groups)
            => groups
                .Flatten()
                .GroupBy(_ => date.DateTime)
                .First();

        private static IThreadSafeTimeEntry[] group(params IThreadSafeTimeEntry[] timeEntries) => timeEntries;

        private static AnimatableSectionModel<DaySummaryViewModel, LogItemViewModel, IMainLogKey> logOf(
            DateTime date,
            string title,
            string trackedTime,
            IEnumerable<LogItemViewModel> items)
            => new AnimatableSectionModel<DaySummaryViewModel, LogItemViewModel, IMainLogKey>(
                new DaySummaryViewModel(date, title, trackedTime), items);

        private static IEnumerable<LogItemViewModel> single(IThreadSafeTimeEntry timeEntry, DurationFormat durationFormat)
        {
            yield return timeEntry.ToViewModel(
                new GroupId(timeEntry),
                LogItemVisualizationIntent.SingleItem,
                durationFormat,
                0,
                0,
                0);
        }

        private static IEnumerable<LogItemViewModel> collapsed(IThreadSafeTimeEntry[] group, DurationFormat durationFormat)
        {
            yield return header(group, LogItemVisualizationIntent.CollapsedGroupHeader, durationFormat);
        }

        private static IEnumerable<LogItemViewModel> expanded(IThreadSafeTimeEntry[] group, DurationFormat durationFormat)
        {
            yield return header(group, LogItemVisualizationIntent.ExpandedGroupHeader, durationFormat);
            foreach (var timeEntry in group)
            {
                yield return groupItem(timeEntry, durationFormat);
            }
        }

        private static LogItemViewModel header(
            IThreadSafeTimeEntry[] group,
            LogItemVisualizationIntent visualizationIntent,
            DurationFormat durationFormat)
        {
            var sample = group.First();
            return new LogItemViewModel(
                groupId: new GroupId(sample),
                representedTimeEntriesIds: group.Select(timeEntry => timeEntry.Id).ToArray(),
                visualizationIntent: visualizationIntent,
                isBillable: sample.Billable,
                isActive: sample.Project?.Active ?? true,
                description: sample.Description,
                duration: DurationAndFormatToString.Convert(
                    TimeSpan.FromSeconds(group.Sum(timeEntry => timeEntry.Duration ?? 0)),
                    durationFormat),
                projectName: sample.Project?.Name,
                projectColor: sample.Project?.Color,
                clientName: sample.Project?.Client?.Name,
                taskName: sample.Task?.Name,
                hasTags: sample.Tags.Any(),
                needsSync: group.Any(timeEntry => timeEntry.SyncStatus == SyncStatus.SyncNeeded),
                canSync: group.All(timeEntry => timeEntry.SyncStatus != SyncStatus.SyncFailed),
                isInaccessible: sample.IsInaccessible,
                indexInLog: 0,
                dayInLog: 0,
                daysInThePast: 0,
                projectIsPlaceholder: sample.Project?.IsPlaceholder() ?? false,
                taskIsPlaceholder: sample.Task?.IsPlaceholder() ?? false);
        }

        private static LogItemViewModel groupItem(IThreadSafeTimeEntry timeEntry, DurationFormat durationFormat)
            => timeEntry.ToViewModel(
                new GroupId(timeEntry),
                LogItemVisualizationIntent.GroupItem,
                durationFormat,
                0,
                0,
                0);
    }
}
