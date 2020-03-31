using Toggl.Core.Calendar;

namespace Toggl.Core.UI.ViewModels.Calendar.ContextualMenu
{
    public struct TimeEntryDisplayInfo
    {
        public string Description { get; }
        public string Project { get; }
        public string Task { get; }
        public string Client { get; }
        public string ProjectTaskColor { get; }

        public TimeEntryDisplayInfo(CalendarItem calendarItem)
        {
            Description = calendarItem.Description;
            Project = calendarItem.Project;
            Task = calendarItem.Task;
            Client = calendarItem.Client;
            ProjectTaskColor = calendarItem.Color;
        }
    }
}