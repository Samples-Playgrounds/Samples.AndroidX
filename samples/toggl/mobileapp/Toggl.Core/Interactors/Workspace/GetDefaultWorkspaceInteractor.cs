﻿using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Interactors
{
    internal sealed class GetDefaultWorkspaceInteractor : TrackableInteractor, IInteractor<IObservable<IThreadSafeWorkspace>>
    {
        private readonly ITogglDataSource dataSource;
        private readonly IInteractorFactory interactorFactory;

        public GetDefaultWorkspaceInteractor(
            ITogglDataSource dataSource,
            IInteractorFactory interactorFactory,
            IAnalyticsService analyticsService) : base(analyticsService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.dataSource = dataSource;
            this.interactorFactory = interactorFactory;
        }

        public IObservable<IThreadSafeWorkspace> Execute()
            => dataSource.User
                .Get()
                .SelectMany(user => user.DefaultWorkspaceId.HasValue
                    ? interactorFactory.GetWorkspaceById(user.DefaultWorkspaceId.Value).Execute()
                    : chooseWorkspace())
                .Catch((InvalidOperationException exception) => chooseWorkspace())
                .Select(Workspace.From);

        private IObservable<IThreadSafeWorkspace> chooseWorkspace()
            => dataSource.Workspaces.GetAll(workspace => !workspace.IsDeleted)
                .Select(workspaces => workspaces.OrderBy(workspace => workspace.Id))
                .SelectMany(workspaces =>
                    workspaces.None()
                        ? Observable.Never<IThreadSafeWorkspace>()
                        : Observable.Return(workspaces.First()));

    }
}
