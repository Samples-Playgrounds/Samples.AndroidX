using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Exceptions;

namespace Toggl.Core.Sync.States.Pull
{
    internal sealed class DetectNotHavingAccessToAnyWorkspaceState : ISyncState<IFetchObservables>
    {
        public StateResult<IFetchObservables> Done { get; } = new StateResult<IFetchObservables>();

        private readonly ITogglDataSource dataSource;
        private readonly IAnalyticsService analyticsService;

        public DetectNotHavingAccessToAnyWorkspaceState(
            ITogglDataSource dataSource,
            IAnalyticsService analyticsService)
        {
            this.dataSource = dataSource;
            this.analyticsService = analyticsService;
        }

        public IObservable<ITransition> Start(IFetchObservables fetch)
            => dataSource.Workspaces.GetAll()
                .Select(ws => ws.Any())
                .Do(trackNoWorkspacesNeeded)
                .Select(userHasAccessToAnyWorkspace => userHasAccessToAnyWorkspace
                    ? Done.Transition(fetch)
                    : throw new NoWorkspaceException());

        private void trackNoWorkspacesNeeded(bool userHasAccessToAnyWorkspace)
        {
            if (userHasAccessToAnyWorkspace)
                return;

            analyticsService.NoWorkspaces.Track();
        }
    }
}
