using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;

namespace Toggl.Core.Analytics
{
    public class StartTimeEntryEvent : ITrackableEvent
    {
        public string EventName => "TimeEntryStarted";

        public TimeEntryStartOrigin Origin { get; }
        public bool HasEmptyDescription { get; }
        public bool HasProject { get; }
        public bool HasTask { get; }
        public int NumberOfTags { get; }
        public bool IsBillable { get; }
        public bool IsRunning { get; }

        public StartTimeEntryEvent(
            TimeEntryStartOrigin origin,
            bool hasEmptyDescription,
            bool hasProject,
            bool hasTask,
            int numberOfTags,
            bool isBillable,
            bool isRunning)
        {
            Origin = origin;
            HasEmptyDescription = hasEmptyDescription;
            HasProject = hasProject;
            HasTask = hasTask;
            NumberOfTags = numberOfTags;
            IsBillable = isBillable;
            IsRunning = isRunning;
        }

        public static Func<IThreadSafeTimeEntry, StartTimeEntryEvent> With(TimeEntryStartOrigin origin)
            => timeEntry => With(origin, timeEntry);

        public static StartTimeEntryEvent With(TimeEntryStartOrigin origin, ITimeEntry timeEntry)
            => new StartTimeEntryEvent(
                origin,
                string.IsNullOrWhiteSpace(timeEntry.Description),
                timeEntry.ProjectId != null,
                timeEntry.TaskId != null,
                timeEntry.TagIds.Count(),
                timeEntry.Billable,
                timeEntry.IsRunning());

        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>
            {
                [nameof(Origin)] = Origin.ToString(),
                [nameof(HasEmptyDescription)] = HasEmptyDescription.ToString(),
                [nameof(HasProject)] = HasProject.ToString(),
                [nameof(HasTask)] = HasTask.ToString(),
                [nameof(NumberOfTags)] = NumberOfTags.ToString(),
                [nameof(IsBillable)] = IsBillable.ToString(),
                [nameof(IsRunning)] = IsRunning.ToString()
            };
        }
    }
}
