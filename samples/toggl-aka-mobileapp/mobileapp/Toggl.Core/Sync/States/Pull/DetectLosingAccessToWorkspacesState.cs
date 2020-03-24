using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Storage.Models;

namespace Toggl.Core.Sync.States.Pull
{
    public sealed class DetectLosingAccessToWorkspacesState : ISyncState<IFetchObservables>
    {
        private readonly IDataSource<IThreadSafeWorkspace, IDatabaseWorkspace> dataSource;
        private readonly IAnalyticsService analyticsService;

        public StateResult<MarkWorkspacesAsInaccessibleParams> WorkspaceAccessLost { get; } = new StateResult<MarkWorkspacesAsInaccessibleParams>();

        public StateResult<IFetchObservables> Done { get; } = new StateResult<IFetchObservables>();

        public DetectLosingAccessToWorkspacesState(IDataSource<IThreadSafeWorkspace, IDatabaseWorkspace> dataSource, IAnalyticsService analyticsService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));

            this.dataSource = dataSource;
            this.analyticsService = analyticsService;
        }

        public IObservable<ITransition> Start(IFetchObservables fetchObservables)
            => fetchObservables.GetList<IWorkspace>()
                .SelectMany(workspacesWhichWereNotFetched)
                .Select(lostWorkspaces => lostWorkspaces.Any()
                    ? processLostWorkspaces(lostWorkspaces, fetchObservables)
                    : Done.Transition(fetchObservables));

        private IObservable<IList<IThreadSafeWorkspace>> workspacesWhichWereNotFetched(List<IWorkspace> fetchedWorkspaces)
            => allStoredWorkspaces()
                .Where(stored => fetchedWorkspaces.None(fetched => fetched.Id == stored.Id))
                .ToList();

        private IObservable<IThreadSafeWorkspace> allStoredWorkspaces()
            => dataSource.GetAll(ws => ws.Id > 0 && ws.IsInaccessible == false)
                         .Flatten();

        private ITransition processLostWorkspaces(
            IEnumerable<IThreadSafeWorkspace> workspaces,
            IFetchObservables fetchObservables)
        {
            analyticsService.LostWorkspaceAccess.Track();
            var stateParams = new MarkWorkspacesAsInaccessibleParams(workspaces, fetchObservables);

            return WorkspaceAccessLost.Transition(stateParams);
        }
    }
}
