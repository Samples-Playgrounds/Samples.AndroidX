﻿using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Extensions;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Sync.States.CleanUp
{
    public sealed class TrackInaccessibleWorkspacesAfterCleanUpState : ISyncState
    {
        private readonly ITogglDataSource dataSource;
        private readonly IAnalyticsService analyticsService;

        public StateResult Done { get; } = new StateResult();

        public TrackInaccessibleWorkspacesAfterCleanUpState(ITogglDataSource dataSource, IAnalyticsService analyticsService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));

            this.dataSource = dataSource;
            this.analyticsService = analyticsService;
        }

        public IObservable<ITransition> Start()
            => dataSource
                .Workspaces
                .GetAll(ws => ws.IsInaccessible, includeInaccessibleEntities: true)
                .Select(data => data.Count())
                .Track(analyticsService.WorkspacesInaccesibleAfterCleanUp)
                .SelectValue(Done.Transition());
    }
}
