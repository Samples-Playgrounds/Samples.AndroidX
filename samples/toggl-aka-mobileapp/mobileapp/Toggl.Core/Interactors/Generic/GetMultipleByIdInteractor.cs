using System;
using System.Collections.Generic;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Models;
using Toggl.Storage.Models;

namespace Toggl.Core.Interactors.Generic
{
    public sealed class GetMultipleByIdInteractor<TThreadsafe, TDatabase>
        : IInteractor<IObservable<IEnumerable<TThreadsafe>>>
        where TDatabase : IDatabaseModel, IIdentifiable
        where TThreadsafe : TDatabase, IThreadSafeModel
    {
        private readonly IDataSource<TThreadsafe, TDatabase> dataSource;
        private readonly long[] ids;

        public GetMultipleByIdInteractor(IDataSource<TThreadsafe, TDatabase> dataSource, long[] ids)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));

            this.dataSource = dataSource;
            this.ids = ids;
        }

        public IObservable<IEnumerable<TThreadsafe>> Execute()
            => dataSource.GetByIds(ids);
    }
}
