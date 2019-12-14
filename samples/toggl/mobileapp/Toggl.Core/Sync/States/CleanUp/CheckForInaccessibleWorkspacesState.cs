using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.Core.Sync.States.CleanUp
{
    public sealed class CheckForInaccessibleWorkspacesState : ISyncState
    {
        private readonly ITogglDataSource dataSource;

        public StateResult NoInaccessibleWorkspaceFound { get; } = new StateResult();
        public StateResult FoundInaccessibleWorkspaces { get; } = new StateResult();

        public CheckForInaccessibleWorkspacesState(ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;
        }

        public IObservable<ITransition> Start()
            => dataSource
                .Workspaces
                .GetAll(ws => ws.IsInaccessible, includeInaccessibleEntities: true)
                .Select(data => data.Any())
                .Select(toTransition);

        private ITransition toTransition(bool hasInaccessibleWorkspaces)
            => hasInaccessibleWorkspaces
                ? FoundInaccessibleWorkspaces.Transition()
                : NoInaccessibleWorkspaceFound.Transition();
    }
}
