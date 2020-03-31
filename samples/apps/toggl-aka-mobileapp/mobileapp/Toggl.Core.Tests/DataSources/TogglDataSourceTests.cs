using FluentAssertions;
using NSubstitute;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Models;
using Toggl.Core.Services;
using Toggl.Core.Shortcuts;
using Toggl.Core.Sync;
using Toggl.Shared.Models;
using Toggl.Storage;
using Toggl.Storage.Models;
using Xunit;
using ThreadingTask = System.Threading.Tasks.Task;

namespace Toggl.Core.Tests.DataSources
{
    public sealed class TogglDataSourceTests
    {
        public abstract class TogglDataSourceTest
        {
            protected ITogglDataSource DataSource { get; }
            protected ITogglDatabase Database { get; } = Substitute.For<ITogglDatabase>();
            protected ITimeService TimeService { get; } = Substitute.For<ITimeService>();
            protected ISyncManager SyncManager { get; } = Substitute.For<ISyncManager>();
            protected INotificationService NotificationService { get; } = Substitute.For<INotificationService>();
            protected ISubject<SyncProgress> ProgressSubject { get; } = new Subject<SyncProgress>();
            protected IApplicationShortcutCreator ApplicationShortcutCreator { get; } = Substitute.For<IApplicationShortcutCreator>();
            protected IAnalyticsService AnalyticsService { get; } = Substitute.For<IAnalyticsService>();

            public TogglDataSourceTest()
            {
                SyncManager.ProgressObservable.Returns(ProgressSubject.AsObservable());
                DataSource = new TogglDataSource(
                    Database,
                    TimeService,
                    AnalyticsService);
            }
        }

        public sealed class TheHasUnsyncedDataMetod
        {
            public abstract class BaseHasUnsyncedDataTest<TModel, TDatabaseModel> : TogglDataSourceTest
                where TModel : class
                where TDatabaseModel : TModel, IDatabaseSyncable
            {
                private readonly IBaseStorage<TDatabaseModel> repository;
                private readonly Func<TModel, TDatabaseModel> dirty;
                private readonly Func<TModel, string, TDatabaseModel> unsyncable;

                public BaseHasUnsyncedDataTest(Func<ITogglDatabase, IBaseStorage<TDatabaseModel>> repository,
                    Func<TModel, TDatabaseModel> dirty, Func<TModel, string, TDatabaseModel> unsyncable)
                {
                    this.repository = repository(Database);
                    this.dirty = dirty;
                    this.unsyncable = unsyncable;
                }

                [Fact, LogIfTooSlow]
                public async ThreadingTask ReturnsTrueWhenThereIsAnEntityWhichNeedsSync()
                {
                    var dirtyEntity = dirty(Substitute.For<TModel>());
                    repository.GetAll(Arg.Any<Func<TDatabaseModel, bool>>())
                        .Returns(Observable.Return(new[] { dirtyEntity }));

                    var hasUnsyncedData = await DataSource.HasUnsyncedData();

                    hasUnsyncedData.Should().BeTrue();
                }

                [Fact, LogIfTooSlow]
                public async ThreadingTask ReturnsTrueWhenThereIsAnEntityWhichFailedToSync()
                {
                    var unsyncableEntity = unsyncable(Substitute.For<TModel>(), "Error message.");
                    repository.GetAll(Arg.Any<Func<TDatabaseModel, bool>>())
                        .Returns(Observable.Return(new[] { unsyncableEntity }));

                    var hasUnsyncedData = await DataSource.HasUnsyncedData();

                    hasUnsyncedData.Should().BeTrue();
                }

                [Fact, LogIfTooSlow]
                public async ThreadingTask ReturnsFalseWhenThereIsNoUnsyncedEntityAndAllOtherRepositoriesAreSyncedAsWell()
                {
                    repository.GetAll(Arg.Any<Func<TDatabaseModel, bool>>())
                        .Returns(Observable.Return(new TDatabaseModel[0]));

                    var hasUnsyncedData = await DataSource.HasUnsyncedData();

                    hasUnsyncedData.Should().BeFalse();
                }
            }

            public sealed class TimeEntriesTest : BaseHasUnsyncedDataTest<ITimeEntry, IDatabaseTimeEntry>
            {
                public TimeEntriesTest()
                    : base(database => database.TimeEntries, TimeEntry.Dirty, TimeEntry.Unsyncable)
                {
                }
            }

            public sealed class ProjectsTest : BaseHasUnsyncedDataTest<IProject, IDatabaseProject>
            {
                public ProjectsTest()
                    : base(database => database.Projects, Project.Dirty, Project.Unsyncable)
                {
                }
            }

            public sealed class UserTest : BaseHasUnsyncedDataTest<IUser, IDatabaseUser>
            {
                public UserTest()
                    : base(database => database.User, User.Dirty, User.Unsyncable)
                {
                }
            }

            public sealed class TasksTest : BaseHasUnsyncedDataTest<ITask, IDatabaseTask>
            {
                public TasksTest()
                    : base(database => database.Tasks, Task.Dirty, Task.Unsyncable)
                {
                }
            }

            public sealed class ClientsTest : BaseHasUnsyncedDataTest<IClient, IDatabaseClient>
            {
                public ClientsTest()
                    : base(database => database.Clients, Client.Dirty, Client.Unsyncable)
                {
                }
            }

            public sealed class TagsTest : BaseHasUnsyncedDataTest<ITag, IDatabaseTag>
            {
                public TagsTest()
                    : base(database => database.Tags, Tag.Dirty, Tag.Unsyncable)
                {
                }
            }

            public sealed class WorkspacesTest : BaseHasUnsyncedDataTest<IWorkspace, IDatabaseWorkspace>
            {
                public WorkspacesTest()
                    : base(database => database.Workspaces, Workspace.Dirty, Workspace.Unsyncable)
                {
                }
            }
        }
    }
}
