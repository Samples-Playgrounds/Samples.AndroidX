using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Shared;

namespace Toggl.Core.Sync.States.Pull
{
    internal sealed class DetectUserHavingNoDefaultWorkspaceSetState : ISyncState
    {
        private readonly ITogglDataSource dataSource;
        private readonly IAnalyticsService analyticsService;

        public StateResult Done { get; } = new StateResult();
        public StateResult NoDefaultWorkspaceDetected { get; } = new StateResult();

        public DetectUserHavingNoDefaultWorkspaceSetState(
            ITogglDataSource dataSource,
            IAnalyticsService analyticsService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));

            this.dataSource = dataSource;
            this.analyticsService = analyticsService;
        }

        public IObservable<ITransition> Start()
            => dataSource.User.Get()
                .Select(user => user.DefaultWorkspaceId.HasValue)
                .SelectMany(transitionToNextState);

        private IObservable<ITransition> transitionToNextState(bool userHasDefaultWorkspace)
            => userHasDefaultWorkspace
                ? Observable.Return(Done.Transition())
                : dataSource.Workspaces.GetAll()
                    .Select(workspaces =>
                    {
                        var numberOfWorkspaces = workspaces.Count();
                        analyticsService.NoDefaultWorkspace
                            .Track(numberOfWorkspaces);

                        return NoDefaultWorkspaceDetected.Transition();
                    });
    }
}
