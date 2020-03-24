using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.DataSources
{
    public sealed class TogglDataSource : ITogglDataSource
    {
        private readonly ITogglDatabase database;

        public TogglDataSource(
            ITogglDatabase database,
            ITimeService timeService,
            IAnalyticsService analyticsService)
        {
            Ensure.Argument.IsNotNull(database, nameof(database));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));

            this.database = database;

            User = new UserDataSource(database.User);
            Tags = new TagsDataSource(database.Tags);
            Tasks = new TasksDataSource(database.Tasks);
            Clients = new ClientsDataSource(database.Clients);
            Projects = new ProjectsDataSource(database.Projects);
            Workspaces = new WorkspacesDataSource(database.Workspaces);
            Preferences = new PreferencesDataSource(database.Preferences);
            WorkspaceFeatures = new WorkspaceFeaturesDataSource(database.WorkspaceFeatures);
            TimeEntries = new TimeEntriesDataSource(database.TimeEntries, timeService, analyticsService);
        }

        public ITimeEntriesSource TimeEntries { get; }

        public ISingletonDataSource<IThreadSafeUser> User { get; }

        public IDataSource<IThreadSafeTag, IDatabaseTag> Tags { get; }

        public IDataSource<IThreadSafeTask, IDatabaseTask> Tasks { get; }

        public IDataSource<IThreadSafeClient, IDatabaseClient> Clients { get; }

        public ISingletonDataSource<IThreadSafePreferences> Preferences { get; }

        public IDataSource<IThreadSafeProject, IDatabaseProject> Projects { get; }

        public IObservableDataSource<IThreadSafeWorkspace, IDatabaseWorkspace> Workspaces { get; }

        public IDataSource<IThreadSafeWorkspaceFeatureCollection, IDatabaseWorkspaceFeatureCollection> WorkspaceFeatures { get; }

        public IObservable<bool> HasUnsyncedData()
            => Observable.Merge(
                hasUnsyncedData(database.TimeEntries),
                hasUnsyncedData(database.Projects),
                hasUnsyncedData(database.User),
                hasUnsyncedData(database.Tasks),
                hasUnsyncedData(database.Clients),
                hasUnsyncedData(database.Tags),
                hasUnsyncedData(database.Workspaces))
                .Any(hasUnsynced => hasUnsynced);

        private IObservable<bool> hasUnsyncedData<TModel>(IBaseStorage<TModel> repository)
            where TModel : IDatabaseSyncable
            => repository
                .GetAll(entity => entity.SyncStatus != SyncStatus.InSync)
                .Select(unsynced => unsynced.Any())
                .SingleAsync();
    }
}
