using System;
using System.Reactive.Linq;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;

namespace Toggl.Core.Interactors
{
    internal sealed class GetCurrentUserInteractor : IInteractor<IObservable<IThreadSafeUser>>
    {
        private readonly ISingletonDataSource<IThreadSafeUser> dataSource;

        public GetCurrentUserInteractor(ISingletonDataSource<IThreadSafeUser> dataSource)
        {
            this.dataSource = dataSource;
        }

        public IObservable<IThreadSafeUser> Execute()
            => dataSource.Current.FirstAsync();
    }
}
