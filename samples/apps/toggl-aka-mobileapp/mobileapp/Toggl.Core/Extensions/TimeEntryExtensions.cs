using System;
using System.Linq;
using Toggl.Core.Autocomplete;
using Toggl.Core.Autocomplete.Span;
using Toggl.Core.Calendar;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;

namespace Toggl.Core.Extensions
{
    public static class TimeEntryExtensions
    {
        public static ITimeEntryPrototype AsTimeEntryPrototype(
            this string description,
            DateTimeOffset startTime,
            long workspaceId,
            TimeSpan? duration = null,
            long? projectId = null,
            long? taskId = null,
            long[] tagIds = null,
            bool isBillable = false)
            => new TimeEntryPrototype(
                workspaceId,
                description: description,
                duration: duration,
                startTime: startTime,
                projectId: projectId,
                taskId: taskId,
                tagIds: tagIds,
                isBillable: isBillable
            );

        public static ITimeEntryPrototype AsTimeEntryPrototype(this CalendarItem calendarItem, long workspaceId)
            => new TimeEntryPrototype(
                workspaceId,
                description: calendarItem.Description,
                duration: calendarItem.Duration,
                startTime: calendarItem.StartTime,
                projectId: null,
                taskId: null,
                tagIds: null,
                isBillable: false
            );

        public static ITimeEntryPrototype AsTimeEntryPrototype(this TimeSpan timeSpan, DateTimeOffset startTime,
            long workspaceId)
            => new TimeEntryPrototype(
                workspaceId,
                description: "",
                duration: timeSpan,
                startTime: startTime,
                projectId: null,
                taskId: null,
                tagIds: null,
                isBillable: false
            );

        public static ITimeEntryPrototype AsTimeEntryPrototype(this TextFieldInfo textFieldInfo,
            DateTimeOffset startTime, TimeSpan? duration, bool billable)
            => new TimeEntryPrototype(
                textFieldInfo.WorkspaceId,
                textFieldInfo.Description,
                duration,
                startTime,
                textFieldInfo.ProjectId,
                textFieldInfo.Spans.OfType<ProjectSpan>().SingleOrDefault()?.TaskId,
                textFieldInfo.Spans.OfType<TagSpan>().Select(span => span.TagId).Distinct().ToArray(),
                billable
            );


        public static ITimeEntryPrototype AsTimeEntryPrototype(this IThreadSafeTimeEntry timeEntry)
            => new TimeEntryPrototype(
                timeEntry.WorkspaceId,
                timeEntry.Description,
                TimeSpan.FromSeconds(timeEntry.Duration ?? 0.0),
                timeEntry.Start,
                timeEntry.Project?.Id,
                timeEntry.Task?.Id,
                timeEntry.TagIds.ToArray(),
                timeEntry.Billable);


        public static ITimeEntryPrototype AsRunningTimeEntryPrototype(this IThreadSafeTimeEntry timeEntry, DateTimeOffset now)
            => new TimeEntryPrototype(
                timeEntry.WorkspaceId,
                timeEntry.Description,
                null,
                now,
                timeEntry.Project?.Id,
                timeEntry.Task?.Id,
                timeEntry.TagIds.ToArray(),
                timeEntry.Billable);

        private sealed class TimeEntryPrototype : ITimeEntryPrototype
        {
            public long WorkspaceId { get; }

            public string Description { get; }

            public TimeSpan? Duration { get; }

            public DateTimeOffset StartTime { get; }

            public long? ProjectId { get; }

            public long? TaskId { get; }

            public long[] TagIds { get; }

            public bool IsBillable { get; }

            public TimeEntryPrototype(
                long workspaceId,
                string description,
                TimeSpan? duration,
                DateTimeOffset startTime,
                long? projectId,
                long? taskId,
                long[] tagIds,
                bool isBillable)
            {
                WorkspaceId = workspaceId;
                Description = description;
                Duration = duration;
                StartTime = startTime;
                ProjectId = projectId;
                TaskId = taskId;
                TagIds = tagIds;
                IsBillable = isBillable;
            }
        }
    }
}
