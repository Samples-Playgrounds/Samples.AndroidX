using Toggl.Networking.ApiClients;
using Toggl.Networking.ApiClients.Interfaces;

namespace Toggl.Networking
{
    public interface ITogglApi
    {
        ITagsApi Tags { get; }
        IUserApi User { get; }
        ITasksApi Tasks { get; }
        IStatusApi Status { get; }
        IClientsApi Clients { get; }
        ILocationApi Location { get; }
        IProjectsApi Projects { get; }
        ICountriesApi Countries { get; }
        IWorkspacesApi Workspaces { get; }
        IPreferencesApi Preferences { get; }
        ITimeEntriesApi TimeEntries { get; }
        ITimeEntriesReportsApi TimeEntriesReports { get; }
        IProjectsSummaryApi ProjectsSummary { get; }
        IPushServicesApi PushServices { get; }
        IWorkspaceFeaturesApi WorkspaceFeatures { get; }
        IFeedbackApi Feedback { get; }
        ITimeZonesApi Timezones { get; }
    }
}
