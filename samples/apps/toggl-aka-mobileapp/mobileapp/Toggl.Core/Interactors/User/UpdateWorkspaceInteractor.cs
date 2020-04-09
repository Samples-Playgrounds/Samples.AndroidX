using System;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;

namespace Toggl.Core.Interactors
{
    internal class UpdateWorkspaceInteractor : IInteractor<IObservable<IThreadSafeUser>>
    {
        private ISingletonDataSource<IThreadSafeUser> dataSource;
        private long selectedWorkspaceId;

        public UpdateWorkspaceInteractor(ISingletonDataSource<IThreadSafeUser> dataSource, long selectedWorkspaceId)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(selectedWorkspaceId, nameof(selectedWorkspaceId));

            this.dataSource = dataSource;
            this.selectedWorkspaceId = selectedWorkspaceId;
        }

        public IObservable<IThreadSafeUser> Execute()
            => dataSource.Get()
                .Select(user => user.With(selectedWorkspaceId))
                .SelectMany(dataSource.Update);
    }
}