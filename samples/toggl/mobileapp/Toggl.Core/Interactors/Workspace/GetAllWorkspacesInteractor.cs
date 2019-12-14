using System;
using System.Collections.Generic;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;

namespace Toggl.Core.Interactors
{
    internal sealed class GetAllWorkspacesInteractor : IInteractor<IObservable<IEnumerable<IThreadSafeWorkspace>>>
    {
        private readonly ITogglDataSource dataSource;

        public GetAllWorkspacesInteractor(ITogglDataSource dataSource)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;
        }

        public IObservable<IEnumerable<IThreadSafeWorkspace>> Execute()
            => dataSource.Workspaces.GetAll();
    }
}
