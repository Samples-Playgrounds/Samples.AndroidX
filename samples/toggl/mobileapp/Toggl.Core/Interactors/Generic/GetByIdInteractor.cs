using System;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors.Generic
{
    public sealed class GetByIdInteractor<TThreadsafe, TDatabase>
        : TrackableInteractor, IInteractor<IObservable<TThreadsafe>>
        where TDatabase : IDatabaseModel
        where TThreadsafe : TDatabase, IThreadSafeModel
    {
        private readonly IDataSource<TThreadsafe, TDatabase> dataSource;
        private readonly long id;

        public GetByIdInteractor(IDataSource<TThreadsafe, TDatabase> dataSource, IAnalyticsService analyticsService, long id)
            : base(analyticsService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;
            this.id = id;
        }

        public IObservable<TThreadsafe> Execute()
            => dataSource.GetById(id);
    }
}
