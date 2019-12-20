using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Extensions;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Storage.Models;
using Toggl.Storage.Settings;
using static Toggl.Shared.Extensions.CommonFunctions;

namespace Toggl.Core.Sync.States.PullTimeEntries
{
    public sealed class CreatePlaceholdersState<TThreadSafe, TDatabase> : ISyncState<IFetchObservables>
        where TDatabase : IDatabaseModel, IIdentifiable
        where TThreadSafe : TDatabase, IThreadSafeModel
    {
        private readonly IDataSource<TThreadSafe, TDatabase> dataSource;
        private readonly IAnalyticsEvent<int> analyticsEvent;
        private readonly Func<ITimeEntry, long[]> dependencyIdsSelector;
        private readonly Func<long, long, TThreadSafe> buildDependencyPlaceholder;
        private readonly ILastTimeUsageStorage lastTimeUsageStorage;
        private readonly ITimeService timeService;

        public StateResult<IFetchObservables> Done { get; } = new StateResult<IFetchObservables>();

        public CreatePlaceholdersState(
            IDataSource<TThreadSafe, TDatabase> dataSource,
            ILastTimeUsageStorage lastTimeUsageStorage,
            ITimeService timeService,
            IAnalyticsEvent<int> analyticsEvent,
            Func<ITimeEntry, long[]> dependencyIdsSelector,
            Func<long, long, TThreadSafe> buildDependencyPlaceholder)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(lastTimeUsageStorage, nameof(lastTimeUsageStorage));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(analyticsEvent, nameof(analyticsEvent));
            Ensure.Argument.IsNotNull(dependencyIdsSelector, nameof(dependencyIdsSelector));
            Ensure.Argument.IsNotNull(buildDependencyPlaceholder, nameof(buildDependencyPlaceholder));

            this.dataSource = dataSource;
            this.lastTimeUsageStorage = lastTimeUsageStorage;
            this.timeService = timeService;
            this.analyticsEvent = analyticsEvent;
            this.dependencyIdsSelector = dependencyIdsSelector;
            this.buildDependencyPlaceholder = buildDependencyPlaceholder;
        }

        public IObservable<ITransition> Start(IFetchObservables fetch)
            => fetch.GetList<ITimeEntry>()
                .SingleAsync()
                .SelectMany(Identity)
                .WhereAsync(hasUnknownDependency)
                .SelectMany(timeEntry =>
                    dependencyIdsSelector(timeEntry)
                        .Select(id => (workspaceId: timeEntry.WorkspaceId, dependencyId: id)))
                .Distinct(tuple => tuple.dependencyId)
                .Select(tuple => buildDependencyPlaceholder(tuple.dependencyId, tuple.workspaceId))
                .SelectMany(dataSource.Create)
                .Count()
                .DoIf(
                    numberOfPlaceholders => numberOfPlaceholders > 0,
                    _ => lastTimeUsageStorage.SetPlaceholdersWereCreated(timeService.CurrentDateTime))
                .Track(analyticsEvent)
                .SelectValue(Done.Transition(fetch));

        private IObservable<bool> hasUnknownDependency(ITimeEntry timeEntry)
            => dependencyIdsSelector(timeEntry).Length > 0
                ? dataSource.GetByIds(dependencyIdsSelector(timeEntry))
                    .SingleAsync()
                    .Select(entities => entities.None())
                : Observable.Return(false);
    }
}
