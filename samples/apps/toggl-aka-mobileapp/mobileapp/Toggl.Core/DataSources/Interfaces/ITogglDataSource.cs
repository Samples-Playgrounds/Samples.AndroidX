using System;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage.Models;

namespace Toggl.Core.DataSources
{
    public interface ITogglDataSource
    {
        ITimeEntriesSource TimeEntries { get; }
        ISingletonDataSource<IThreadSafeUser> User { get; }
        IDataSource<IThreadSafeTag, IDatabaseTag> Tags { get; }
        IDataSource<IThreadSafeTask, IDatabaseTask> Tasks { get; }
        IDataSource<IThreadSafeClient, IDatabaseClient> Clients { get; }
        ISingletonDataSource<IThreadSafePreferences> Preferences { get; }
        IDataSource<IThreadSafeProject, IDatabaseProject> Projects { get; }
        IObservableDataSource<IThreadSafeWorkspace, IDatabaseWorkspace> Workspaces { get; }
        IDataSource<IThreadSafeWorkspaceFeatureCollection, IDatabaseWorkspaceFeatureCollection> WorkspaceFeatures { get; }

        IObservable<bool> HasUnsyncedData();
    }
}
