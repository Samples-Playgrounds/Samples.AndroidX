using System;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.DTOs;
using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage;

namespace Toggl.Core.Interactors
{
    internal class UpdateUserInteractor : IInteractor<IObservable<IThreadSafeUser>>
    {
        private readonly EditUserDTO dto;
        private readonly ITimeService timeService;
        private readonly ISingletonDataSource<IThreadSafeUser> dataSource;

        public UpdateUserInteractor(ITimeService timeService, ISingletonDataSource<IThreadSafeUser> dataSource, EditUserDTO dto)
        {
            Ensure.Argument.IsNotNull(dto, nameof(dto));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.dto = dto;
            this.dataSource = dataSource;
            this.timeService = timeService;
        }

        public IObservable<IThreadSafeUser> Execute()
            => dataSource.Get()
                .Select(updatedUser)
                .SelectMany(dataSource.Update);

        private IThreadSafeUser updatedUser(IThreadSafeUser existing)
            => User.Builder
                   .FromExisting(existing)
                   .SetBeginningOfWeek(dto.BeginningOfWeek)
                   .SetSyncStatus(SyncStatus.SyncNeeded)
                   .SetAt(timeService.CurrentDateTime)
                   .Build();
    }
}