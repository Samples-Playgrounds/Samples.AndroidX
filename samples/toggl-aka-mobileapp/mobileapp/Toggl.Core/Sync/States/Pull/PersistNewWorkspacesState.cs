using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Storage.Models;

namespace Toggl.Core.Sync.States.Pull
{
    public class PersistNewWorkspacesState : ISyncState<IEnumerable<IWorkspace>>
    {
        private readonly IDataSource<IThreadSafeWorkspace, IDatabaseWorkspace> dataSource;

        public StateResult Done { get; } = new StateResult();

        public PersistNewWorkspacesState(IDataSource<IThreadSafeWorkspace, IDatabaseWorkspace> dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            this.dataSource = dataSource;
        }

        public IObservable<ITransition> Start(IEnumerable<IWorkspace> workspaces)
            => Observable.Return(workspaces)
                .Flatten()
                .Select(Workspace.Clean)
                .SelectMany(createOrUpdate)
                .ToList()
                .SelectValue(Done.Transition());

        private IObservable<IThreadSafeWorkspace> createOrUpdate(IThreadSafeWorkspace workspace)
            => dataSource
                .GetAll(ws => ws.Id == workspace.Id, includeInaccessibleEntities: true)
                .SelectMany(stored => stored.None()
                    ? dataSource.Create(workspace)
                    : dataSource.Update(workspace));
    }
}
