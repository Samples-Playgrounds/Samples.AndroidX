using System;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Models;

namespace Toggl.Core.Sync.States.Pull
{
    public sealed class MarkWorkspacesAsInaccessibleState : ISyncState<MarkWorkspacesAsInaccessibleParams>
    {
        private readonly IDataSource<IThreadSafeWorkspace, IDatabaseWorkspace> dataSource;

        public StateResult<IFetchObservables> Done { get; } = new StateResult<IFetchObservables>();

        public MarkWorkspacesAsInaccessibleState(IDataSource<IThreadSafeWorkspace, IDatabaseWorkspace> dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            this.dataSource = dataSource;
        }

        public IObservable<ITransition> Start(MarkWorkspacesAsInaccessibleParams stateParams)
        {
            var workspaces = stateParams.Workspaces;
            var fetchObservables = stateParams.FetchObservables;

            return Observable.Return(workspaces)
                .Flatten()
                .SelectMany(markAsInaccessible)
                .ToList()
                .SelectValue(Done.Transition(fetchObservables));
        }

        private IObservable<IThreadSafeWorkspace> markAsInaccessible(IThreadSafeWorkspace workspaceToMark)
            => dataSource.Update(workspaceToMark.AsInaccessible());

    }
}
