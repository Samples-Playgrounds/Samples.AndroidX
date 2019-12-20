namespace Toggl.Core.Analytics
{
    public enum ContinueTimeEntryMode
    {
        SingleTimeEntrySwipe = TimeEntryStartOrigin.SingleTimeEntrySwipe,
        SingleTimeEntryContinueButton = TimeEntryStartOrigin.SingleTimeEntryContinueButton,
        TimeEntriesGroupSwipe = TimeEntryStartOrigin.TimeEntriesGroupSwipe,
        TimeEntriesGroupContinueButton = TimeEntryStartOrigin.TimeEntriesGroupContinueButton,
        CalendarContextualMenu = TimeEntryStartOrigin.CalendarEvent
    }
}
