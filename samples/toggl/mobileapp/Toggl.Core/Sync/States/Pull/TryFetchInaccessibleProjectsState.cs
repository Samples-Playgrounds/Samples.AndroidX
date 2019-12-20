﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Extensions;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Networking.ApiClients;
using Toggl.Networking.Exceptions;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Sync.States.Pull
{
    public sealed class TryFetchInaccessibleProjectsState : IPersistState
    {
        private readonly IDataSource<IThreadSafeProject, IDatabaseProject> dataSource;

        private readonly ITimeService timeService;

        private readonly IProjectsApi projectsApi;

        public StateResult<IFetchObservables> FetchNext { get; } = new StateResult<IFetchObservables>();

        public StateResult<IFetchObservables> Done { get; } = new StateResult<IFetchObservables>();

        public StateResult<ApiException> ErrorOccured { get; } = new StateResult<ApiException>();

        private DateTimeOffset yesterdayThisTime => timeService.CurrentDateTime.AddDays(-1);

        public TryFetchInaccessibleProjectsState(
            IDataSource<IThreadSafeProject, IDatabaseProject> dataSource,
            ITimeService timeService,
            IProjectsApi projectsApi)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(projectsApi, nameof(projectsApi));

            this.dataSource = dataSource;
            this.timeService = timeService;
            this.projectsApi = projectsApi;
        }

        public IObservable<ITransition> Start(IFetchObservables fetch)
            => getProjectsWhichNeedsRefetching()
                .SelectMany(projects => projects == null
                    ? Observable.Return(Done.Transition(fetch))
                    : refetch(projects).SelectValue(FetchNext.Transition(fetch)))
                .OnErrorReturnResult(ErrorOccured);

        private IObservable<IGrouping<long, IThreadSafeProject>> getProjectsWhichNeedsRefetching()
            => dataSource.GetAll(project =>
                    project.SyncStatus == SyncStatus.RefetchingNeeded && project.At < yesterdayThisTime)
                .SelectMany(projects =>
                    projects.GroupBy(project => project.WorkspaceId).OrderBy(group => group.Key))
                .FirstOrDefaultAsync();

        private IObservable<Unit> refetch(IGrouping<long, IThreadSafeProject> projectsToFetch)
            => projectsApi.Search(
                    workspaceId: projectsToFetch.Key,
                    projectIds: projectsToFetch.Select(p => p.Id).ToArray())
                .ToObservable()
                .Flatten()
                .Select(Project.Clean)
                .SelectMany(dataSource.Update)
                .ToList()
                .SelectMany(projectsWhichWereNotFetched(projectsToFetch))
                .SelectMany(updateAtValue)
                .ToList()
                .SelectUnit();

        private Func<IList<IThreadSafeProject>, IEnumerable<IThreadSafeProject>> projectsWhichWereNotFetched(IEnumerable<IThreadSafeProject> searchedProjects)
            => foundProjects => searchedProjects.Where(
                searchedProject => foundProjects.None(foundProject => foundProject.Id == searchedProject.Id));

        private IObservable<IThreadSafeProject> updateAtValue(IThreadSafeProject project)
        {
            var updatedProject = Project.Builder.From(project)
                .SetAt(timeService.CurrentDateTime)
                .Build();

            return dataSource.Update(updatedProject);
        }
    }
}
