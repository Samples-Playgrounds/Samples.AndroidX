using System;
using Toggl.Shared.Models;

namespace Toggl.iOS.Shared.Models
{
    public class TimeEntryViewModel
    {
        public ITimeEntry TimeEntry { get; }

        public string Description => TimeEntry.Description;

        public DateTimeOffset StartTime => TimeEntry.Start;

        public string ProjectName { get; }

        public string ProjectColor { get; }

        public string TaskName { get; }

        public string ClientName { get; }

        public TimeEntryViewModel(ITimeEntry timeEntry, string projectName, string projectColor, string taskName, string clientName)
        {
            TimeEntry = timeEntry;
            ProjectName = projectName;
            ProjectColor = projectColor;
            TaskName = taskName;
            ClientName = clientName;
        }
    }
}
