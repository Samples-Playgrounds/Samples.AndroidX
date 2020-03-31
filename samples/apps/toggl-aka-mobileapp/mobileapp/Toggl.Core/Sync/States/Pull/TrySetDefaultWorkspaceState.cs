using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Core.Exceptions;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Sync.States.Pull
{
    public sealed class TrySetDefaultWorkspaceState : ISyncState
    {
        private readonly ITimeService timeService;
        private readonly ITogglDataSource dataSource;

        public StateResult Done { get; } = new StateResult();

        public TrySetDefaultWorkspaceState(ITimeService timeService, ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;
            this.timeService = timeService;
        }

        public IObservable<ITransition> Start()
            => dataSource
                .Workspaces
                .GetAll()
                .Select(getDefaulWorkspaceIfPossible)
                .SelectMany(setDefaultWorkspace)
                .Select(_ => Done.Transition());

        private IThreadSafeWorkspace getDefaulWorkspaceIfPossible(IEnumerable<IThreadSafeWorkspace> workspaces)
            => workspaces.Count() == 1
                ? workspaces.First()
                : throw new NoDefaultWorkspaceException();

        private IObservable<Unit> setDefaultWorkspace(IThreadSafeWorkspace workspace)
            => new SetDefaultWorkspaceInteractor(timeService, dataSource.User, workspace.Id).Execute();
    }
}
