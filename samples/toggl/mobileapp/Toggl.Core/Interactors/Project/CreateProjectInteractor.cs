using System;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.DTOs;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors
{
    internal class CreateProjectInteractor : IInteractor<IObservable<IThreadSafeProject>>
    {
        private readonly CreateProjectDTO dto;
        private readonly IIdProvider idProvider;
        private readonly ITimeService timeService;
        private readonly IDataSource<IThreadSafeProject, IDatabaseProject> dataSource;

        public CreateProjectInteractor(
            IIdProvider idProvider,
            ITimeService timeService,
            IDataSource<IThreadSafeProject, IDatabaseProject> dataSource,
            CreateProjectDTO dto)
        {
            Ensure.Argument.IsNotNull(dto, nameof(dto));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(idProvider, nameof(idProvider));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.dto = dto;
            this.dataSource = dataSource;
            this.idProvider = idProvider;
            this.timeService = timeService;
        }

        public IObservable<IThreadSafeProject> Execute()
            => idProvider.GetNextIdentifier()
                .Apply(Project.Builder.Create)
                .SetName(dto.Name)
                .SetColor(dto.Color)
                .SetClientId(dto.ClientId)
                .SetBillable(dto.Billable)
                .SetWorkspaceId(dto.WorkspaceId)
                .SetAt(timeService.CurrentDateTime)
                .SetSyncStatus(SyncStatus.SyncNeeded)
                .SetIsPrivate(dto.IsPrivate)
                .Build()
                .Apply(dataSource.Create);
    }
}
