using System;
using System.Reactive;
using Toggl.Core.DataSources;
using Toggl.Shared;

namespace Toggl.Core.Interactors
{
    internal sealed class ObserveWorkspacesChangesInteractor : IInteractor<IObservable<Unit>>
    {
        private readonly ITogglDataSource dataSource;

        public ObserveWorkspacesChangesInteractor(ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;
        }

        public IObservable<Unit> Execute()
            => dataSource.Workspaces.ItemsChanged;
    }
}
