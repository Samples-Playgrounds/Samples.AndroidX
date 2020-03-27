using System;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    internal class CreateClientInteractor : IInteractor<IObservable<IThreadSafeClient>>
    {
        private readonly long workspaceId;
        private readonly string clientName;
        private readonly IIdProvider idProvider;
        private readonly ITimeService timeService;
        private readonly IDataSource<IThreadSafeClient, IDatabaseClient> dataSource;

        public CreateClientInteractor(
            IIdProvider idProvider,
            ITimeService timeService,
            IDataSource<IThreadSafeClient, IDatabaseClient> dataSource,
            string clientName,
            long workspaceId)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(idProvider, nameof(idProvider));
            Ensure.Argument.IsNotNull(clientName, nameof(clientName));
            Ensure.Argument.IsNotNull(workspaceId, nameof(workspaceId));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.idProvider = idProvider;
            this.timeService = timeService;
            this.dataSource = dataSource;
            this.clientName = clientName;
            this.workspaceId = workspaceId;
        }

        public IObservable<IThreadSafeClient> Execute()
            => dataSource.Create(new Client(
                idProvider.GetNextIdentifier(),
                workspaceId,
                clientName,
                timeService.CurrentDateTime,
                SyncStatus.SyncNeeded
            ));
    }
}